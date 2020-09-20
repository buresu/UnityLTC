using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LTCCapture : MonoBehaviour
{
    // Audio
    private string _device = "";
    private AudioClip _audioInput = null;
    private List<float> _audioBuffer = new List<float>();
    private int _lastAudioPos = 0;

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
        // Update audio buffer
        int pos = Microphone.GetPosition(_device);

        int size1 = pos < _lastAudioPos ? _audioInput.samples * _audioInput.channels - _lastAudioPos : 0;
        int size2 = pos >= _lastAudioPos ? pos - _lastAudioPos : pos;

        if (size1 > 0)
        {
            var data = new float[size1];
            _audioInput.GetData(data, _lastAudioPos);
            _audioBuffer.AddRange(new List<float>(data));
        }

        if (size2 > 0)
        {
            var data = new float[size2];
            _audioInput.GetData(data, 0);
            _audioBuffer.AddRange(new List<float>(data));
        }

        _lastAudioPos = pos;

        //Debug.Log(_audioBuffer.Count);
    }
}
