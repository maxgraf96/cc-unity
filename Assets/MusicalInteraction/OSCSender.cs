using UnityEngine;

public static class OSCSender
{
    private static SharpOSC.UDPSender sender;

    public static void init()
    {
        sender = new SharpOSC.UDPSender("192.168.1.100", 1337);
        SendLog("OSC hello");

        Debug.Log("OSCSender started.");
    }

    public static void SendGenerateFromScratch()
    {
        string dummy = "dummy";
        var message = new SharpOSC.OscMessage("/something/generate_from_scratch", dummy);
        sender.Send(message);
    }

    public static void SendNoteOn(string noteNumber)
    {
        var message = new SharpOSC.OscMessage("/something/note_on", noteNumber);
        sender.Send(message);
    }

    public static void SendNoteOn(int noteNumber)
    {
        var message = new SharpOSC.OscMessage("/something/note_on", noteNumber);
        sender.Send(message);
    }

    public static void SendNoteOff(string noteNumber)
    {
        var message = new SharpOSC.OscMessage("/something/note_off", noteNumber);
        sender.Send(message);
    }

    public static void SendNoteOff(int noteNumber)
    {
        var message = new SharpOSC.OscMessage("/something/note_off", noteNumber);
        sender.Send(message);
    }

    public static void SendPitchWheel(int value)
    {
        var message = new SharpOSC.OscMessage("/something/pitch_wheel", value);
        sender.Send(message);
    }

    public static void SendLog(string content)
    {
        var message = new SharpOSC.OscMessage("/something/log", content);
        sender.Send(message);
    }
}