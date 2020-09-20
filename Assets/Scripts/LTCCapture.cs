using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LTCCapture : MonoBehaviour
{
    // Audio
    private string _device = "";
    private AudioClip _audioInput = null;

    private void Start()
    {
        // List devices
        foreach (var d in Microphone.devices)
            Debug.Log(d);

        // Get device
        _device = Microphone.devices[6];

        // Start capture
        _audioInput = Microphone.Start(_device, true, 1, 44100);
        Debug.Log("Capture " + _device);
    }

    private void Stop()
    {
        // Stop capture
        Microphone.End(_device);
    }

    private void Update()
    {

    }
}
