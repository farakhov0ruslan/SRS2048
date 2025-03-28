using UnityEngine;

public class GameStateController : MonoBehaviour
{
    [SerializeField] private GameField gameField;              // Ссылка на объект GameField
    [SerializeField] private EndGamePanelController endGamePanel; // Ссылка на EndGamePanelController
    [SerializeField] private InputController inputController; // Ссылка на EndGamePanelController

    // Порог победы, например 2048
    public int winThreshold = 2048;

    private void Start()
    {
        if(endGamePanel != null)
            endGamePanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Проверяет состояние игры: победа или отсутствие доступных ходов.
    /// </summary>
    public void CheckGameState()
    {
        bool win = false;
        for (int x = 0; x < gameField.fieldSize.x; x++)
        {
            for (int y = 0; y < gameField.fieldSize.y; y++)
            {
                var cellView = gameField.Field[x, y];
                if (cellView != null)
                {
                    int tileValue = (int)Mathf.Pow(2, cellView.Cell.Value);
                    if (tileValue >= winThreshold)
                    {
                        win = true;
                        break;
                    }
                }
            }
            if (win) break;
        }

        if (win)
        {
            endGamePanel.ShowEndGame("You Win!");
            gameField.enabled = false;
            inputController.enabled = false;
            return;
        }

        if (NoMovesAvailable())
        {
            endGamePanel.ShowEndGame("Game Over");
            gameField.enabled = false;
            inputController.enabled = false;
        }
    }

    /// <summary>
    /// Возвращает true, если нет свободных ячеек и ни у какой плитки нет возможного объединения с соседней.
    /// </summary>
    private bool NoMovesAvailable()
    {
        // Если есть пустая клетка, ходы возможны
        for (int x = 0; x < gameField.fieldSize.x; x++)
        {
            for (int y = 0; y < gameField.fieldSize.y; y++)
            {
                if (gameField.Field[x, y] == null)
                    return false;
            }
        }
        // Проверяем возможность объединения по горизонтали и вертикали
        for (int x = 0; x < gameField.fieldSize.x; x++)
        {
            for (int y = 0; y < gameField.fieldSize.y; y++)
            {
                if (x < gameField.fieldSize.x - 1)
                {
                    if (gameField.Field[x, y] != null && gameField.Field[x + 1, y] != null &&
                        gameField.Field[x, y].Cell.Value == gameField.Field[x + 1, y].Cell.Value)
                        return false;
                }
                if (y < gameField.fieldSize.y - 1)
                {
                    if (gameField.Field[x, y] != null && gameField.Field[x, y + 1] != null &&
                        gameField.Field[x, y].Cell.Value == gameField.Field[x, y + 1].Cell.Value)
                        return false;
                }
            }
        }
        return true;
    }
}
