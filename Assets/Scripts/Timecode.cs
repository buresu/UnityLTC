using System.Collections;
using System.Collections.Generic;

public class Timecode
{
    public int hours = 0;
    public int minutes = 0;
    public int seconds = 0;
    public int frames = 0;
    public bool dropFrame = false;
    public bool colorFrame = false;

    public override string ToString()
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2};{3:D2}", hours, minutes, seconds, frames);
    }

    public static Timecode FromBitString(string str)
    {
        Timecode tc = new Timecode();

        if (str.IndexOf("0011111111111101") < 0 || str.Length != 80)
            return tc;

        tc.frames = Decode4BitString(str, 0) + Decode2BitString(str, 8) * 10;
        tc.seconds = Decode4BitString(str, 16) + Decode3BitString(str, 24) * 10;
        tc.minutes = Decode4BitString(str, 32) + Decode3BitString(str, 40) * 10;
        tc.hours = Decode4BitString(str, 48) + Decode2BitString(str, 56) * 10;
        tc.dropFrame = Decode1BitString(str, 10) > 0;
        tc.colorFrame = Decode1BitString(str, 11) > 0;

        return tc;
    }

    static int Decode1BitString(string str, int pos)
    {
        return int.Parse(str.Substring(pos, 1));
    }

    static int Decode2BitString(string str, int pos)
    {
        int v = Decode1BitString(str, pos);
        v += Decode1BitString(str, pos + 1) * 2;
        return v;
    }

    static int Decode3BitString(string str, int pos)
    {
        int v = Decode2BitString(str, pos);
        v += Decode1BitString(str, pos + 2) * 4;
        return v;
    }

    static int Decode4BitString(string str, int pos)
    {
        int v = Decode3BitString(str, pos);
        v += Decode1BitString(str, pos + 3) * 8;
        return v;
    }
}
