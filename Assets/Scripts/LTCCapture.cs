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

    // LTC
    private string _ltcBits = "";

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

        // Audio buffer to bit array
        if (_audioBuffer.Count > 0)
        {
            int count = 0;
            bool sign = _audioBuffer[0] < 0;
            bool flag = false;

            while (++count < _audioBuffer.Count)
            {
                if (sign != (_audioBuffer[count] < 0))
                {
                    sign = _audioBuffer[count] < 0;
                    //Debug.Log(count);
                    //Debug.Log(_audioInput.frequency / 3000);
                    _audioBuffer.RemoveRange(0, count - 1);
                    if (count > _audioInput.frequency / 3000)
                    {
                        _ltcBits += "0";
                        flag = false;
                    }
                    else if (count > _audioInput.frequency / 9000)
                    {
                        if (flag)
                        {
                            _ltcBits += "1";
                            flag = false;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    count = 0;
                    continue;
                }
            }
        }

        //Debug.Log(_ltcBits);

        // Find LTC block
        int index = _ltcBits.IndexOf("0011111111111101");

        if (index - 64 < 0)
            _ltcBits.Remove(0, index + 16);

        if (index - 64 > 0)
        {
            Debug.Log("find!");
            string ltcBlock = _ltcBits.Substring(index - 64, 80);
            Debug.Log(ltcBlock);
            _ltcBits.Remove(0, index + 16);
        }

        if (_ltcBits.Length > 96)
            _ltcBits.Remove(0, 80);
    }
}
