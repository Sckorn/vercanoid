using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameLevels {
    private int totalLevels = 0;
    private GameLevel[] CurrentLevels = new GameLevel[1];

    public int TotalLevels
    {
        get { return this.totalLevels; }
    }

    public GameLevel this[int param]
    {
        get 
        {
            if (param > this.CurrentLevels.Length || param < 0) return null;

            return this.CurrentLevels[param];
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
}

public sealed class GameLevel {
    private int levelNumber = 0;
    private string levelPath;
    private bool levelFileEncrypted;
    private bool userLevelFlag;

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
                if (!bool.TryParse(value, out this.userLevelFlag))
                {
                    throw new Exception("Wrong type for the specified field");
                }
            }

            if (param.Equals("filePath"))
            {
                this.levelPath = value;
            }

            if (param.Equals("encrypted"))
            {
                if (!bool.TryParse(value, out this.levelFileEncrypted))
                {
                    throw new Exception("Wrong type for the specified field");
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
