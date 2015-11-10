using UnityEngine;
using System;
using System.Collections;

public class Options {

    private bool bEnableUserBackgrounds = true;
    private bool bEnableUserAudio = true;
    private float fAudioVolumeLevel = 100.0f;  //percentage 100 == 100%

    public Options()
    { 
        
    }

    public Options(bool _userBG, bool _userAudio, float _volume)
    {
        this.bEnableUserAudio = _userAudio;
        this.bEnableUserBackgrounds = _userBG;
        this.fAudioVolumeLevel = _volume;
    }

    public bool UserBackgroundsEnabled
    {
        get { return this.bEnableUserBackgrounds; }
        set { this.bEnableUserBackgrounds = value; }
    }

    public bool UserAudioEnabled
    {
        get { return this.bEnableUserAudio; }
        set { this.bEnableUserAudio = value; }
    }

    public float VolumeLevel
    {
        get { return this.fAudioVolumeLevel; }
        set { this.fAudioVolumeLevel = value; }
    }

    public string this[string param]
    {
        get
        {
            if (param.Equals("bEnableUserAudio"))
            {
                return this.bEnableUserAudio.ToString();
            }
            else if (param.Equals("bEnableUserBackgrounds"))
            {
                return this.bEnableUserBackgrounds.ToString();
            }
            else if (param.Equals("fAudioVolumeLevel"))
            {
                return this.fAudioVolumeLevel.ToString();
            }

            return string.Empty;
        }

        set
        {
            if (param.Equals("bEnableUserAudio"))
            {
                int extra;
                bool tmp;
                if (!bool.TryParse(value, out tmp))
                {
                    if (!int.TryParse(value, out extra))
                    {
                        throw new Exception("Bad incoming value");
                    }

                    if(extra > 0) tmp = true;
                    else tmp = false;
                }

                this.bEnableUserAudio = tmp;
            }
            else if (param.Equals("bEnableUserBackgrounds"))
            {
                int extra;
                bool tmp;
                if (!bool.TryParse(value, out tmp))
                {
                    if (!int.TryParse(value, out extra))
                    {
                        throw new Exception("Bad incoming value");
                    }

                    if (extra > 0) tmp = true;
                    else tmp = false;
                }

                this.bEnableUserBackgrounds = tmp;
            }
            else if (param.Equals("fAudioVolumeLevel"))
            {
                float tmp;

                if (!float.TryParse(value, out tmp))
                {
                    throw new Exception("Bad incoming value");
                }

                this.fAudioVolumeLevel = tmp;
            }
        }
    }
}
