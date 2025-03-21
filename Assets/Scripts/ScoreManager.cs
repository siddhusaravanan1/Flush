using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    public GameManager gameManager;

    public Dictionary<string, int> levelID = new Dictionary<string, int>()
    {
        { "1", 5 },
        { "2", 6 },
        { "3", 7 },
        { "4", 8 }
    };

    public string selectedLevelID;
    void Start()
    {
        selectedLevelID = "1";
        gameManager.scoreRequired = levelID[selectedLevelID];
    }
    void Update()
    {

    }

}
