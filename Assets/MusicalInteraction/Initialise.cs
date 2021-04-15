using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialise : MonoBehaviour
{
    public GameObject singleNoteCubePrefab;
    public GameObject chordCubePrefab;
    public GameObject parabolaPrefab;
    private OSCServer oscServer;

    // Start is called before the first frame update
    void Start()
    {
        OSCSender.init();
        oscServer = gameObject.AddComponent<OSCServer>();

        SendGenerateFromScratch();
    }

    public void SendGenerateFromScratch()
    {
        OSCSender.SendGenerateFromScratch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
