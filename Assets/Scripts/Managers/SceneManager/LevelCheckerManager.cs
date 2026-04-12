using UnityEngine;

public static class LevelCheckerManager 
{
    
    public static void MarkLevelAsCompleted(string levelName)
    {
        // Guardamos un 1 (true) en el registro de Windows/Mac/Móvil
        PlayerPrefs.SetInt("Level_" + levelName + "_Completed", 1);
        PlayerPrefs.Save();
        Debug.Log($"<color=green>Progreso guardado: {levelName} completado.</color>");
    }

    // Consulta si un nivel está desbloqueado
    public static bool IsLevelUnlocked(string levelName)
    {
        // El Nivel 1 siempre debería estar desbloqueado por defecto
        if (levelName == "A Wonderfull life") return true; 

        return PlayerPrefs.GetInt("Level_" + levelName + "_Completed", 0) == 1;
    }
    
    
    
    
}