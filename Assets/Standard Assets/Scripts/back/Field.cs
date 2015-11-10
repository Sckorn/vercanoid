using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Field {
    private int sizeX = 17;
    private int sizeY = 17;
    private GridCell[, ] grid;
    private char[,] charTmpgrid;
    private int totalBricks = 1;

    public int TotalBricks
    {
        get { return this.totalBricks; }
        set { this.totalBricks = value; }
    }

    public int SizeX
    {
        get { return this.sizeX; }
        set { this.sizeX = value; }
    }

    public int SizeY
    {
        get { return this.sizeY; }
        set { this.sizeY = value; }
    }

    public Field()
    {
        this.ConstructGrid();
    }

    public Field(int _x, int _y)
    {
        this.sizeX = _x;
        this.sizeY = _y;
        this.ConstructGrid();
    }

    public Field(int level = 0)
    {
        this.ConstructGrid(level);
    }

    private bool ReadFieldFromFile(int levelNum = 0)
    {
        if (MainHelper.CurrentGameSession != null)
        {
            if(MainHelper.CurrentGameSession.CurrentLevels[levelNum] != null)
                return this.ReadFieldFromFile(levelNum, true);
        }
#if UNITY_EDITOR
        string path = @"Assets/Standard Assets/Resources/";
#else
        string path = @"Data/Levels/";
#endif

        try
        {
            this.charTmpgrid = new char[this.sizeX, this.sizeY];
            int i = 0;
            using (StreamReader sr = new StreamReader(path + @"Level" + levelNum.ToString() + ".txt"))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    int k = 0;
                    for (int j = 0; j < line.Length; j++)
                    {
                        if (line[j] == '_' || line[j] == 'x')
                        {
                            this.charTmpgrid[k, i] = line[j];
                            k++;
                        }
                    }
                    i++;
                }
            }

            return true;
        }
        catch (IOException e)
        {
#if UNITY_EDITOR
            Debug.LogError("Can't read from file!");
            Debug.LogError(e.Message);
            return false;
#else
            GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().PauseGame();
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't read from file!", e);
            EventSystem.FireInterfaceUpdate(this, ev);
            return false;
#endif
        }
    }

    private bool ReadFieldFromFile(int levelNum, bool bGameSessionFlag)
    {
#if UNITY_EDITOR
        string path = @"Temp";
#else
        string path = @"Data/Temp";
#endif       
        string decryptedFile = string.Empty;
        try
        {
            string levelPath = MainHelper.CurrentGameSession.CurrentLevels[levelNum].LevelPath;
            Debug.LogWarning(levelPath);
            this.charTmpgrid = new char[this.sizeX, this.sizeY];
            int i = 0;
            decryptedFile = path + @"/" + DateTime.Now.GetHashCode().ToString() + @".txt";

            LevelFileCrypto.DecryptFile(levelPath, decryptedFile, "");

            using (StreamReader sr = new StreamReader(decryptedFile))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    int k = 0;
                    for (int j = 0; j < line.Length; j++)
                    {
                        if (line[j] == '_' || line[j] == 'x')
                        {
                            this.charTmpgrid[k, i] = line[j];
                            k++;
                        }
                    }
                    i++;
                }
            }

            return true;
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError("Can't read from file!");
            Debug.LogError(e.Message);
            return false;
#else
            GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().PauseGame();
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't read from file!", e);
            EventSystem.FireInterfaceUpdate(this, ev);
            return false;
#endif
        }
        finally
        {
            try
            {
                int i = 0;
                //File.Delete(decryptedFile);
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR
                Debug.LogError("Can't delete file!");
                Debug.LogError(ex.Message);
#else
                GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().PauseGame();
                InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Can't read from file!", ex);
                EventSystem.FireInterfaceUpdate(this, ev);
#endif
            }
        }
    }

    private bool ConstructGrid(int forLevel = 0)
    {
        try
        {
            this.grid = new GridCell[this.sizeX, this.sizeY];

            if(MainHelper.CurrentGameSession != null)
                this.ReadFieldFromFile(forLevel, true);
            else
                this.ReadFieldFromFile(forLevel);
            for (int i = 0; i < this.sizeX; i++)
            {
                for (int j = 0; j < this.sizeY; j++)
                {
                    if (this.charTmpgrid[i, j] == 'x')
                    {
                        this.grid[i, j] = new GridCell(i, j, true, this.charTmpgrid[i, j]);
                        this.totalBricks++;
                    }
                    else
                    {
                        this.grid[i, j] = new GridCell(i, j, false);
                    }
                }
            }

            return true;
        }
        catch (UnityException e)
        {
#if UNITY_EDITOR
            Debug.LogError("Error Constructing Field");
            Debug.LogError(e.Message);
            return false;
#else
            GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().PauseGame();
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "Error Constructing Field", e);
            EventSystem.FireInterfaceUpdate(this, ev);
            return false;
#endif
        }
    }

    public void BrickDestroyed(GridCellCoords c)
    {
        try
        {
            this.grid[c.x, c.y].HasBrick = false;
        }
        catch (KeyNotFoundException e)
        {
#if UNITY_EDITOR
            Debug.LogError("No such index.");
            Debug.LogError(e.Message);
            return;
#else
            GameObject.Find("MainHelper").GetComponent<MainHelper>().GetCurrentGame().PauseGame();
            InterfaceUpdateEventArgs ev = new InterfaceUpdateEventArgs(InterfaceUpdateReasons.ExceptionThrown, "No such index.", e);
            EventSystem.FireInterfaceUpdate(this, ev);
#endif
        }
        finally
        {
            --this.totalBricks;
            Debug.LogWarning(this.totalBricks.ToString());
        }
    }

    public void DestroyAllBricks()
    {
        Time.timeScale = 0;

        for (int i = 0; i < this.sizeX; i++)
        {
            for (int j = 0; j < this.sizeY; j++)
            {
                if (this.grid[i, j].HasBrick)
                {
                    GameObject.Find("Morpher").GetComponent<Morpher>().DestroyBrick(this.grid[i, j].ObjectReference);
                }
            }
        }

        Time.timeScale = 1;
    }
}
