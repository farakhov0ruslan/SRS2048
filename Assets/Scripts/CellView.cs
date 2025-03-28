using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
  public TMP_Text text; //  TMP_Text со значением клетки
  [SerializeField] private Color startColor = Color.white; // Начальный цвет
  [SerializeField] private Color endColor = Color.red; // Конечный цвет
  [SerializeField] private int maxExponent = 11; // Максимальное значение exponent (можно настроить)

  private Image _image; // Компонент Image, на котором будет меняться цвет
  public Cell Cell;
  private Grid _grid;            // Ссылка на Grid для доступа к CellsTransforms

  public void Init(Cell cell, Grid grid)
  {
    this.Cell = cell;
    _grid = grid;
    cell.OnValueChanged += UpdateValue;
    cell.OnPositionChanged += UpdatePosition;
    this.text = GetComponentInChildren<TMP_Text>();
    _image = GetComponent<Image>();


    // Инициализируем текущими значениями
    UpdateValue(cell.Value);
    UpdatePosition(cell.Position);
  }

  private void UpdateValue(int newValue)
  {
    int displayed = (int)Mathf.Pow(2, newValue);

    if (text != null)
      text.text = displayed.ToString();

    // Рассчитываем параметр для Lerp
    float t = Mathf.Clamp01((float)(newValue - 1) / (maxExponent - 1));

    // Применяем Lerp для получения промежуточного цвета
    if (_image != null)
    {
      _image.color = Color.Lerp(startColor, endColor, t);
    }
  }

  private void UpdatePosition(Vector2Int newPos)
  {
    if (_grid != null && _grid.CellsTransforms.ContainsKey(newPos))
    {
      Transform newParent = _grid.CellsTransforms[newPos];
      transform.SetParent(newParent);
      transform.localPosition = Vector3.zero;
    }
    else
    {
      Debug.LogWarning("Не удалось обновить позицию: отсутствует Grid или трансформ для позиции " + newPos);
    }
  }

  private void OnDestroy()
  {
    // Важно отписаться от событий
    if (Cell != null)
    {
      Cell.OnValueChanged -= UpdateValue;
      Cell.OnPositionChanged -= UpdatePosition;
    }
  }
}