using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Chord
{
    List<Note> notes = new List<Note>();
    string commonName;
    // Where note starts and duration in seconds
    private float start = 0.0f;
    private float duration = 0.0f;

    private List<float> starts = new List<float>();
    private List<float> durations = new List<float>();

    public Chord()
    {

    }

    public Chord(List<Note> notes)
    {
        Notes = notes;
    }

    public void AddNote(Note note)
    {
        Notes.Add(note);
    }

    public List<Note> Notes { get => notes; set => notes = value; }
    public string CommonName { get => commonName; set => commonName = value; }
    public float Start { get => start; set => start = value; }
    public float Duration { get => duration; set => duration = value; }
    public List<float> Starts { get => starts; set => starts = value; }
    public List<float> Durations { get => durations; set => durations = value; }
}
