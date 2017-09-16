﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour {
	public float levelStartDelay = 2f;
    public Dictionary<string, int> levelRequirements = new Dictionary<string, int>();

    private LevelLoader[] _levelLoaders;
    private ResetProgress _resetProgress;

	void Start() {
        _levelLoaders = FindObjectsOfType<LevelLoader>();
        _resetProgress = FindObjectOfType<ResetProgress>();
    
        LoadLevelRequirements();
        if (SceneManager.GetActiveScene().name == "InteractiveMainMenu")
            UpdateLevelLoaders();
    }

	public void RestartLevel() {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public void StopLevel() {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene ("InteractiveMainMenu");
	}

    public void PlayerInteract()
    {
        foreach(LevelLoader levelLoader in _levelLoaders)
        {
            levelLoader.PlayerInteract();
        }
        if(_resetProgress != null)
            _resetProgress.PlayerInteract();
    }

    private void LoadLevelRequirements()
    {
        using (StreamReader r = new StreamReader("Assets/Resources/LevelRequirements.json"))
        {
            string json = r.ReadToEnd();
            LevelInfo[] levelData = JsonHelper.FromJson<LevelInfo>(json);

            foreach (LevelInfo level in levelData)
                levelRequirements.Add(level.levelName, level.collectiblesRequired);
        }
    }

    public void UpdateLevelLoaders()
    {
        foreach (LevelLoader levelLoader in _levelLoaders)
        {
            int remaining = levelRequirements[levelLoader.levelToLoad] - PlayerPrefs.GetInt("totalCollectiblesGlobal");
            if (remaining > 0)
            {
                levelLoader.GetComponentInChildren<TextMesh>().text = remaining.ToString() + " more to go";
            }
            else
            {
                levelLoader.GetComponentInChildren<TextMesh>().text = "Interact";
            }
        }
    }
}

[System.Serializable]
public class LevelInfo
{
    public string levelName;
    public int collectiblesRequired;
}
