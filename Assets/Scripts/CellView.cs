using TMPro;
using UnityEngine;

public class CellView : MonoBehaviour
{
  public TMP_Text text; // UI Text (или TMP_Text)
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
    // Например, в 2048 иногда выводят 2^value, но вы можете менять логику
    // Если по заданию нужно Math.Pow(newValue, 2), используйте:
    int displayed = (int)Mathf.Pow(newValue, 2);

    if (text != null)
      text.text = displayed.ToString();
  }

  private void UpdatePosition(Vector2Int newPos)
  {
    // Если используете UI Canvas и хотите ставить клетку в CellPlace:
    // Обычно достаточно родителя CellPlace, но если нужно, можно вручную сместить.
    // transform.localPosition = Vector3.zero; // к примеру, если хотим по центру.

    // Или, если в мире:
    // transform.position = new Vector3(newPos.x, newPos.y, 0);
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