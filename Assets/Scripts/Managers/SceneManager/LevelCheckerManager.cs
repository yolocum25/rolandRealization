using UnityEngine;

public static class LevelCheckerManager
{
    // Guarda que un nivel específico ha sido superado
    public static void MarkLevelAsCompleted(string levelName)
    {

        string key = "Level_" + levelName.Trim() + "_Completed";
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        Debug.Log($"<color=green>Progreso guardado: {levelName} ahora consta como completado.</color>");
    }

    // Esta es la función que deben usar tus botones del menú
    public static bool IsLevelUnlocked(string levelName) // <--- Solo un parámetro
    {
        if (levelName == "A Wonderfull life") return true;
        return PlayerPrefs.GetInt("Level_" + levelName.Trim() + "_Completed", 0) == 1;
    }
}
    
    
    
