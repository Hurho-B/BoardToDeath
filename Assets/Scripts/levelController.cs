using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class levelController : MonoBehaviour
{
    [Header("Objective Settings")]
    [Tooltip("The score needed to finish the level.")]
    public int targetScore;
    [Tooltip("Debug: Amount of time before a combo finishes.")]
    public int comboTime;
    [Tooltip("DEBUG: The current score of the player.\nDefault = 0")]
    public int playerScore;
    [Tooltip("How much time will the level have, in seconds.")]
    public float timer = 60.0f;

    [Header("Generator Settings")]
    [Tooltip("The size of the grid.")]
    public Vector2 size;
    [Tooltip("The distance between the center of each room.")]
    public int offset;
    [Tooltip("The prefab used for the generator.")]
    public GameObject room;

    [Header("UI Elements")]
    [Tooltip("Text element to display time left on the timer.")]
    public TMP_Text timeUI;
    [Tooltip("Text element to display score.")]
    public TMP_Text scoreUI;

    float comboTimeLeft = 0;
    bool calculatingScore = false;
    List<int> scores;
    int sumScore = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelGenerator();
    }

    void Update()
    {
        if (comboTimeLeft > 0.0 && calculatingScore)
        { comboTimeLeft -= Time.deltaTime; }
        else if (calculatingScore)
        {
            calculatingScore = false;

            foreach (int score in scores)
            { sumScore += score; }
            playerScore += sumScore * scores.Count;
            scores = new List<int>();
        }
    }

    void CalculateScore(int addedScore)
    {
        scores.Add(addedScore);
        comboTimeLeft = comboTime;
    }

    void LevelGenerator()
    {

    }

}
