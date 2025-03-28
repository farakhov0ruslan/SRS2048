using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
  private const float MinSwipeDistance = 50f; // Минимальное расстояние для распознавания свайпа
  private Vector2 _initialPosition; // Начальная позиция касания/нажатия
  private GameField _gameField; // Приватное поле для ссылки на GameField


  private void Awake()
  {
    _gameField = FindObjectOfType<GameField>();
    if (_gameField == null)
    {
      Debug.LogError("Объект GameField не найден в сцене!");
    }
  }


  void Update()
  {
    HandleKeyboardInput();
    HandleMouseSwipe();
    HandleTouchSwipe();
  }

  // Обработка ввода с клавиатуры (WASD или стрелки)
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

    if (direction != Vector2.zero)
    {
      Debug.Log("Клавиатурный ввод: " + direction);
      // Передаём направление в логику игры (предполагается, что GameField реализует синглтон Instance)
      _gameField.ProcessInput(direction);
    }
  }

  // Обработка свайпов мышью
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

      if (swipeDelta.magnitude > MinSwipeDistance)
      {
        Vector2 swipeDir = DetermineCardinalDirection(swipeDelta.normalized);
        Debug.Log("Свайп мышью: " + swipeDir);
        _gameField.ProcessInput(swipeDir);
      }
    }
  }

  // Обработка сенсорного ввода
  private void HandleTouchSwipe()
  {
    if (Input.touchCount > 0)
    {
      Touch touch = Input.GetTouch(0);

      if (touch.phase == TouchPhase.Began)
      {
        _initialPosition = touch.position;
      }
      else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
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


  // Определяет основное направление движения, выбирая из четырёх (вверх, вниз, влево, вправо).
  private Vector2 DetermineCardinalDirection(Vector2 normalizedDelta)
  {
    if (Mathf.Abs(normalizedDelta.x) > Mathf.Abs(normalizedDelta.y))
    {
      return (normalizedDelta.x > 0) ? Vector2.right : Vector2.left;
    }

    return (normalizedDelta.y > 0) ? Vector2.up : Vector2.down;
  }
}