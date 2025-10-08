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
    public float timer = 60.0f;

    [Header("Generator Settings")]
    [Tooltip("The size of the grid.")]
    public Vector2 size;
    [Tooltip("The distance between the center of each module.")]
    public int offset;
    [Tooltip("The prefabs used for the generator.")]
    public List<GameObject> module;
    [Tooltip("Temp var, which cell to start generator at.")]
    public int startPos = 0;

    [Header("UI Elements")]
    [Tooltip("Text element to display time left on the timer.")]
    public TMP_Text timeUI;
    [Tooltip("Text element to display score.")]
    public TMP_Text scoreUI;
    [Tooltip("Text element to display # of tricks done in a combo")]
    public TMP_Text comboUI;
    [Tooltip("Text element to display the score that will be earned after a combo")]
    public TMP_Text sumScoreUI;
    [Tooltip("Menu to look at Boons.")]
    public GameObject boonOptions;

    float comboTimeLeft = 0;
    bool calculatingScore = false;
    List<int> scores = new List<int>();
    int sumScore = 0;
    string currentSceneName;

    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }
    List<Cell> board;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelGenerator();
        boonOptions.SetActive(false);
        currentSceneName = SceneManager.GetActiveScene().name;
        print(module.Count);
    }

    void FixedUpdate()
    {
        if (timer > 0)
        { timer -= Time.deltaTime; }
        else if (timer < 0)
        {
            timer = 0;
            timeUI.color = Color.red;
            SceneManager.LoadScene(currentSceneName);
        }
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timeUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void Update()
    {
        bool isBoonMenuActive = boonOptions.activeSelf;
        // BoonMenu(isBoonMenuActive);
        int tempScore = 50;

        // KeyBind calling CalculateScore
        if (Input.GetKeyDown(KeyCode.R))
        { CalculateScore(tempScore); }

        // This math will not run until no more scores
        // are being added to the combo
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

        // Deletes the existing list + gameObjects
        // Generates a new dungeon
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

        // Enables Boon Menu
        // if (Input.GetKeyDown(KeyCode.M))
        // { boonOptions.SetActive(!isBoonMenuActive); }
    }

    void BoonMenu(bool active)
    {
        if (!active)
            return;

        // Ignore function, to be deleted

        if (Input.GetKeyDown(KeyCode.Keypad1))
        { }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        { }
    }

    void CalculateScore(int addedScore)
    {
        scores.Add(addedScore);
        comboTimeLeft = comboTime;
        calculatingScore = true;
        print(scores.Count);
    }

    void GenerateLevel()
    {
        // After LevelGenerator(), new list is brought in
        // and used to make the actual environment/gameObjects
        // for (int i = 0; i < size.x; i++)
        // {
        //     for (int j = 0; j < size.y; j++)
        //     {
        //         Cell currentCell = board[Mathf.FloorToInt(i + j * size.x)];
        //         if (currentCell.visited)
        //         {
        //             RoomBehaviour newRoom = Instantiate(room, new Vector3(i * offset, 0, -j * offset), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
        //             newRoom.UpdateRoom(currentCell.status);

        //             newRoom.name += " " + i + "-" + j;
        //         }
        //     }

        // }
    }

    void LevelGenerator()
    {
        // Generating the workable area
        board = new List<Cell>();
        for (int i = 0; i < (size.x * size.y); i++)
        { board.Add(new Cell()); }

        for (int i = 0; i < size.x; i++)
        {

            int currentCell = startPos;
            Stack<int> path = new Stack<int>();
            int k = 0;

            while (k < 1000)
            {
                k++;

                // Marking current cell, checking adjacent cells if
                // they've already been visited
                // This type of check means no 3 or 4 way junctions
                board[currentCell].visited = true;
                List<int> neighbors = CheckNeighbors(currentCell);

                if (currentCell == board.Count - 1)
                {
                    break;
                }


                if (neighbors.Count == 0)
                {
                    if (path.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        currentCell = path.Pop();
                    }
                }
                else
                {
                    path.Push(currentCell);
                    int newCell = neighbors[Random.Range(0, neighbors.Count)];

                    // This amalgamation marks which doors and walls
                    // end up being toggled
                    if (newCell > currentCell)
                    {
                        if (newCell - 1 == currentCell)
                        {
                            board[currentCell].status[1] = true;
                            currentCell = newCell;
                            board[currentCell].status[3] = true;
                        }
                        else
                        {
                            board[currentCell].status[2] = true;
                            currentCell = newCell;
                            board[currentCell].status[0] = true;
                        }
                    }
                    else
                    {
                        if (newCell + 1 == currentCell)
                        {
                            board[currentCell].status[3] = true;
                            currentCell = newCell;
                            board[currentCell].status[1] = true;
                        }
                        else
                        {
                            board[currentCell].status[0] = true;
                            currentCell = newCell;
                            board[currentCell].status[2] = true;
                        }
                    }
                }
            }
        }
        GenerateLevel();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();
        // Checks North neighbor
        if (cell - size.x >= 0 && !board[Mathf.FloorToInt(cell - size.x)].visited)
        { neighbors.Add(Mathf.FloorToInt(cell - size.x)); }
        // Checks South neighbor
        if (cell + size.x < board.Count && !board[Mathf.FloorToInt(cell + size.x)].visited)
        { neighbors.Add(Mathf.FloorToInt(cell + size.x)); }
        // Checks East neighbor
        if ((cell + 1) % size.y != 0 && !board[Mathf.FloorToInt(cell + 1)].visited)
        { neighbors.Add(Mathf.FloorToInt(cell + 1)); }
        // Checks West neighbor
        if (cell % size.y != 0 && !board[Mathf.FloorToInt(cell - 1)].visited)
        { neighbors.Add(Mathf.FloorToInt(cell - 1)); }
        // Returns a list of valid neighbors (board indexes) to steer into
        return neighbors;
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
