using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Note
{
    private int midiNumber = -1;
    // Where note starts and duration in seconds
    private float start = 0.0f;
    private float duration = 0.0f;

    private List<float> starts = new List<float>();
    private List<float> durations = new List<float>();

    public Note()
    {

    }

    public Note(int midiNumber, float start, float duration)
    {
        MidiNumber = midiNumber;
        Start = start;
        Duration = duration;
        starts.Add(start);
        durations.Add(duration);
    }

    // Constructor for adding notes to chord -> won't have beat and quarter length set -> will be set in chord
    public Note(int midiNumber)
    {
        MidiNumber = midiNumber;
    }

    public int MidiNumber { get => midiNumber; set => midiNumber = value; }
    public float Start { get => start; set => start = value; }
    public float Duration { get => duration; set => duration = value; }
    public List<float> Starts { get => starts; set => starts = value; }
    public List<float> Durations { get => durations; set => durations = value; }

}
