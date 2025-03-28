using UnityEngine;


public class InputController : MonoBehaviour
{
    private const float MinSwipeDistance = 50f;
    private Vector2 _initialPosition;
    private GameField _gameField;

    public void Awake()
    {
        // Находим GameField в сцене
        _gameField = FindObjectOfType<GameField>();
        if (_gameField == null)
        {
            Debug.LogError("Объект GameField не найден в сцене!");
        }
    }

    public void Update()
    {
        HandleKeyboardInput();
        HandleMouseSwipe();
        HandleTouchSwipe();
    }

    /// <summary>
    /// Обрабатывает ввод с клавиатуры (WASD/стрелки).
    /// Если нажата кнопка, отправляет направление в GameField.
    /// </summary>
    private void HandleKeyboardInput()
    {
        Vector2 direction = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            direction = Vector2.right;

        if (direction != Vector2.zero && _gameField != null)
        {
            Debug.Log("Клавиатурный ввод: " + direction);
            _gameField.ProcessInput(direction);
        }
    }

    /// <summary>
    /// Обрабатывает свайп мышью. При отпускании кнопки мыши вычисляет вектор свайпа
    /// и, если длина свайпа превышает минимальное расстояние, передаёт направление в GameField.
    /// </summary>
    private void HandleMouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _initialPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 endPosition = Input.mousePosition;
            Vector2 swipeDelta = endPosition - _initialPosition;
            if (_gameField != null && swipeDelta.magnitude > MinSwipeDistance)
            {
                Vector2 swipeDir = DetermineCardinalDirection(swipeDelta.normalized);
                Debug.Log("Свайп мышью: " + swipeDir);
                _gameField.ProcessInput(swipeDir);
            }
        }
    }

    /// <summary>
    /// Обрабатывает сенсорный ввод. При завершении касания вычисляет вектор свайпа и,
    /// если его длина превышает минимальное расстояние, отправляет направление в GameField.
    /// </summary>
    private void HandleTouchSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _initialPosition = touch.position;
            }
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && _gameField != null)
            {
                Vector2 endPosition = touch.position;
                Vector2 swipeDelta = endPosition - _initialPosition;
                if (swipeDelta.magnitude > MinSwipeDistance)
                {
                    Vector2 swipeDir = DetermineCardinalDirection(swipeDelta.normalized);
                    Debug.Log("Свайп сенсорного экрана: " + swipeDir);
                    _gameField.ProcessInput(swipeDir);
                }
            }
        }
    }

    /// <summary>
    /// Определяет кардинальное направление движения по нормализованному вектору.
    /// Если по оси X величина больше – возвращает вправо или влево, иначе – вверх или вниз.
    /// </summary>
    private Vector2 DetermineCardinalDirection(Vector2 normalizedDelta)
    {
        if (Mathf.Abs(normalizedDelta.x) > Mathf.Abs(normalizedDelta.y))
        {
            return (normalizedDelta.x > 0) ? Vector2.right : Vector2.left;
        }
        return (normalizedDelta.y > 0) ? Vector2.up : Vector2.down;
    }
}
