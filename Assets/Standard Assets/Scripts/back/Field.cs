using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum ControlSymbols
{
    _,
    X,
    Y,
    Z
};

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
        Logger.WriteToLog("Initial level num " + levelNum.ToString());

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

#if UNITY_EDITOR
        string tempPath = @"Temp";
#else
        string tempPath = @"Data/Temp";
#endif

        Logger.WriteToLog("Still here for file number " + levelNum.ToString());
        string decryptedPath = string.Empty;
        try
        {
            this.charTmpgrid = new char[this.sizeX, this.sizeY];
            int i = 0;

#if UNITY_EDITOR
            string finalPath = path + @"level" + levelNum.ToString() + ".txt";
#else
            string finalPath = path + @"level" + levelNum.ToString() + ".txt";
            decryptedPath = tempPath + @"/" + DateTime.Now.GetHashCode().ToString() + ".txt";
            LevelFileCrypto.DecryptFile(finalPath, decryptedPath, "");
            finalPath = decryptedPath;
            File.SetAttributes(decryptedPath, File.GetAttributes(decryptedPath) | FileAttributes.Hidden);
#endif

            using (StreamReader sr = new StreamReader(finalPath))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    int k = 0;
                    for (int j = 0; j < line.Length; j++)
                    {
                        if (this.IsControlSymbol(line[j]))
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
        finally
        {
            try
            {
                File.Delete(decryptedPath);
            }
            catch (Exception ex)
            { 
#if UNITY_EDITOR
                Debug.Log("Can't delete decrypted file!");
                Debug.Log(ex.Message);                
#else
                Logger.WriteToLog("Can't delete decrypted file! " + ex.Message);
#endif
            }
        }
    }

    private bool ReadFieldFromFile(int levelNum, bool bGameSessionFlag)
    {
        Debug.Log("Dafuq it's here?");
        Logger.WriteToLog("Reading level from file number " + levelNum.ToString());
#if UNITY_EDITOR
        string path = @"Temp";
#else
        string path = @"Data/Temp";
#endif       
        Logger.WriteToLog("From folder " + path);
        string decryptedFile = string.Empty;
        try
        {
            string levelPath = MainHelper.CurrentGameSession.CurrentLevels[levelNum].LevelPath;
            Logger.WriteToLog(levelPath);
            this.charTmpgrid = new char[this.sizeX, this.sizeY];
            int i = 0;
            decryptedFile = path + @"/" + DateTime.Now.GetHashCode().ToString() + @".txt";

            LevelFileCrypto.DecryptFile(levelPath, decryptedFile, "");
            File.SetAttributes(decryptedFile, File.GetAttributes(decryptedFile) | FileAttributes.Hidden);
            
            using (StreamReader sr = new StreamReader(decryptedFile))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    int k = 0;
                    for (int j = 0; j < line.Length; j++)
                    {
                        if (this.IsControlSymbol(line[j]))
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
                File.Delete(decryptedFile);
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
                    if (this.IsControlSymbol(this.charTmpgrid[i, j], true))
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
        }
    }

    public void ReInit()
    {
        Array.Clear(this.charTmpgrid, 0, this.charTmpgrid.Length);
        Array.Clear(this.grid, 0, this.grid.Length);
        this.totalBricks = 1;
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

    private bool IsControlSymbol(char symb, bool brickFlag = false)
    {
        string[] symbols = Enum.GetNames(typeof(ControlSymbols));

        if (brickFlag)
        {
            string[] tmpArr = new string[symbols.Length - 1];
            int i = 0;
            foreach (string s in symbols)
            {
                if (!s.Equals("_"))
                {
                    tmpArr[i] = s;
                    i++;
                }
            }

            symbols = tmpArr;
        }

        for (int i = 0; i < symbols.Length; i++)
        {
            symbols[i] = symbols[i].ToLower();
        }
        string tmp = symb.ToString().ToLower();
        bool ex = Array.Exists<string>(symbols, (x => x == tmp));

        return ex;
    }
}
