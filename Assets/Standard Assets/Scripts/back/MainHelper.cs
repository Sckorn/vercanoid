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
    private Queue<DelayedCollision> collisionQueue = new Queue<DelayedCollision>();
    public static GameSession CurrentGameSession = null;

    void Awake()
    {
        LevelFileCrypto.GenerateSessionCryptoKey();
        MainHelper.CurrentGameSession = new GameSession();
        this.currentGame = new Game();
    }
	
    // Use this for initialization
	void Start () {
        this.clips = new List<AudioClip>();
        this.mainAudioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        this.startingBallPosition = GameObject.Find("Ball").transform;
#if UNITY_EDITOR
        //this.BgLoader();
        this.BgAudioLoader();
        this.UserBgLoader();
        //this.UserBgAudioLoader();
#else
        this.UserBgLoader();
        this.BgAudioLoader();
#endif
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(this.currentGame.GetHumanPlayer().CurrentBall.ToString());
        if (Input.GetButtonUp("PauseGame"))
        {
            if (this.currentGame.GamePaused)
            {
                this.currentGame.ResumeGame();
            }
            else
            {
                this.currentGame.PauseGame();
            }
        }

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

        if (this.collisionQueue.Count > 0)
        {
            DelayedCollision dc = new DelayedCollision();
            dc = this.collisionQueue.Dequeue();
            GameObject.Find("Platform").GetComponent<PlatformMover>().LaunchBall(dc.cp, dc.platformFlag);
            this.collisionQueue.Clear();
        }

#if UNITY_EDITOR //debug level switching
        if (Input.GetKeyUp(KeyCode.E))
        {
            object sender = new object();
            ChangeLevelEventArgs e = new ChangeLevelEventArgs(this.currentGame.Level, this.currentGame.Level, ChangeLevelReasons.NextLevelKeyPressed);
            EventSystem.FireChangeLevel(sender, e);
        }
#endif
	}

    public void AddCollisionToQueue(DelayedCollision dc)
    {
        this.collisionQueue.Enqueue(dc);
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
            this.currentGame.PauseGame();
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
            this.currentGame.PauseGame();
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
        string userDataFolder = "UserData/Backgrounds/";

        try
        {
            UnityEngine.Object[] objs = Resources.LoadAll(@"bg_textures/");
            List<Texture2D> textures = new List<Texture2D>();
            foreach (UnityEngine.Object obj in objs)
            {
                textures.Add((Texture2D)obj);
            }

            if (!Directory.Exists(userDataFolder))
            {
                throw new Exception("No User Data directory found!");
            }

            string[] filesFound = Directory.GetFiles(userDataFolder);

            foreach (string str in filesFound)
            {
                if (this.CheckFileForBeingImage(str))
                {
                    Texture2D tmpTex = new Texture2D(2, 2);
                    byte[] readBytes = File.ReadAllBytes(str);
                    if (tmpTex.LoadImage(readBytes))
                    {
                        textures.Add(tmpTex);
                    }
                    else
                    {
                        throw new Exception("Error while loading BG!");
                    }
                }
            }

            Texture2D tex = textures[UnityEngine.Random.Range(0, textures.Count)];

            if (tex != null)
            {
                GameObject.Find("Plane").GetComponent<Renderer>().material.mainTexture = tex;
            }
            else
            {
                throw new Exception("Texture is null object");
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("Can't load bg textures");
            Debug.Log(e.Message);
            return;
#else
            this.currentGame.PauseGame();
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't load bg textures", e);
            EventSystem.FireInterfaceUpdate(this, ev);
#endif
        }
    }

    /*
     
     * finish later some problems with playback, probably i will have to write a plugin for unity in order to playback uadio files the right way
     
     */
    protected void UserBgAudioLoader()
    { 
        /*
         
         * TODO Same as above but with user music
         
         */
        string userDataFolder = "UserData/Music/";

        try
        {
            if (!Directory.Exists(userDataFolder))
            {
                throw new Exception("No User Data folder found!");
            }

            UnityEngine.Object[] objs = Resources.LoadAll(@"bg_audio/");

            foreach (UnityEngine.Object obj in objs)
            {
                this.clips.Add((AudioClip)obj);
            }

            float[] actualFloatData = new float[clips[clips.Count - 1].samples * clips[clips.Count - 1].channels];
            clips[clips.Count - 1].GetData(actualFloatData, 0);

            string[] filesFound = Directory.GetFiles(userDataFolder);

            foreach (string str in filesFound)
            {
                if (this.CheckFileForBeingAudio(str))
                {
                    Debug.LogWarning(str);
                    Debug.LogWarning(clips[clips.Count - 1].name);
                    Debug.LogWarning(actualFloatData[0] + " " + actualFloatData[1] + " " + actualFloatData[2] + " " + actualFloatData[3] + " " + actualFloatData[4] + " " + actualFloatData[5] + " " + actualFloatData[6] + " " + actualFloatData[7] + " " + actualFloatData[8] + " " + actualFloatData[9]);
                    byte[] readBytes = File.ReadAllBytes(str);
                    float[] f = this.ConvertByteToFloat(readBytes);
                    
                    Debug.LogWarning(f[0] + " " + f[1] + " " + f[2] + " " + f[3] + " " + f[4] + " " + f[5] + " " + f[6] + " " + f[7] + " " + f[8] + " " + f[9]);
                    AudioClip TmpClip = AudioClip.Create(clips.Count.ToString(), f.Length, 1, 44100, false, false);
                    TmpClip.SetData(f, 0);
                    this.clips.Add(TmpClip);
                    
                }
            }

            //this.totalClips = clips.Count;
            //AudioClip clip = clips[clips.Count - 1];

            //this.mainAudioSource.clip = clip;
            //this.mainAudioSource.Play();
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("Can't load audio clips");
            Debug.Log(e.Message);
            return;
#else
            this.currentGame.PauseGame();
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't load audio clips", e);
            EventSystem.FireInterfaceUpdate(this, ev);
#endif
        }
    }

    private float[] ConvertByteToFloat(byte[] byteArray)
    {
        float[] floatArray = new float[byteArray.Length / 4];

        for (int i = 0; i < floatArray.Length; i++)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteArray, i * 4, 4);

            floatArray[i] = BitConverter.ToSingle(byteArray, i * 4) / 0x80000000;
        }

        return floatArray;
    }

    private bool CheckFileForBeingImage(string path)
    {
        string[] pathParts = path.Split('.');

        if (pathParts[pathParts.Length - 1] == "jpg" || pathParts[pathParts.Length - 1] == "jpeg")
            return true;
        else if (pathParts[pathParts.Length - 1] == "png")
            return true;
        else if (pathParts[pathParts.Length - 1] == "gif")
            return true;

        return false;
    }

    private bool CheckFileForBeingAudio(string path)
    {
        string[] pathParts = path.Split('.');
        Debug.LogWarning(pathParts[pathParts.Length - 1]);
        if (pathParts[pathParts.Length - 1] == "mp3" || pathParts[pathParts.Length - 1] == "MP3")
            return true;
        else if (pathParts[pathParts.Length - 1] == "wav")
            return true;
        else if (pathParts[pathParts.Length - 1] == "ogg")
            return true;

        return false;
    }
}
