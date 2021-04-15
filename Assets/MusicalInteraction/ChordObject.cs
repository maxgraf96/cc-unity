using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChordObject : NoteObject
{
    private Chord chord;

    public Chord Chord { get => chord; set => chord = value; }

    public ChordObject(Note note) : base(note)
    {

    }


    public override void noteOn()
    {
        IsPlaying = true;
        foreach (Note note in Chord.Notes)
        {
            OSCSender.SendNoteOn(note.MidiNumber);
        }
        interactable.HasPress = true;
        //StartCoroutine(ScaleUp(transform, 0.1f, 0.1f));
    }

    public override void noteOff()
    {
        IsPlaying = false;
        foreach (Note note in Chord.Notes)
        {
            OSCSender.SendNoteOff(note.MidiNumber);
        }
        interactable.HasPress = false;

        //if (Math.Abs(transform.localScale.x - origScale.x) > 0.01)
        //{
        //    StartCoroutine(ScaleDown(transform, 0.1f, 0.1f));
        //}
    }

    public void ApplyChord(Chord chord)
    {
        Chord = chord;
        SetText(chord.CommonName);
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override float GetStart()
    {
        return Chord.Start;
    }

    public override float GetDuration()
    {
        return Chord.Duration;
    }
}
