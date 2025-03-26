using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
  public Vector2Int fieldSize = new(4, 4);

  // Префаб для отображения клетки (должен иметь компонент CellView)
  public GameObject cellViewPrefab;

  // Список созданных клеток
  public List<Cell> cells = new List<Cell>();

  // Для хранения позиций, которые  уже заняты клетками (можно расширить логику)
  private HashSet<Vector2Int> _occupiedPositions = new HashSet<Vector2Int>();

  void Start()
  {
    CreateCell();
  }

  // Метод возвращает случайную пустую позицию на поле
  public Vector2Int GetEmptyPosition()
  {
    List<Vector2Int> freePositions = new List<Vector2Int>();

    for (int y = 0; y < fieldSize.x; y++)
    {
      for (int x = 0; x < fieldSize.y; x++)
      {
        Vector2Int pos = new Vector2Int(x, y);
        if (!_occupiedPositions.Contains(pos))
        {
          freePositions.Add(pos);
        }
      }
    }

    if (freePositions.Count == 0)
    {
      Debug.LogWarning("Нет свободных позиций!");
      return Vector2Int.zero;
    }

    return freePositions[Random.Range(0, freePositions.Count)];
  }

  // Метод создания новой клетки
  public void CreateCell()
  {
    // Определяем позицию
    Vector2Int pos = GetEmptyPosition();

    // Вероятность 90% для 1, 10% для 2
    int value = (Random.value <= 0.9f) ? 1 : 2;

    // Создаем новую клетку
    Cell newCell = new Cell(pos, value);
    cells.Add(newCell);
    _occupiedPositions.Add(pos);

    // Создаем префаб CellView и инициализируем его
    if (cellViewPrefab != null)
    {
      var cellPlaceTransform = GetComponentInChildren<Grid>().CellsTransforms[pos];
      GameObject cellViewObject = Instantiate(cellViewPrefab, cellPlaceTransform);
      
      
      Debug.Log($"Создана клетка с коориданатыми {cellPlaceTransform.position.ToString()}");
      CellView cellView = cellViewObject.GetComponent<CellView>();
      if (cellView != null)
      {
        cellView.Init(newCell);
      }
      else
      {
        Debug.LogError("Префаб не содержит компонента CellView!");
      }
    }
    else
    {
      Debug.LogError("Не установлен префаб для клетки (cellViewPrefab)!");
    }
  }
}