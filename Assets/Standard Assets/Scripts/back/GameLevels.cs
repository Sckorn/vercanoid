using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameLevels {
    private int totalLevels = 1;
    private GameLevel[] CurrentLevels = new GameLevel[1];

    public int TotalLevels
    {
        get { return this.totalLevels; }
    }

    public int ActualArrayLength
    {
        get { return this.CurrentLevels.Length; }
    }

    public string this[string param]
    {
        get { if (param.Equals("levelFiles")) return this.totalLevels.ToString(); return string.Empty; }
        set { if (param.Equals("levelFiles")) int.TryParse(value, out this.totalLevels); }
    }

    public GameLevel this[int param]
    {
        get 
        {
            if (param > this.CurrentLevels.Length || param < 0) return null;

            try
            {
                return this.CurrentLevels[param];
            }
            catch (Exception e)
            {
                Debug.Log("Fuck up");
                Debug.Log(e.Message);
                Debug.Log(param.ToString());
                Debug.Log(this.CurrentLevels.Length.ToString());
                return null;
            }
        }

        set
        {
            this.SetLevel(param, value);
        }
    }

    public void SetLevel(int index, GameLevel gl)
    {
        if (index == this.CurrentLevels.Length)
        {
            this.AddLevel(gl);
        }
        else
        {
            this.CurrentLevels[index] = gl;
        }
    }

    public void AddLevel(GameLevel gl)
    {        
        GameLevel[] tmpLev = new GameLevel[this.CurrentLevels.Length + 1];
        this.CurrentLevels.CopyTo(tmpLev, 0);
        tmpLev[tmpLev.Length - 1] = gl;
        this.CurrentLevels = tmpLev;
        this.totalLevels++;
    }

    public bool RemoveLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex > this.CurrentLevels.Length) return false;

        Debug.Log("From remove level, levelIndex: " + levelIndex.ToString());
        try
        {
            this.CurrentLevels[levelIndex].Dispose();
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.Log("Wrong index?");
            Debug.Log(e.Message);
        }

        for (int i = levelIndex; i < this.CurrentLevels.Length; i++)
        {
            try
            {
                this.CurrentLevels[i] = this.CurrentLevels[i + 1];
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log("Index out fo range");
                Debug.Log(e.Message);
            }
        }

        GameLevel[] TmpArr = new GameLevel[this.CurrentLevels.Length - 1];
        Array.Copy(this.CurrentLevels, 0, TmpArr, 0, TmpArr.Length);

        this.CurrentLevels = TmpArr;
        this.totalLevels--;

        return true;
    }
}

public class GameLevel : IDisposable {
    private int levelNumber = 0;
    private string levelPath;
    private bool levelFileEncrypted;
    private bool userLevelFlag;
    private bool disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        { 
            
        }

        this.levelPath = string.Empty;

        disposed = true;
    }

    ~GameLevel()
    {
        Dispose(false);
    }

    public string this[string param]
    {
        get
        {
            if (param.Equals("levelNumber"))
                return this.LevelNumber.ToString();

            if (param.Equals("userLevel"))
                return this.UserLevel.ToString();

            if (param.Equals("filePath"))
                return this.LevelPath;

            if (param.Equals("encrypted"))
                return this.LevelEncrypted.ToString();

            return string.Empty;
        }

        set
        {
            if (param.Equals("levelNumber"))
            {
                if (!int.TryParse(value, out this.levelNumber))
                {
                    throw new Exception("Wrong type for the specified field");
                }
            }

            if (param.Equals("userLevel"))
            {
                int test = -1;
                if (!int.TryParse(value, out test))
                {
                    throw new Exception("Wrong type for the specified field");
                }
                else
                {
                    if (test == 0) this.userLevelFlag = false;
                    else this.userLevelFlag = true;
                }
            }

            if (param.Equals("filePath"))
            {
                this.levelPath = value;
            }

            if (param.Equals("encrypted"))
            {
                int test = -1;
                if (!int.TryParse(value, out test))
                {
                    throw new Exception("Wrong type for the specified field");
                }
                else
                {
                    if (test == 0) this.levelFileEncrypted = false;
                    else this.levelFileEncrypted = true;
                }
            }
        }
    }

    public int LevelNumber
    {
        get { return this.levelNumber; }
        set { this.levelNumber = value; }
    }

    public string LevelPath
    {
        get { return this.levelPath; }
        set { this.levelPath = value; }
    }

    public bool LevelEncrypted
    {
        get { return this.levelFileEncrypted; }
        set { this.levelFileEncrypted = value; }
    }

    public bool UserLevel
    {
        get { return this.userLevelFlag; }
        set { this.userLevelFlag = value; }
    }

    public GameLevel(int _Num, string _Path, bool _Enc, bool _User)
    {
        this.levelNumber = _Num;
        this.levelFileEncrypted = _Enc;
        this.levelPath = _Path;
        this.userLevelFlag = _User;
    }

    public GameLevel()
    { 
        
    }
}
