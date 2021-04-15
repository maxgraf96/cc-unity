using System;

public static class Utility
{
    public static String getNoteFromMidiNumber(int midiNote)
    {
        String[] note_names = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        return note_names[midiNote % 12] + ((midiNote / 12) - 1);
    }

    public static int getNoteNumberFromName(string name)
    {
        String[] note_names = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        int octave = Int32.Parse(name.Substring(name.Length - 1, 1));
        string noteDescription = name.Substring(0, name.Length - 1);

        return 24 + (octave - 1) * 12 + Array.IndexOf(note_names, noteDescription.ToUpper());
    }
}
