using UnityEngine;

public static class LevelCheckerManager 
{
    
    public static void MarkLevelAsCompleted(string levelName)
    {
        
        PlayerPrefs.SetInt("Level_" + levelName + "_Completed", 1);
        PlayerPrefs.Save();
        Debug.Log($"<color=green>Progreso guardado: {levelName} completado.</color>");
    }

    
    public static bool IsLevelUnlocked(string levelName)
    {
        
        if (levelName == "A Wonderfull life") return true; 

        return PlayerPrefs.GetInt("Level_" + levelName + "_Completed", 0) == 1;
        
    }
    

    
    
    
    
    
    
}