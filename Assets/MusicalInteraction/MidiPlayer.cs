using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MidiPlayer : MonoBehaviour
{
    // BPM
    private float bpm = 120f;
    private float timeSeconds = 0.0f;
    private float timeBeats = 0.0f;
    private float timeBar = 0.0f;
    private bool isPlaying = false;
    private float measure = 4.0f;

    public GameObject playButton;

    private List<NoteObject> notesAndChords;

    Stopwatch stopwatch;
    
    public List<NoteObject> NotesAndChords { get => notesAndChords; set => notesAndChords = value; }

    public void SetNotesAndChords(ref List<NoteObject> notesAndChords)
    {
        NotesAndChords = notesAndChords;
    }
    public void Play()
    {
        var label = playButton.transform.Find("Text").gameObject;
        if (!isPlaying)
        {
            isPlaying = true;
            // Change play button color
            label.GetComponent<TextMeshPro>().text = "Stop";
            stopwatch.Restart();
        }
        else
        {
            Stop();
            // Change play button color
            label.GetComponent<TextMeshPro>().text = "Play";
        }
    }

    public void Stop()
    {
        isPlaying = false;
        timeSeconds = 0.0f;
        stopwatch.Stop();

        foreach (NoteObject element in NotesAndChords)
        {
            element.noteOff();
        }

    }

    void Start()
    {
        stopwatch = Stopwatch.StartNew();
    }

    void Update()
    {
        if (isPlaying)
        {
            timeSeconds += Time.deltaTime;
            timeBeats = secondsToBeats();
            timeBar = secondsToBar();

            // Loop through notes and chords and trigger on/off
            foreach(NoteObject element in NotesAndChords)
            {
                if(element.GetType() == typeof(ChordObject))
                {
                    float start = element.GetStart();
                    float duration = element.GetDuration();
                    bool isPlaying = element.IsPlaying;

                    if (start <= timeSeconds
                        && start + duration > timeSeconds
                        && !isPlaying)
                    {
                        element.noteOn();
                    }
                    if (timeSeconds > start + duration && isPlaying)
                    {
                        element.noteOff();
                    }
                } else
                {
                    bool isPlaying = element.IsPlaying;
                    for (int i = 0; i < element.Note.Starts.Count; i++)
                    {
                        float start = element.Note.Starts[i];
                        float duration = element.Note.Durations[i];
                        float end = start + duration;
                        float margin = Time.deltaTime;

                        if(timeSeconds >= start - margin 
                            && timeSeconds <= start + margin
                            && !isPlaying)
                        {
                            element.noteOn();
                            break;
                        }
                        if (timeSeconds >= end - margin
                            && timeSeconds <= end + margin
                            && isPlaying)
                        {
                            element.noteOff();
                            break;
                        }
                    }
                }
            }
        }
    }

    float secondsToBeats()
    {
        return (bpm * timeSeconds) / 60;
    }

    float secondsToBar()
    {
        return (bpm * timeSeconds) / 60 / measure;
    }
}
