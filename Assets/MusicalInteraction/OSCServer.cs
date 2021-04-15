using SharpOSC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class OSCServer : MonoBehaviour
{
    private static bool receivedNote = false;
    private static bool receivedChord = false;
    private static bool isDoneReceivingNotes = false;

    private static Note individualNote;
    private static Chord chord;

    protected class OSCThread
    {
        private UDPListener listener;
        private GameObject gameObject;

        public OSCThread(GameObject gameObject, int port)
        {
            listener = new UDPListener(port);
            this.gameObject = gameObject;

            //listener = new UDPListener(1338, callback);
            Debug.Log("OSCServer started at IP address " + GetLocalIPAddress());
        }

        public void poll()
        {
            OscBundle bundle = null;
            while (bundle == null)
            {
                bundle = (OscBundle)listener.Receive();
                Thread.Sleep(1);
            }

            //Debug.Log("Received a bundle!");
            List<OscMessage> messages = bundle.Messages;

            OscMessage lastMessage = bundle.Messages[bundle.Messages.Count - 1];

            switch (lastMessage.Address)
            {
                case "/notes_receiver":
                    Note note = new Note();
                    object midiNumber = lastMessage.Arguments[0];
                    object start = lastMessage.Arguments[1];
                    object duration = lastMessage.Arguments[2];
                    // Typecheck to make sure that message is intact
                    if (midiNumber is int midiNumberInt)
                    {
                        note.MidiNumber = midiNumberInt;
                        note.Start = (float)start;
                        note.Duration = (float)duration;
                        note.Starts.Add((float) start);
                        note.Durations.Add((float)duration);

                        individualNote = note;
                        receivedNote = true;
                    }
                    break;
                case "/notes_complete":
                    isDoneReceivingNotes = true;
                    break;
                case "/chords_receiver":
                    int i = 0;
                    object commonName = lastMessage.Arguments[i];
                    // Typecheck to make sure that message is intact
                    if (commonName is string commonNameString)
                    {
                        // Initialise chord object
                        chord = new Chord();
                        chord.CommonName = commonNameString;
                        // Add all subsequent ints (notes) to the chord object until separator "|" is hit
                        object next = lastMessage.Arguments[++i];
                        while (next is int nextMidiNumberInt)
                        {
                            Note individualNote = new Note(nextMidiNumberInt);
                            chord.AddNote(individualNote);
                            next = lastMessage.Arguments[++i];
                        }
                        // After notes (int) in OSC message it's beat and duration (floats)
                        var chordStart = (float)next;
                        next = lastMessage.Arguments[++i];
                        var chordDuration = (float)next;

                        chord.Start = chordStart;
                        chord.Duration = chordDuration;
                        receivedChord = true;
                    }
                    break;
            }
            poll();
        }
    }

    void Start()
    {
        OSCThread thread = new OSCThread(gameObject, 1338);
        ThreadStart childStart = new ThreadStart(thread.poll);
        Thread childThread = new Thread(childStart);
        childThread.Start();
    }

    void Update()
    {
        if (receivedNote)
        {
            // Send individual notes to game object creator
            gameObject.GetComponent<GameObjectCreator>().GenerateSingleNote(individualNote);
            receivedNote = false;
        }
        if (receivedChord)
        {
            // Send chords to game object creator
            gameObject.GetComponent<GameObjectCreator>().GenerateChord(chord);
            receivedChord = false;
        }
        if (isDoneReceivingNotes)
        {
            isDoneReceivingNotes = false;
            gameObject.GetComponent<GameObjectCreator>().CreateChordConnections();
        }
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
