using TMPro;
using UnityEngine;

public class CellView : MonoBehaviour
{
  public TMP_Text text; //  TMP_Text со значением клетки
  private Cell _cell;

  public void Init(Cell cell)
  {
    this._cell = cell;
    cell.OnValueChanged += UpdateValue;
    cell.OnPositionChanged += UpdatePosition;
    this.text = GetComponentInChildren<TMP_Text>();


    // Инициализируем текущими значениями
    UpdateValue(cell.Value);
    UpdatePosition(cell.Position);
  }
  
  private void UpdateValue(int newValue)
  {
    
    int displayed = (int)Mathf.Pow(newValue, 2);

    if (text != null)
      text.text = displayed.ToString();
  }

  private void UpdatePosition(Vector2Int newPos)
  {
    transform.localPosition = Vector3.zero; // Ставим локальные координатына 0.
  }

  private void OnDestroy()
  {
    // Важно отписаться от событий
    if (_cell != null)
    {
      _cell.OnValueChanged -= UpdateValue;
      _cell.OnPositionChanged -= UpdatePosition;
    }
  }
}