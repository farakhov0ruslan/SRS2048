using FluentAssertions;
using NUnit.Framework;
using TMPro;
using UnityEngine;

namespace Tests
{
  [TestFixture]
  public class ScoreControllerTests
  {
    private GameObject scoreGO;
    private ScoreController scoreController;
    private TextMeshProUGUI scoreText;
    
    [SetUp]
    public void SetUp() {
      // Создаем объект с компонентом ScoreController
      scoreGO = new GameObject("ScoreController", typeof(ScoreController));
      scoreController = scoreGO.GetComponent<ScoreController>();
        
      // Создаем дочерний объект для текстового компонента с конкретной реализацией TextMeshProUGUI
      var textGO = new GameObject("ScoreText", typeof(TextMeshProUGUI));
      textGO.transform.SetParent(scoreGO.transform);
      scoreText = textGO.GetComponent<TextMeshProUGUI>();
        
      // Назначаем приватное поле scoreText через reflection
      var field = typeof(ScoreController)
        .GetField("scoreText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      field.Should().NotBeNull("Поле scoreText должно быть определено");
      field.SetValue(scoreController, scoreText);
    }
    
    [TearDown]
    public void TearDown() {
      GameObject.DestroyImmediate(scoreGO);
    }
    
    [Test]
    public void UpdateScore_ShouldUpdateText()
    {
      // Вызываем UpdateScore и проверяем, что текст обновился правильно
      scoreController.UpdateScore(100);
      scoreText.text.Should().Be("Current\n100");
    }
  }
}