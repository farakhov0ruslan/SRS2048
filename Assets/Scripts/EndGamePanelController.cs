using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGamePanelController : MonoBehaviour
{
  [SerializeField] private TMP_Text messageText; // Текстовое сообщение (победа/проигрыш)
  [SerializeField] private Button restartButton; // Кнопка перезапуска игры
  

  private void Start()
  {
    // Панель скрыта по умолчанию
    restartButton.onClick.AddListener(RestartGame);
  }

  /// <summary>
  /// Отображает панель с заданным сообщением.
  /// </summary>
  /// <param name="message">Сообщение: "You Win!" или "Game Over"</param>
  public void ShowEndGame(string message)
  {
    messageText.text = message;
    // Debug.Log($"Открытие сцены{message}");
    gameObject.SetActive(true);
  }

  /// <summary>
  /// Перезапускает текущую сцену.
  /// </summary>
  private void RestartGame()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }
}