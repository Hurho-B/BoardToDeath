using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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
    public float levelTime;

    [Header("Generator Settings")]
    [Tooltip("Manually assign a size for the level.\nDefault will generate a square shape.")]
    public Vector2 size;
    [Tooltip("The distance between the center of each module.")]
    public int offset;
    [Tooltip("The modules used for the generator.")]
    public List<GameObject> modules;

    [Header("UI Elements")]
    [Tooltip("Text element to display time left on the timer.")]
    public TMP_Text timeUI;
    [Tooltip("Text element to display score.")]
    public TMP_Text scoreUI;
    [Tooltip("Text element to display # of tricks done and banked score.")]
    public TMP_Text comboUI;
    [Tooltip("Text element to display the score that will be earned after a combo")]
    public TMP_Text tricksDoneUI;

    // Additional combo logic
    bool calculatingScore = false;
    float comboTimeLeft = 0;
    int numOfTricks = 0;
    int sumOfTricks = 0;
    // Additional combo logic

    // Failsafe logic
    bool isHUDPresent = true;
    bool isPlayerPresent = true;

    string currentSceneName;

    public class Cell
    {
        public int module;
        public bool[] status = new bool[4];
    }
    List<Cell> board;


    // Awake is handling failsafes, make sure that in the absense of an interacting Game Object
    // that other elements can still be tested. Please leave this here :)
    void Awake()
    {
        if (GameObject.Find("HUD") == null)
        {
            isHUDPresent = false;
            Debug.LogWarning("HUD not present in scene, disabling HUD interactivity.");
        }
        if (GameObject.Find("PlayerCharacter") == null)
        {
            isPlayerPresent = false;
            Debug.LogWarning("PlayerCharacter not present in scene, disabling PlayerCharacter interactivity.");
        }
        if (modules.Count < (size.x * size.y))
        {
            Debug.Log("Not enough modules to handle given size, auto-generating new size.");
            size.x = 1;
            size.y = 1;
            while (modules.Count > (size.x * size.y))
            {
                size.x += 1;
                while (modules.Count > (size.x * size.y))
                    size.y += 1;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelGenerator();
        currentSceneName = SceneManager.GetActiveScene().name;
        

        // If no size is properly defined, generator
        // will attempt to default to a square

    }

    void FixedUpdate()
    {
        if (levelTime > 0)
        { levelTime -= Time.deltaTime; }
        else if (levelTime < 0)
        {
            levelTime = 0;
            timeUI.color = Color.red;
            if (playerScore < targetScore)
                SceneManager.LoadScene(currentSceneName);
        }
    }

    void Update()
    {
        // KeyBind calling CalculateScore
        if (Input.GetKeyDown(KeyCode.R))
        {
            CalculateScore("Ollie", 50);
            // CalculateScore("Manual", 20);
            // CalculateScore("Kickflip", 20);
            // CalculateScore("Rail Grind", 10);
        }

        // This math will not run until no more scores
        // are being added to the combo
        if (comboTimeLeft > 0.0 && calculatingScore)
        {
            comboTimeLeft -= Time.deltaTime;
        }
        else if (calculatingScore)
        {
            calculatingScore = false;
            playerScore += numOfTricks * sumOfTricks;
            numOfTricks = 0;
            sumOfTricks = 0;
            comboUI.text = "";
            tricksDoneUI.text = "";
        }

        int minutes = Mathf.FloorToInt(levelTime / 60);
        int seconds = Mathf.FloorToInt(levelTime % 60);

        // Update UI elements
        if (minutes > 0)
            timeUI.text = string.Format("{0}:{1:00}", minutes, seconds);
        else
            timeUI.text = string.Format("{0}", seconds);
        if (numOfTricks > 0)
            comboUI.text = string.Format("{0} x {1}", numOfTricks, sumOfTricks);
        else
            comboUI.text = "";
        scoreUI.text = "Score:\n" + playerScore.ToString();


        // Deletes the existing list + gameObjects
        // Generates a new dungeon, comment out for final build.
        if (Input.GetKeyDown(KeyCode.T))
        {
            board.Clear();
            for (int i = (transform.childCount - 1); i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                Destroy(child.gameObject);
            }
            LevelGenerator();
        }
    }

    // Updates appropriate values relating to tricks 
    // being done and score being earned.
    void CalculateScore(string trickType, int addedScore)
    {
        // Look at moving UI if/else later
        if (tricksDoneUI.text == "")
            tricksDoneUI.text = trickType + " ";
        else
            tricksDoneUI.text += "+ " + trickType + " ";
        numOfTricks += 1;
        comboTimeLeft = comboTime;
        calculatingScore = true;
        if (trickType != "Manual" && trickType != "Rail Grind")
        {
            sumOfTricks += addedScore;
            return;
        }
        float comboTimePast = 0f;
        // while (isTricking)
        //     comboTimePast += Time.deltaTime;
        //     comboTimeLeft = comboTime;
        // sumOfTricks += addedScore * Mathf.FloorToInt(comboTimePast);
    }

    void GenerateLevel()
    {
        // After LevelGenerator(), new list is brought in
        // and used to make the actual environment/gameObjects
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currCell = board[Mathf.FloorToInt(i + j * size.x)];
                CellBehaviour newCell = Instantiate(modules[currCell.module], new Vector3(i * offset, 0, -j * offset), Quaternion.identity, transform).GetComponent<CellBehaviour>();
                newCell.UpdateRoom(currCell.status);

                newCell.name += " " + i + "-" + j;
            }

        }
    }

    void LevelGenerator()
    {
        // Size values must be assigned a value and be able to encompass
        // all given modules.
        board = new List<Cell>();
        for (int i = 0; i < (size.x * size.y); i++)
        { board.Add(new Cell()); }

        // Mapping each module to a cell in a randomized manner
        List<int> mapping = new List<int>();
        int modVal = Random.Range(0, modules.Count);
        for (int i = 0; i < (size.x * size.y); i++)
        {
            while (mapping.Contains(modVal))
            { modVal = Random.Range(0, modules.Count); }
            board[i].module = modVal;
            board[i].status = IsBorderingEdges(i);
            mapping.Add(modVal);
        }
        GenerateLevel();
    }

    bool[] IsBorderingEdges(int cell)
    {
        bool[] isTouchingEdge = {false, false, false, false};
        // Checks North neighbor
        if (cell - size.x < 0) 
        { isTouchingEdge[0] = true; }
        // Checks East neighbor
        if ((cell + 1) % size.y == 0)
        { isTouchingEdge[1] = true; }
        // Checks South neighbor
        if (cell + size.x > board.Count)
        { isTouchingEdge[2] = true; }
        // Checks West neighbor
        if (cell % size.y == 0)
        { isTouchingEdge[3] = true; }
        return isTouchingEdge;
    }

}
