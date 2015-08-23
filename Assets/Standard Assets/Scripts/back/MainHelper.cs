using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MainHelper : MonoBehaviour {
    private Game currentGame;
    private Transform startingBallPosition;
    private List<AudioClip> clips;
    private int firstClipIndex = 0;
    private int currentClipIndex = 0;
    private int totalClips = 0;
    private AudioSource mainAudioSource;

    void Awake()
    {
        this.currentGame = new Game();
        
    }
	
    // Use this for initialization
	void Start () {
        this.clips = new List<AudioClip>();
        this.mainAudioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        this.startingBallPosition = GameObject.Find("Ball").transform;
#if UNITY_EDITOR
        this.BgLoader();
        this.BgAudioLoader();
#else
        this.BgLoader();
        this.BgAudioLoader();
        this.UserBgLoader();
        this.UserBgAudioLoader();
#endif
    }
	
	// Update is called once per frame
	void Update () {
        if (totalClips > 0)
        {
            if (!this.mainAudioSource.isPlaying)
            {
                ++this.currentClipIndex;
                if (this.currentClipIndex < this.totalClips)
                {
                    this.mainAudioSource.clip = this.clips[this.currentClipIndex];
                    this.mainAudioSource.Play();
                }
                else
                {
                    this.currentClipIndex = this.firstClipIndex;
                    this.mainAudioSource.clip = this.clips[this.currentClipIndex];
                    this.mainAudioSource.Play();
                }
            }
        }

#if UNITY_EDITOR //debug level switching
        if (Input.GetKeyUp(KeyCode.E))
        {
            object sender = new object();
            ChangeLevelEventArgs e = new ChangeLevelEventArgs(this.currentGame.Level, this.currentGame.Level, ChangeLevelReasons.NextLevelKeyPressed);
            EventSystem.FireChangeLevel(sender, e);
        }

        if (Input.GetKeyUp(KeyCode.Q))
        { 
            
        }
#endif
	}

    public Game GetCurrentGame()
    {
        return this.currentGame;
    }

    public Transform GetInitialBallTransform()
    {
        return this.startingBallPosition;
    }

    protected void BgLoader()
    {
        try
        {
            UnityEngine.Object[] objs = Resources.LoadAll(@"bg_textures/");
            List<Texture2D> textures = new List<Texture2D>();
            foreach (UnityEngine.Object obj in objs)
            {
                textures.Add((Texture2D)obj);
            }
            Texture2D tex = textures[UnityEngine.Random.Range(0, textures.Count)];
            
            if (tex != null)
            {
                GameObject.Find("Plane").GetComponent<Renderer>().material.mainTexture = tex;
            }
            else
            {
                throw new UnityException("Texture is null object");
            }            
        }
        catch (UnityException e)
        { 
#if UNITY_EDITOR
            Debug.Log("Can't load bg textures");
            Debug.Log(e.Message);
            return;
#else
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't load bg textures", e);
            EventSystem.FireInterfaceUpdate(this, ev);
#endif
        }   
    }

    protected void BgAudioLoader()
    {
        try
        {
            UnityEngine.Object[] objs = Resources.LoadAll(@"bg_audio/");
            
            foreach (UnityEngine.Object obj in objs)
            {
                this.clips.Add((AudioClip)obj);
            }
            this.totalClips = clips.Count;
            AudioClip clip = clips[0];
            
            this.mainAudioSource.clip = clip;
            this.mainAudioSource.Play();
        }
        catch (UnityException e)
        {
#if UNITY_EDITOR
            Debug.Log("Can't load audio clips");
            Debug.Log(e.Message);
            return;
#else
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't load audio clips", e);
            EventSystem.FireInterfaceUpdate(this, ev);
#endif
        }
    }

    protected void UserBgLoader()
    { 
        /*
         
         * TODO program user backgrounds loader prior to the final release
         
         */
    }

    protected void UserBgAudioLoader()
    { 
        /*
         
         * TODO Same as above but with user music
         
         */
    }
}
