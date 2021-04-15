using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChordBuilder : MonoBehaviour
{
    public List<GameObject> parabolas = new List<GameObject>();

    public GameObject parabolaPrefab;

    struct Overlaps  {
        Dictionary<float, float> overlaps;
        public Overlaps(float start, float duration)
        {
            overlaps = new Dictionary<float, float>();
            AddOrUpdate(start, duration);
        }

        public float GetTotalDuration()
        {
            return overlaps.Values.Sum();
        }
        public float GetMostProminentStart()
        {
            return overlaps.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }
        public float GetMostProminentDuration()
        {
            return overlaps.Aggregate((l, r) => l.Value > r.Value ? l : r).Value;
        }
        public void AddOrUpdate(float start, float duration)
        {
            if (overlaps.ContainsKey(start))
                overlaps[start] += duration;
            else
                overlaps.Add(start, duration);
        }
    }

    public ChordBuilder()
    {
    }

    public void CreateHigherOrderConnections(List<GameObject> currentScope, int depth)
    {
        List<GameObject> copy = currentScope.ToList();
        List<GameObject> newCreated = new List<GameObject>();

        Dictionary<(Parabola, Parabola), bool> visited = new Dictionary<(Parabola, Parabola), bool>();

        int count = 0;
        foreach(GameObject parGO in copy)
        {
            var par = parGO.GetComponent<Parabola>();

            foreach (GameObject parOtherGO in copy)
            {
                var parOther = parOtherGO.GetComponent<Parabola>();
                if (par.Equals(parOther))
                    continue;
                if (visited.ContainsKey((par, parOther)) || visited.ContainsKey((parOther, par)))
                    continue;

                (bool isOverlap, float start, float duration) overlap = par.OverlapsWith(parOther);
                if (overlap.isOverlap)
                {
                    var parabola = Instantiate(parabolaPrefab, Vector3.zero, Quaternion.identity);
                    List<Parabola> children = new List<Parabola>();
                    children.Add(par);
                    children.Add(parOther);
                    parabola.GetComponent<Parabola>().ApplyParabolas(ref children, overlap.start, overlap.duration, 1 + depth * 0.2f);
                    if(parabolas.Contains(parabola) || newCreated.Contains(parabola))
                    {
                        DestroyImmediate(parabola);
                    } else
                    {
                        count++;
                        visited.Add((par, parOther), true);
                        // Add to scene
                        parabolas.Add(parabola);
                        // Add to scope for next iteration
                        newCreated.Add(parabola);
                    }
                }
            }
        }
        Debug.Log("Depth " + depth + ", " + count + " parabolas created");
        if (depth > 3)
        {
            return;
        }
        else if (count > 1)
            CreateHigherOrderConnections(newCreated, depth + 1);
    }

    public void CreateChordConnections(ref List<NoteObject> noteObjects)
    {
        parabolas.Clear();
        Dictionary<(NoteObject, NoteObject), Overlaps> overlaps = new Dictionary<(NoteObject, NoteObject), Overlaps>();

        foreach(NoteObject noteObjectOuter in noteObjects)
        {
            Note noteOuter = noteObjectOuter.Note;
            foreach (NoteObject noteObjectInner in noteObjects)
            {
                if (noteObjectOuter.Equals(noteObjectInner))
                    continue;
                
                Note noteInner = noteObjectInner.Note;
                for(int outerIdx = 0; outerIdx < noteOuter.Durations.Count; outerIdx++)
                {
                    for (int innerIdx = 0; innerIdx < noteInner.Durations.Count; innerIdx++)
                    {
                        float outerStart = noteOuter.Starts[outerIdx];
                        float outerEnd = outerStart + noteOuter.Durations[outerIdx];

                        float innerStart = noteInner.Starts[innerIdx];
                        float innerEnd = innerStart + noteInner.Durations[innerIdx];

                        float start = Math.Max(outerStart, innerStart);
                        float end = Math.Min(outerEnd, innerEnd);

                        if (start <= end)
                        {
                            // Overlap exists
                            // Overlapping range will be [e...f] -> get size of overlap
                            float overlap = end - start;

                            if (overlap < 0.5f)
                                continue;

                            if(overlaps.ContainsKey((noteObjectOuter, noteObjectInner)))
                            {
                                overlaps[(noteObjectOuter, noteObjectInner)].AddOrUpdate(start, overlap);
                            } else if(overlaps.ContainsKey((noteObjectInner, noteObjectOuter)))
                            {
                                //overlaps[(noteObjectInner, noteObjectOuter)].AddOrUpdate(start, overlap);
                            }
                            else
                            {
                                overlaps.Add((noteObjectOuter, noteObjectInner), new Overlaps(start, overlap));
                            }
                        }
                    }
                }
            }
        }

        foreach (KeyValuePair<(NoteObject, NoteObject), Overlaps> entry in overlaps)
        {
            var no1 = entry.Key.Item1;
            var no2 = entry.Key.Item2;
            var overlap = entry.Value;

            var parabola = Instantiate(parabolaPrefab, Vector3.zero, Quaternion.identity);
            parabola.GetComponent<Parabola>()
                .ApplyNoteObject(ref no1, ref no2, overlap.GetMostProminentStart(), overlap.GetMostProminentDuration(), 30, 0.5f);
            parabolas.Add(parabola);
        }

        CreateHigherOrderConnections(parabolas, 1);
    }

    public void Clear()
    {
        foreach(var par in parabolas)
        {
            DestroyImmediate(par);
        }
        parabolas.Clear();
    }

    public static bool nearlyEqual(float a, float b, float epsilon)
    {
        float absA = Math.Abs(a);
        float absB = Math.Abs(b);
        float diff = Math.Abs(a - b);

        if (a == b)
        { // shortcut, handles infinities
            return true;
        }
        else
        { // use relative error
            return diff / (absA + absB) < epsilon;
        }
    }
}
