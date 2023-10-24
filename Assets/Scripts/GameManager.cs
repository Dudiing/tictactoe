using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isCubeTurn = true;
    public TextMeshProUGUI label;
    public Cell[] cells;
    public GameObject restartButton;
    public GameObject backToMenuButton;
    public AudioClip clipWin;
    public AudioClip clipDraw;
    public bool modeAI;
    private bool hasAIMoved = false;

    void Start()
    {
        ChangeTurn();
        restartButton.SetActive(false);
        backToMenuButton.SetActive(false);
        int flag = PlayerPrefs.GetInt("AI", 1);
        modeAI = flag == 1;
    }

    public void CheckWinner()
    {
        if (IsWinningMove())
        {
            DeclareWinner(isCubeTurn ? CellType.CUBE : CellType.SPHERE);
            return;
        }

        bool isDraw = true;
        foreach (var cell in cells)
        {
            if (cell.status == CellType.EMPTY)
            {
                isDraw = false;
                break;
            }
        }

        if (isDraw)
        {
            label.text = "It's a draw!";
            SetupGameFinished(false);
        }
    }

    public void ChangeTurn()
    {
        isCubeTurn = !isCubeTurn;
        label.text = modeAI ? (isCubeTurn ? "Player turn" : "AI turn") : (isCubeTurn ? "Cube's turn..." : "Sphere's turn...");
    }

    void DeclareWinner(CellType status)
    {
        label.text = modeAI ? (status == CellType.SPHERE ? "AI wins" : "Player wins") : (status == CellType.SPHERE ? "Sphere is the winner" : "Cube is the winner");
        SetupGameFinished(true);
    }

    void Update()
    {
        if (modeAI && !isCubeTurn && !hasAIMoved)
        {
            hasAIMoved = true;
            StartCoroutine(MakeAIMove());
        }
    }

    IEnumerator MakeAIMove()
    {
        float waitTime = Random.Range(1.0f, 3.0f);
        yield return new WaitForSeconds(waitTime);

        int move = GetBestMoveForAI();
        if (move != -1)
        {
            cells[move].onClick();
            hasAIMoved = false;
        }
    }

    int GetBestMoveForAI()
    {
        int move = FindBestMove(CellType.CUBE); // Intenta ganar
        if (move != -1) return move;

        move = FindBestMove(CellType.SPHERE); // Intenta bloquear
        if (move != -1) return move;

        int[] priorities = { 4, 0, 2, 6, 8, 1, 3, 5, 7 };
        foreach (int i in priorities)
        {
            if (cells[i].status == CellType.EMPTY)
            {
                return i;
            }
        }
        return -1;
    }

    int FindBestMove(CellType type)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].status == CellType.EMPTY)
            {
                cells[i].status = type;
                if (IsWinningMove())
                {
                    cells[i].status = CellType.EMPTY;
                    return i;
                }
                cells[i].status = CellType.EMPTY;
            }
        }
        return -1;
    }

    bool IsWinningMove()
    {
        // Revisa filas, columnas y diagonales
        int[][] winConditions = {
            new int[] {0, 1, 2}, new int[] {3, 4, 5}, new int[] {6, 7, 8}, // Filas
            new int[] {0, 3, 6}, new int[] {1, 4, 7}, new int[] {2, 5, 8}, // Columnas
            new int[] {0, 4, 8}, new int[] {2, 4, 6} // Diagonales
        };

        foreach (var condition in winConditions)
        {
            if (cells[condition[0]].status != CellType.EMPTY && 
                cells[condition[0]].status == cells[condition[1]].status && 
                cells[condition[1]].status == cells[condition[2]].status)
            {
                return true;
            }
        }
        return false;
    }

    void SetupGameFinished(bool hasWinner)
    {
        restartButton.SetActive(true);
        backToMenuButton.SetActive(true);
        // Sonidos por implementar aquí.
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
