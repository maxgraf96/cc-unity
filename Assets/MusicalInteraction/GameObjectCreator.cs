using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectCreator : MonoBehaviour
{
    GameObject singleNoteCubePrefab;
    GameObject chordCubePrefab;
    GameObject parabolaPrefab;
    ChordBuilder chordBuilder;

    List<GameObject> currentScene = new List<GameObject>();

    float originSingleNotes = -1.5f;
    float counterSingleNotes = 0f;
    float originChords = -1.5f;
    float counterChords = 0f;

    private List<NoteObject> notesAndChords = new List<NoteObject>();

    public void GenerateSingleNote(Note note)
    {
        bool isExisting = false;
        // Check if note is already included
        foreach(NoteObject existingNote in notesAndChords)
        {
            if(existingNote.Note.MidiNumber == note.MidiNumber)
            {
                isExisting = true;
                existingNote.Note.Starts.Add(note.Start);
                existingNote.Note.Durations.Add(note.Duration);
                break;
            }
        }
        if (!isExisting)
        {
            var singleNoteCube = Instantiate(singleNoteCubePrefab, new Vector3(originSingleNotes + counterSingleNotes, 0, 2), Quaternion.identity);
            singleNoteCube.GetComponent<NoteObject>().ApplyNote(note);
            currentScene.Add(singleNoteCube);
            counterSingleNotes += 0.5f;
            notesAndChords.Add(singleNoteCube.GetComponent<NoteObject>());
        }
    }

    public void GenerateChord(Chord chord)
    {
        var chordCube = Instantiate(chordCubePrefab, new Vector3(originChords + counterChords, 0.6f, 2), Quaternion.identity);
        chordCube.GetComponent<ChordObject>().ApplyChord(chord);
        currentScene.Add(chordCube);
        counterChords += 0.5f;
        notesAndChords.Add(chordCube.GetComponent<ChordObject>());
    }

    public void CreateChordConnections()
    {
        chordBuilder.CreateChordConnections(ref notesAndChords);

    }
    public void clearScene()
    {
        counterSingleNotes = 0f;
        counterChords = 0f;
        foreach(var element in currentScene)
        {
            Destroy(element);
        }
        chordBuilder.Clear();
        // Empty list
        currentScene.Clear();
        notesAndChords.Clear();
    }


    // Use this for initialization
    void Start()
    {
        singleNoteCubePrefab = gameObject.GetComponent<Initialise>().singleNoteCubePrefab;
        chordCubePrefab = gameObject.GetComponent<Initialise>().chordCubePrefab;
        parabolaPrefab = gameObject.GetComponent<Initialise>().parabolaPrefab;
        gameObject.GetComponent<MidiPlayer>().SetNotesAndChords(ref notesAndChords);
        chordBuilder = GetComponent<ChordBuilder>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}