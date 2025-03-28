using UnityEngine;

public class GameStateController : MonoBehaviour
{
    [SerializeField] private GameField gameField;              
    [SerializeField] private EndGamePanelController endGamePanel;
    [SerializeField] private InputController inputController;

    public int winThreshold = 2048;

    private void Start()
    {
        if(endGamePanel != null)
            endGamePanel.gameObject.SetActive(false);
    }

    public void CheckGameState()
    {
        bool win = false;
        for (int x = 0; x < gameField.fieldSize.x; x++)
        {
            for (int y = 0; y < gameField.fieldSize.y; y++)
            {
                var cv = gameField.Field[x, y];
                if (cv != null)
                {
                    int tileValue = (int)Mathf.Pow(2, cv.Cell.Value);
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
            if (endGamePanel)
                endGamePanel.ShowEndGame("You Win!");
            if (gameField) gameField.enabled = false;
            if (inputController) inputController.enabled = false;
            return;
        }

        if (NoMovesAvailable())
        {
            if (endGamePanel)
                endGamePanel.ShowEndGame("Game Over");
            if (gameField) gameField.enabled = false;
            if (inputController) inputController.enabled = false;
        }
    }

    private bool NoMovesAvailable()
    {
        // Если есть пустая клетка => false
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
                    if (gameField.Field[x, y] != null &&
                        gameField.Field[x + 1, y] != null &&
                        gameField.Field[x, y].Cell.Value == gameField.Field[x + 1, y].Cell.Value)
                        return false;
                }
                if (y < gameField.fieldSize.y - 1)
                {
                    if (gameField.Field[x, y] != null &&
                        gameField.Field[x, y + 1] != null &&
                        gameField.Field[x, y].Cell.Value == gameField.Field[x, y + 1].Cell.Value)
                        return false;
                }
            }
        }
        return true;
    }
}
