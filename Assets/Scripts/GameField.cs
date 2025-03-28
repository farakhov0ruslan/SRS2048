using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameField : MonoBehaviour
{
  // Размер поля: fieldSize.x = количество столбцов (x), fieldSize.y = количество строк (y)
  public Vector2Int fieldSize = new Vector2Int(4, 4);

  // Префаб плитки (должен содержать компонент CellView)
  public GameObject cellViewPrefab;

  // Список данных плиток (Cell) – может пригодиться для сохранения/подсчёта
  public List<Cell> cells = new List<Cell>();

  // Массив визуальных представлений плиток: Field[x, y], где x – столбец, y – строка (0-я строка сверху)
  public CellView[,] Field;

  // Текущий счёт
  public int score = 0;

  // Ссылка на объект Grid, который хранит привязку ячеек (CellPlace)
  private Grid _grid;

  public ScoreController scoreController;
  public GameStateController gameStateController;

  private void Awake()
  {
    _grid = GetComponentInChildren<Grid>();
    Field = new CellView[fieldSize.x, fieldSize.y];
  }

  private void Start()
  {
    // Создаем начальные плитки
    CreateCell();
    CreateCell();
    // DebugPrintField();
  }


  // Находит случайную пустую позицию в массиве Field.
  public Vector2Int GetEmptyPosition()
  {
    List<Vector2Int> freePositions = new List<Vector2Int>();
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

  /// <summary>
  /// Создает новую плитку в случайной пустой позиции.
  /// С вероятностью 20% создается плитка со значением 4 (Cell.Value = 2),
  /// иначе – со значением 2 (Cell.Value = 1).
  /// </summary>
  public void CreateCell()
  {
    Vector2Int pos = GetEmptyPosition();
    if (pos.x < 0) return; // Нет свободных позиций

    int newValue = (Random.value <= 0.2f) ? 2 : 1;
    Cell newCell = new Cell(pos, newValue);
    cells.Add(newCell);

    if (cellViewPrefab != null)
    {
      // Получаем Transform ячейки из Grid для размещения плитки
      Transform cellPlaceTransform = _grid.CellsTransforms[pos];
      GameObject cellViewObject = Instantiate(cellViewPrefab, cellPlaceTransform);
      CellView cellView = cellViewObject.GetComponent<CellView>();
      if (cellView != null)
      {
        cellView.Init(newCell, _grid);
        Field[pos.x, pos.y] = cellView;
      }
      else
      {
        Debug.LogError("Префаб не содержит компонента CellView!");
      }
    }
    else
    {
      Debug.LogError("Не установлен префаб для плитки (cellViewPrefab)!");
    }
  }

  /// <summary>
  /// Обрабатывает входное направление. Перед вызовом логики перемещения
  /// преобразует координатную систему так, чтобы "вверх" в поле соответствовал уменьшению y.
  /// Если ход изменил поле, создается новая плитка и обновляется счёт.
  /// После хода выводится состояние поля в консоль.
  /// </summary>
  public void ProcessInput(Vector2 inputDirection)
  {
    // Преобразуем входное направление: Unity: Vector2.down = (0, -1), но в нашей сетке y увеличивается вниз.
    // Поэтому инвертируем вертикальную компоненту.
    Vector2 gridDirection = new Vector2(inputDirection.x, -inputDirection.y);

    bool moved = MoveTiles(gridDirection);
    if (moved)
    {
      CreateCell();
      UpdateScore();
    }
    else
    {
      // Debug.Log("Ход не изменил поле.");
    }

    // DebugPrintField();  
    if (scoreController != null)
      scoreController.UpdateScore(score);
    if (gameStateController != null)
      gameStateController.CheckGameState();
  }

  /// <summary>
  /// Перемещает и объединяет плитки в направлении gridDirection.
  /// gridDirection: (±1, 0) для горизонтальных и (0, ±1) для вертикальных движений.
  /// Возвращает true, если хотя бы одна плитка переместилась или объединилась.
  /// Реализована отдельная логика для горизонтальных и вертикальных движений.
  /// </summary>
  private bool MoveTiles(Vector2 gridDirection)
  {
    bool moved = false;
    // Флаг, запрещающий повторное объединение одной плитки за ход.
    bool[,] merged = new bool[fieldSize.x, fieldSize.y];

    // Горизонтальное перемещение
    if (gridDirection.x < 0) // движение влево
    {
      // Для движения влево обходим столбцы от x = 1 до x = width-1 (слева не двигаем 0-й столбец)
      for (int y = 0; y < fieldSize.y; y++)
      {
        for (int x = 1; x < fieldSize.x; x++)
        {
          if (Field[x, y] != null)
          {
            Vector2Int pos = new Vector2Int(x, y);
            Vector2Int targetPos = pos;
            while (targetPos.x > 0)
            {
              int nextX = targetPos.x - 1;
              // Если ячейка слева пуста
              if (Field[nextX, y] == null)
              {
                Field[nextX, y] = Field[targetPos.x, y];
                Field[targetPos.x, y] = null;
                targetPos.x = nextX;
                moved = true;
              }
              // Если ячейка слева имеет ту же ценность и ещё не объединилась
              else if (!merged[nextX, y] &&
                       Field[nextX, y].Cell.Value == Field[targetPos.x, y].Cell.Value)
              {
                Field[nextX, y].Cell.Value++;
                Destroy(Field[targetPos.x, y].gameObject);
                Field[targetPos.x, y] = null;
                merged[nextX, y] = true;
                moved = true;
                break;
              }
              else break;
            }

            if (Field[targetPos.x, y] != null)
              Field[targetPos.x, y].Cell.Position = targetPos;
          }
        }
      }
    }
    else if (gridDirection.x > 0) // движение вправо
    {
      // Для движения вправо обходим столбцы от x = width-2 до 0
      for (int y = 0; y < fieldSize.y; y++)
      {
        for (int x = fieldSize.x - 2; x >= 0; x--)
        {
          if (Field[x, y] != null)
          {
            Vector2Int pos = new Vector2Int(x, y);
            Vector2Int targetPos = pos;
            while (targetPos.x < fieldSize.x - 1)
            {
              int nextX = targetPos.x + 1;
              if (Field[nextX, y] == null)
              {
                Field[nextX, y] = Field[targetPos.x, y];
                Field[targetPos.x, y] = null;
                targetPos.x = nextX;
                moved = true;
              }
              else if (!merged[nextX, y] &&
                       Field[nextX, y].Cell.Value == Field[targetPos.x, y].Cell.Value)
              {
                Field[nextX, y].Cell.Value++;
                Destroy(Field[targetPos.x, y].gameObject);
                Field[targetPos.x, y] = null;
                merged[nextX, y] = true;
                moved = true;
                break;
              }
              else break;
            }

            if (Field[targetPos.x, y] != null)
              Field[targetPos.x, y].Cell.Position = targetPos;
          }
        }
      }
    }

    // Вертикальное перемещение
    if (gridDirection.y < 0) // движение вверх (так как gridDirection.y < 0 соответствует "вверх" в сетке)
    {
      // Для движения вверх обходим строки от y = 1 до y = height-1
      for (int x = 0; x < fieldSize.x; x++)
      {
        for (int y = 1; y < fieldSize.y; y++)
        {
          if (Field[x, y] != null)
          {
            Vector2Int pos = new Vector2Int(x, y);
            Vector2Int targetPos = pos;
            while (targetPos.y > 0)
            {
              int nextY = targetPos.y - 1;
              if (Field[x, nextY] == null)
              {
                Field[x, nextY] = Field[x, targetPos.y];
                Field[x, targetPos.y] = null;
                targetPos.y = nextY;
                moved = true;
              }
              else if (!merged[x, nextY] &&
                       Field[x, nextY].Cell.Value == Field[x, targetPos.y].Cell.Value)
              {
                Field[x, nextY].Cell.Value++;
                Destroy(Field[x, targetPos.y].gameObject);
                Field[x, targetPos.y] = null;
                merged[x, nextY] = true;
                moved = true;
                break;
              }
              else break;
            }

            if (Field[x, targetPos.y] != null)
              Field[x, targetPos.y].Cell.Position = targetPos;
          }
        }
      }
    }
    else if (gridDirection.y > 0) // движение вниз
    {
      // Для движения вниз обходим строки от y = height-2 до 0
      for (int x = 0; x < fieldSize.x; x++)
      {
        for (int y = fieldSize.y - 2; y >= 0; y--)
        {
          if (Field[x, y] != null)
          {
            Vector2Int pos = new Vector2Int(x, y);
            Vector2Int targetPos = pos;
            while (targetPos.y < fieldSize.y - 1)
            {
              int nextY = targetPos.y + 1;
              if (Field[x, nextY] == null)
              {
                Field[x, nextY] = Field[x, targetPos.y];
                Field[x, targetPos.y] = null;
                targetPos.y = nextY;
                moved = true;
              }
              else if (!merged[x, nextY] &&
                       Field[x, nextY].Cell.Value == Field[x, targetPos.y].Cell.Value)
              {
                Field[x, nextY].Cell.Value++;
                Destroy(Field[x, targetPos.y].gameObject);
                Field[x, targetPos.y] = null;
                merged[x, nextY] = true;
                moved = true;
                break;
              }
              else break;
            }

            if (Field[x, targetPos.y] != null)
              Field[x, targetPos.y].Cell.Position = targetPos;
          }
        }
      }
    }

    return moved;
  }

  /// <summary>
  /// Пересчитывает счёт как сумму всех отображаемых значений плиток.
  /// Отображаемое значение вычисляется как 2^(Cell.Value).
  /// </summary>
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


  // Выводит текущее состояние поля в консоль.
  private void DebugPrintField()
  {
    string output = "Состояние поля:\n";
    for (int y = 0; y < fieldSize.y; y++)
    {
      for (int x = 0; x < fieldSize.x; x++)
      {
        if (Field[x, y] != null)
        {
          int displayedValue = (int)Mathf.Pow(2, Field[x, y].Cell.Value);
          output += displayedValue.ToString().PadRight(5);
        }
        else
        {
          output += ".".PadRight(5);
        }
      }

      output += "\n";
    }

    Debug.Log(output);
  }
}