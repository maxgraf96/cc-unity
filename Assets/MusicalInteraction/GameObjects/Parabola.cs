using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class Parabola : MonoBehaviour
{
    Mesh mesh;
    MeshCollider meshCollider;
    public float meshWidth;

    public NoteObject startNO, endNO;
    private Vector3 startPosition, endPosition;
    public int resolution;
    public float height;
    float overlapMostProminentStart;
    float overlap;

    List<Parabola> children = null;

    public float Overlap { get => overlap; set => overlap = value; }

    public Parabola() { }

    public void ApplyNoteObject(ref NoteObject start, ref NoteObject end, float overlapMostProminentStart, float overlap, int resolution, float height)
    {
        startNO = start;
        endNO = end;
        startPosition = start.transform.position;
        endPosition = end.transform.position;

        this.overlapMostProminentStart = overlapMostProminentStart;
        this.overlap = overlap;
        this.height = height;
        this.resolution = resolution;
        meshWidth = 0.04f * height;
    }

    public void ApplyParabolas(ref List<Parabola> children, float overlapStart, float overlap, float height)
    {
        this.children = children;
        overlapMostProminentStart = overlapStart;
        startPosition = children[0].startPosition;
        endPosition = children[1].endPosition;
        this.overlap = overlap;
        this.height = height;
        resolution = 30;
        meshWidth = 0.04f * overlap * 0.10f;
    }

    public virtual void PlayChord()
    {
        if (IsParent())
        {
            foreach (var child in children)
                child.PlayChord();
        }
        else
        {
            startNO.noteOn();
            endNO.noteOn();
        }
    }

    public virtual void ReleaseChord()
    {
        if (IsParent())
        {
            foreach (var child in children)
                child.ReleaseChord();
        }
        else
        {
            startNO.noteOff();
            endNO.noteOff();
        }
    }

    public bool IsParent()
    {
        return children != null;
    }

    public (bool isOverlap, float start, float duration) OverlapsWith(Parabola other)
    {
        float thisStart = overlapMostProminentStart;
        float thisEnd = thisStart + overlap;

        float otherStart = other.overlapMostProminentStart;
        float otherEnd = otherStart + other.overlap;

        float start = Mathf.Max(thisStart, otherStart);
        float end = Mathf.Min(thisEnd, otherEnd);

        float maxOverlap = Mathf.Max(this.overlap, other.overlap);

        if (start <= end)
        {
            // Overlap exists
            // Overlapping range will be [e...f] -> get size of overlap
            float overlap = end - start;

            if (overlap >= maxOverlap)
                return (true, start, overlap);
        }


        return (false, 0.0f, 0.0f);
    }

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    private void OnValidate()
    {
        if (mesh != null && Application.isPlaying)
        {
            MakeArcMesh(CalculateArcArray());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MakeArcMesh(CalculateArcArray());

        //var collider = gameObject.GetComponent<MeshCollider>();
    }

    void Update()
    {
        /*
        if (transform.hasChanged)
        {
            MakeArcMesh(CalculateArcArray());
        }
        */
    }

    void MakeArcMesh(Vector3[] arcVerts)
    {
        mesh.Clear();
        Vector3[] vertices = new Vector3[(resolution + 1) * 2];
        int[] triangles = new int[resolution * 6 * 2]; // because every quad is 2 triangles so 6 vertices, and we want to render both sides

        for (int i = 0; i <= resolution; i++)
        {
            //set vertices
            vertices[i * 2] = new Vector3(arcVerts[i].x, arcVerts[i].y + meshWidth * 0.5f, arcVerts[i].z);
            vertices[i * 2 + 1] = new Vector3(arcVerts[i].x, arcVerts[i].y + meshWidth * -0.5f, arcVerts[i].z);

            //set triangles
            if (i != resolution)
            {
                triangles[i * 12] = i * 2;
                triangles[i * 12 + 1] = triangles[i * 12 + 4] = i * 2 + 1;
                triangles[i * 12 + 2] = triangles[i * 12 + 3] = (i + 1) * 2;
                triangles[i * 12 + 5] = (i + 1) * 2 + 1;

                triangles[i * 12 + 6] = i * 2;
                triangles[i * 12 + 7] = triangles[i * 12 + 10] = (i + 1) * 2;
                triangles[i * 12 + 8] = triangles[i * 12 + 9] = i * 2 + 1;
                triangles[i * 12 + 11] = (i + 1) * 2 + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Use complex mesh for interaction
        if(meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();

        }
        meshCollider.sharedMesh = mesh;
    }

    //Create an array of Vector 3 positions for the arc
    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t);
        }

        return arcArray;
    }

    Vector3 CalculateArcPoint(float t)
    {
        Vector3 lerped = Vector3.Lerp(startPosition, endPosition, t);

        float x = lerped.x;
        float y = height * Mathf.Sin(Mathf.PI * t);
        float z = lerped.z;
        return new Vector3(x, y, z);
    }

    public override bool Equals(object other)
    {
        var o = (Parabola)other;

        return 
               this.startPosition.Equals(o.startPosition)
            && this.endPosition.Equals(o.endPosition)
            && this.height == o.height
            && this.meshWidth == o.meshWidth
            && this.overlap == o.overlap
            && this.overlapMostProminentStart == o.overlapMostProminentStart;
    }

    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + startPosition.GetHashCode();
        hash = (hash * 7) + endPosition.GetHashCode();
        hash = (hash * 7) + height.GetHashCode();
        hash = (hash * 7) + meshWidth.GetHashCode();
        hash = (hash * 7) + overlap.GetHashCode();
        hash = (hash * 7) + overlapMostProminentStart.GetHashCode();   
        return hash;
    }
}