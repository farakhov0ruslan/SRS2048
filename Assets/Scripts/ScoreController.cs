using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
  [SerializeField] private TMP_Text scoreText; // Ссылка на UI текст для отображения счёта

  private int currentScore = 0;

  private void Start()
  {
    // Изначально устанавливаем 0
    UpdateScore(0);
  }

  // Обновляет отображаемый счёт.
  public void UpdateScore(int newScore)
  {
    currentScore = newScore;
    scoreText.text = $"Current\n{currentScore}";
  }
}