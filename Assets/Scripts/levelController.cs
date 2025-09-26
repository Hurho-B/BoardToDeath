using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class levelController : MonoBehaviour
{
    [Header("Objective Settings")]
    [Tooltip("The score needed to finish the level.")]
    public long targetScore;
    [Tooltip("DEBUG: The current score of the player.\nDefault = 0")]
    public long playerScore;
    [Tooltip("Debug: Amount of time before a combo finishes.")]
    public int comboTime;
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
    [Tooltip("Text element to display # of tricks done in a combo")]
    public TMP_Text comboUI;
    [Tooltip("Text element to display the score that will be earned after a combo")]
    public TMP_Text sumScoreUI;

    float comboTimeLeft = 0;
    bool calculatingScore = false;
    List<int> scores = new List<int>();
    int sumScore = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelGenerator();
    }

    void FixedUpdate()
    {
        if (timer > 0)
        { timer -= Time.deltaTime; }
        else if (timer < 0) 
        { 
            timer = 0;
            timeUI.color = Color.red;
        }
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timeUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void Update()
    {
        int tempScore = 50;
        // KeyBind calling CalculateScore
        if (Input.GetKeyDown(KeyCode.R))
        { CalculateScore(tempScore); }
        if (comboTimeLeft > 0.0 && calculatingScore)
        { comboTimeLeft -= Time.deltaTime; }
        else if (calculatingScore)
        {
            calculatingScore = false;
            foreach (int score in scores)
            { sumScore += score; }

            sumScore *= scores.Count;
            playerScore += sumScore;

            sumScore = 0;
            scores.Clear();
        }
        scoreUI.text = "Score:\n" + playerScore.ToString();
    }

    void CalculateScore(int addedScore)
    {
        scores.Add(addedScore);
        comboTimeLeft = comboTime;
        calculatingScore = true;
        print(scores.Count);
    }

    void LevelGenerator()
    {

    }

}
