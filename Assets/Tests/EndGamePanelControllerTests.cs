using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    [TestFixture]
    public class EndGamePanelControllerTests
    {
        private GameObject panelGO;
        private EndGamePanelController panelController;
        private TextMeshProUGUI messageText;
        private Button restartButton;
    
        [SetUp]
        public void SetUp() {
            // Создаем GameObject для панели с компонентом EndGamePanelController
            panelGO = new GameObject("EndGamePanel", typeof(EndGamePanelController));
            panelController = panelGO.GetComponent<EndGamePanelController>();
        
            // Создаем дочерний объект для текстового сообщения с конкретной реализацией TextMeshProUGUI
            var textGO = new GameObject("MessageText", typeof(TextMeshProUGUI));
            textGO.transform.SetParent(panelGO.transform);
            messageText = textGO.GetComponent<TextMeshProUGUI>();
        
            // Создаем дочерний объект для кнопки перезапуска с компонентом Button
            var buttonGO = new GameObject("RestartButton", typeof(Button), typeof(TextMeshProUGUI));
            buttonGO.transform.SetParent(panelGO.transform);
            restartButton = buttonGO.GetComponent<Button>();
        
            // Назначаем приватные поля через reflection
            FieldInfo messageTextField = typeof(EndGamePanelController)
                .GetField("messageText", BindingFlags.NonPublic | BindingFlags.Instance);
            messageTextField.Should().NotBeNull();
            messageTextField.SetValue(panelController, messageText);
        
            FieldInfo restartButtonField = typeof(EndGamePanelController)
                .GetField("restartButton", BindingFlags.NonPublic | BindingFlags.Instance);
            restartButtonField.Should().NotBeNull();
            restartButtonField.SetValue(panelController, restartButton);
        
            // Панель по умолчанию неактивна
            panelGO.SetActive(false);
        }
    
        [TearDown]
        public void TearDown() {
            GameObject.DestroyImmediate(panelGO);
        }
    
        [Test]
        public void ShowEndGame_ShouldSetMessageAndActivatePanel()
        {
            string message = "You Win!";
            panelController.ShowEndGame(message);
            messageText.text.Should().Be(message);
            panelGO.activeSelf.Should().BeTrue();
        }
    }
}
