using System;
using UnityEngine;

public class Cell 
{
  // Свойства позиции и значения клетки
  private Vector2Int _position;

  public Vector2Int Position
  {
    get => _position;
    set
    {
      if (_position != value)
      {
        _position = value;
        OnPositionChanged?.Invoke(_position);
      }
    }
  }

  private int _cellValue;

  public int Value
  {
    get => _cellValue;
    set
    {
      if (_cellValue != value)
      {
        _cellValue = value;
        OnValueChanged?.Invoke(_cellValue);
      }
    }
  }

  // События, вызываемые при изменении позиции и значения
  public event Action<Vector2Int> OnPositionChanged;
  public event Action<int> OnValueChanged;

  public Cell(Vector2Int startPosition, int startValue)
  {
    _position = startPosition;
    _cellValue = startValue;
  }
}