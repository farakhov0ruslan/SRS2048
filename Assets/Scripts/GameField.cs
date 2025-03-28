using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameField : MonoBehaviour
{
    public Vector2Int fieldSize = new Vector2Int(4, 4);
    public GameObject cellViewPrefab; 
    public List<Cell> cells = new List<Cell>();
    public CellView[,] Field;
    public int score = 0;

    private Grid _grid;
    public ScoreController scoreController;
    public GameStateController gameStateController;

    public void Awake()
    {
        _grid = GetComponentInChildren<Grid>();
        Field = new CellView[fieldSize.x, fieldSize.y];
    }

    public void Start()
    {
        // Создаем начальные плитки
        CreateCell();
        CreateCell();
    }

    public Vector2Int GetEmptyPosition()
    {
        var freePositions = new List<Vector2Int>();
        for (int y = 0; y < fieldSize.y; y++)
        {
            for (int x = 0; x < fieldSize.x; x++)
            {
                if (Field[x, y] == null)
                    freePositions.Add(new Vector2Int(x, y));
            }
        }
        if (freePositions.Count == 0)
        {
            Debug.LogWarning("Нет свободных позиций!");
            return new Vector2Int(-1, -1);
        }
        return freePositions[Random.Range(0, freePositions.Count)];
    }

    public void CreateCell()
    {
        Vector2Int pos = GetEmptyPosition();
        if (pos.x < 0) 
            return; // нет свободных позиций

       
        // Создаём данные и добавляем в список
        int newValue = (Random.value <= 0.2f) ? 2 : 1;
        Cell newCell = new Cell(pos, newValue);
        cells.Add(newCell);

        // Создаём визуальную часть
        Transform cellPlaceTransform = _grid.CellsTransforms[pos];
        GameObject cellViewObject = Instantiate(cellViewPrefab, cellPlaceTransform);
        CellView cellView = cellViewObject.GetComponent<CellView>();
        cellView.Init(newCell, _grid);
        Field[pos.x, pos.y] = cellView;
    }

    public virtual void ProcessInput(Vector2 inputDirection)
    {
        // Преобразуем направление: Unity vs наша логика (y меняется знаком)
        Vector2 gridDirection = new Vector2(inputDirection.x, -inputDirection.y);

        bool moved = MoveTiles(gridDirection);
        if (moved)
        {
            CreateCell();
            UpdateScore();
        }
        if (scoreController != null)
            scoreController.UpdateScore(score);
        if (gameStateController != null)
            gameStateController.CheckGameState();
    }

    private bool MoveTiles(Vector2 gridDirection)
    {
        bool moved = false;
        bool[,] merged = new bool[fieldSize.x, fieldSize.y];

        if (gridDirection.x < 0) // движение влево
        {
            for (int y = 0; y < fieldSize.y; y++)
            {
                for (int x = 1; x < fieldSize.x; x++)
                {
                    moved |= ShiftLeft(x, y, merged);
                }
            }
        }
        else if (gridDirection.x > 0) // вправо
        {
            for (int y = 0; y < fieldSize.y; y++)
            {
                for (int x = fieldSize.x - 2; x >= 0; x--)
                {
                    moved |= ShiftRight(x, y, merged);
                }
            }
        }
        if (gridDirection.y < 0) // вверх
        {
            for (int x = 0; x < fieldSize.x; x++)
            {
                for (int y = 1; y < fieldSize.y; y++)
                {
                    moved |= ShiftUp(x, y, merged);
                }
            }
        }
        else if (gridDirection.y > 0) // вниз
        {
            for (int x = 0; x < fieldSize.x; x++)
            {
                for (int y = fieldSize.y - 2; y >= 0; y--)
                {
                    moved |= ShiftDown(x, y, merged);
                }
            }
        }
        return moved;
    }

    private bool ShiftLeft(int x, int y, bool[,] merged)
    {
        if (Field[x, y] == null) return false;
        bool changed = false;
        Vector2Int targetPos = new Vector2Int(x, y);

        while (targetPos.x > 0)
        {
            int nextX = targetPos.x - 1;
            if (Field[nextX, y] == null)
            {
                Field[nextX, y] = Field[targetPos.x, y];
                Field[targetPos.x, y] = null;
                targetPos.x = nextX;
                changed = true;
            }
            else if (!merged[nextX, y] && Field[nextX, y].Cell.Value == Field[targetPos.x, y].Cell.Value)
            {
                Field[nextX, y].Cell.Value++;
                Destroy(Field[targetPos.x, y].gameObject);
                Field[targetPos.x, y] = null;
                merged[nextX, y] = true;
                changed = true;
                break;
            }
            else break;
        }
        if (Field[targetPos.x, y] != null)
            Field[targetPos.x, y].Cell.Position = targetPos;
        return changed;
    }

    private bool ShiftRight(int x, int y, bool[,] merged)
    {
        if (Field[x, y] == null) return false;
        bool changed = false;
        Vector2Int targetPos = new Vector2Int(x, y);

        while (targetPos.x < fieldSize.x - 1)
        {
            int nextX = targetPos.x + 1;
            if (Field[nextX, y] == null)
            {
                Field[nextX, y] = Field[targetPos.x, y];
                Field[targetPos.x, y] = null;
                targetPos.x = nextX;
                changed = true;
            }
            else if (!merged[nextX, y] && Field[nextX, y].Cell.Value == Field[targetPos.x, y].Cell.Value)
            {
                Field[nextX, y].Cell.Value++;
                Destroy(Field[targetPos.x, y].gameObject);
                Field[targetPos.x, y] = null;
                merged[nextX, y] = true;
                changed = true;
                break;
            }
            else break;
        }
        if (Field[targetPos.x, y] != null)
            Field[targetPos.x, y].Cell.Position = targetPos;
        return changed;
    }

    private bool ShiftUp(int x, int y, bool[,] merged)
    {
        if (Field[x, y] == null) return false;
        bool changed = false;
        Vector2Int targetPos = new Vector2Int(x, y);

        while (targetPos.y > 0)
        {
            int nextY = targetPos.y - 1;
            if (Field[x, nextY] == null)
            {
                Field[x, nextY] = Field[x, targetPos.y];
                Field[x, targetPos.y] = null;
                targetPos.y = nextY;
                changed = true;
            }
            else if (!merged[x, nextY] && Field[x, nextY].Cell.Value == Field[x, targetPos.y].Cell.Value)
            {
                Field[x, nextY].Cell.Value++;
                Destroy(Field[x, targetPos.y].gameObject);
                Field[x, targetPos.y] = null;
                merged[x, nextY] = true;
                changed = true;
                break;
            }
            else break;
        }
        if (Field[targetPos.x, targetPos.y] != null)
            Field[targetPos.x, targetPos.y].Cell.Position = targetPos;
        return changed;
    }

    private bool ShiftDown(int x, int y, bool[,] merged)
    {
        if (Field[x, y] == null) return false;
        bool changed = false;
        Vector2Int targetPos = new Vector2Int(x, y);

        while (targetPos.y < fieldSize.y - 1)
        {
            int nextY = targetPos.y + 1;
            if (Field[x, nextY] == null)
            {
                Field[x, nextY] = Field[x, targetPos.y];
                Field[x, targetPos.y] = null;
                targetPos.y = nextY;
                changed = true;
            }
            else if (!merged[x, nextY] && Field[x, nextY].Cell.Value == Field[x, targetPos.y].Cell.Value)
            {
                Field[x, nextY].Cell.Value++;
                Destroy(Field[x, targetPos.y].gameObject);
                Field[x, targetPos.y] = null;
                merged[x, nextY] = true;
                changed = true;
                break;
            }
            else break;
        }
        if (Field[targetPos.x, targetPos.y] != null)
            Field[targetPos.x, targetPos.y].Cell.Position = targetPos;
        return changed;
    }

    private void UpdateScore()
    {
        int newScore = 0;
        for (int y = 0; y < fieldSize.y; y++)
        {
            for (int x = 0; x < fieldSize.x; x++)
            {
                if (Field[x, y] != null)
                {
                    int displayedValue = (int)Mathf.Pow(2, Field[x, y].Cell.Value);
                    newScore += displayedValue;
                }
            }
        }
        score = newScore;
        Debug.Log("Score: " + score);
    }
}
