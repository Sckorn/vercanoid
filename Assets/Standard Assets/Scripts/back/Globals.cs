using UnityEngine;
using System.Collections;

public static class Globals {

    public static Options options = null;
    private static GameModes currentGameMode;

    public static GameModes CurrentGameMode
    {
        get { return Globals.currentGameMode; }
    }

    public static void SetGameMode(GameModes _mode)
    {
        Globals.currentGameMode = _mode;
    }

    public static void InitializeOptions()
    {
        Globals.options = new Options();
    }

    public static void WriteAndCloseOptions()
    {
        Globals.options.WriteOptions();
        Globals.options = null;
    }

    public static void CreateDefaultOptionsFile()
    { 
        
    }
}
