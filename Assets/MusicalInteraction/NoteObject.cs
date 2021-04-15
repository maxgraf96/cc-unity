using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    // Set via editor or when instantiating
    private Note note;
    private bool isPlaying;
    protected Vector3 origScale;
    protected Interactable interactable;

    public Note Note { get => note; set => note = value; }
    public bool IsPlaying { get => this.isPlaying; set => this.isPlaying = value; }

    public void SetNoteNumber(int midiNoteNumber)
    {
        note.MidiNumber = midiNoteNumber;
    }

    public NoteObject(Note note)
    {
        this.note = note;
    }

    public void ApplyNote(Note note)
    {
        Note = note;
        // Set text in UI
        SetText(Utility.getNoteFromMidiNumber(note.MidiNumber));
    }

    public virtual void noteOn()
    {
        interactable.HasPress = true;
        IsPlaying = true;
        OSCSender.SendNoteOn(note.MidiNumber);
        //StartCoroutine(ScaleUp(transform, 0.1f, 0.1f));
    }

    public virtual void noteOff()
    {
        IsPlaying = false;
        OSCSender.SendNoteOff(note.MidiNumber);
        interactable.HasPress = false;

        //if (Math.Abs(transform.localScale.x - origScale.x) > 0.01)
        //{
        //    StartCoroutine(ScaleDown(transform, 0.1f, 0.1f));
        //}
    }

    public void SetText(string text)
    {
        var label = gameObject.transform.Find("Label").gameObject;
        label.GetComponent<TextMeshPro>().text = text;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        origScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        interactable = gameObject.GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual float GetStart()
    {
        return Note.Start;
    }

    public virtual float GetDuration()
    {
        return Note.Duration;
    }

    protected IEnumerator ScaleUp(Transform transform, float amount, float duration)
    {
        Vector3 initialScale = transform.localScale;
        Vector3 upScale = initialScale + new Vector3(amount, amount, amount);
        float currentTime = 0.0f;
        do
        {
            transform.localScale = Vector3.Lerp(initialScale, upScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= duration);

        transform.localScale = upScale;
    }

    protected IEnumerator ScaleDown(Transform transform, float amount, float duration)
    {
        Vector3 initialScale = transform.localScale;
        Vector3 downScale = initialScale - new Vector3(amount, amount, amount);
        float currentTime = 0.0f;
        do
        {
            transform.localScale = Vector3.Lerp(initialScale, downScale, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= duration);

        transform.localScale = downScale;
    }
}
