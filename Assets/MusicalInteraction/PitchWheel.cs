using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchWheel : MonoBehaviour
{
    private float originalPosition = 0f;
    public bool isWheeling = false;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWheeling)
        {
            int val = (int) ((transform.position.y - originalPosition ) * 3000);
            OSCSender.SendPitchWheel(val);
        }
    }

    public void toggleWheeling()
    {
        isWheeling = !isWheeling;
        // Reset pitch wheel
        OSCSender.SendPitchWheel(0);
    }
}
