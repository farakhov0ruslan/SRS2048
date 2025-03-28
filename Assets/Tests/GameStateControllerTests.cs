using FluentAssertions;
using NUnit.Framework;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    [TestFixture]
    public class GameStateControllerTests
    {
        private GameObject gameStateGO;
        private GameStateController gameStateController;
        private GameObject gameFieldGO;
        private GameField gameField;
        private GameObject dummyPanelGO;
        private EndGamePanelController endGamePanel;
        private GameObject dummyGridGO;
        private Grid dummyGrid;
        private GameObject dummyInputGO;
        private InputController dummyInput;
    
        [SetUp]
        public void SetUp() {
            // Создаем GameField
            gameFieldGO = new GameObject("GameField", typeof(GameField));
            gameField = gameFieldGO.GetComponent<GameField>();
            gameField.fieldSize = new Vector2Int(2, 2);
            gameField.Field = new CellView[2,2];

            // Создаем Grid
            dummyGridGO = new GameObject("DummyGrid", typeof(Grid));
            dummyGrid = dummyGridGO.GetComponent<Grid>();
            dummyGrid.CellsTransforms = new System.Collections.Generic.Dictionary<Vector2Int, Transform>();
            for (int y=0; y<2; y++){
                for (int x=0; x<2; x++){
                    var place = new GameObject($"CellPlace_{x}_{y}").transform;
                    dummyGrid.CellsTransforms[new Vector2Int(x,y)] = place;
                }
            }
            dummyGridGO.transform.SetParent(gameFieldGO.transform);

            // Создаем панель
            dummyPanelGO = new GameObject("EndGamePanel", typeof(EndGamePanelController));
            endGamePanel = dummyPanelGO.GetComponent<EndGamePanelController>();
            dummyPanelGO.SetActive(false);

            // Добавляем TMP_Text и кнопку
            var textGO = new GameObject("MessageText", typeof(TextMeshProUGUI));
            textGO.transform.SetParent(dummyPanelGO.transform);
            var tmp = textGO.GetComponent<TextMeshProUGUI>();

            var btnGO = new GameObject("RestartButton", typeof(Button), typeof(TextMeshProUGUI));
            btnGO.transform.SetParent(dummyPanelGO.transform);
            var btn = btnGO.GetComponent<Button>();

            // Присваиваем приватные поля EndGamePanelController через reflection
            var msgField = typeof(EndGamePanelController).GetField("messageText",
                BindingFlags.NonPublic|BindingFlags.Instance);
            msgField.SetValue(endGamePanel, tmp);

            var rbField = typeof(EndGamePanelController).GetField("restartButton",
                BindingFlags.NonPublic|BindingFlags.Instance);
            rbField.SetValue(endGamePanel, btn);

            // Создаем GameStateController
            gameStateGO = new GameObject("GameStateController", typeof(GameStateController));
            gameStateController = gameStateGO.GetComponent<GameStateController>();

            // Присваиваем privat'ные поля через reflection
            typeof(GameStateController).GetField("gameField", BindingFlags.NonPublic|BindingFlags.Instance)
                ?.SetValue(gameStateController, gameField);
            typeof(GameStateController).GetField("endGamePanel", BindingFlags.NonPublic|BindingFlags.Instance)
                ?.SetValue(gameStateController, endGamePanel);

            // Создаем фиктивный InputController
            dummyInputGO = new GameObject("DummyInputController", typeof(InputController));
            dummyInput = dummyInputGO.GetComponent<InputController>();
            typeof(GameStateController).GetField("inputController", BindingFlags.NonPublic|BindingFlags.Instance)
                ?.SetValue(gameStateController, dummyInput);

            // winThreshold=4 для упрощения (2^2=4)
            gameStateController.winThreshold = 4;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameStateGO);
            Object.DestroyImmediate(gameFieldGO);
            Object.DestroyImmediate(dummyPanelGO);
            Object.DestroyImmediate(dummyGridGO);
            Object.DestroyImmediate(dummyInputGO);
        }

        [Test]
        public void CheckGameState_ShouldShowWin_WhenTileMeetsThreshold()
        {
            // Плитка (0,0) со значением 2 => 2^2=4 => win
            var go = new GameObject("DummyCell", typeof(CellView));
            var cv = go.GetComponent<CellView>();
            var c = new Cell(new Vector2Int(0,0),2);
            cv.Init(c, dummyGrid);
            gameField.Field[0,0] = cv;

            gameStateController.CheckGameState();

            dummyPanelGO.activeSelf.Should().BeTrue("должна показаться панель win");
            gameField.enabled.Should().BeFalse();
            dummyInput.enabled.Should().BeFalse();
        }

        [Test]
        public void CheckGameState_ShouldShowGameOver_WhenNoMovesAvailable()
        {
            // Заполняем (0,0)->1, (1,0)->2, (0,1)->2, (1,1)->1 => нет merges
            var arr = new (int x, int y, int val)[]
            {
                (0,0,1), (1,0,2), (0,1,2), (1,1,1)
            };
            foreach(var t in arr)
            {
                var go = new GameObject($"Cell_{t.x}_{t.y}", typeof(CellView));
                var cv = go.GetComponent<CellView>();
                cv.Init(new Cell(new Vector2Int(t.x,t.y), t.val), dummyGrid);
                gameField.Field[t.x,t.y] = cv;
            }

            gameStateController.CheckGameState();

            dummyPanelGO.activeSelf.Should().BeTrue("Все заполнено и нет merges => Game Over");
            gameField.enabled.Should().BeFalse();
            dummyInput.enabled.Should().BeFalse();
        }

        [Test]
        public void CheckGameState_NoWinAndNoGameOver_ShouldDoNothing()
        {
            // Пустое поле => есть свободные клетки => не game over
            // Нет плиток => не win
            gameStateController.CheckGameState();

            dummyPanelGO.activeSelf.Should().BeFalse();
            gameField.enabled.Should().BeTrue();
            dummyInput.enabled.Should().BeTrue();
        }

        [Test]
        public void CheckGameState_NoEndGamePanel_ShouldNotThrow()
        {
            // Удаляем panel
            Object.DestroyImmediate(dummyPanelGO);
            typeof(GameStateController).GetField("endGamePanel", BindingFlags.NonPublic|BindingFlags.Instance)
                ?.SetValue(gameStateController, null);

            // Плитка => >= threshold => win
            var go = new GameObject("Cell", typeof(CellView));
            var cv = go.GetComponent<CellView>();
            cv.Init(new Cell(new Vector2Int(0,0),2), dummyGrid); // val=2 => tile=4
            gameField.Field[0,0] = cv;

            gameStateController.CheckGameState();

            gameField.enabled.Should().BeFalse("хотя нет панели, gameField всё равно отключается");
            dummyInput.enabled.Should().BeFalse();
        }

        [Test]
        public void CheckGameState_NoInputController_ShouldNotThrow()
        {
            // Удаляем input
            Object.DestroyImmediate(dummyInputGO);
            typeof(GameStateController).GetField("inputController", BindingFlags.NonPublic|BindingFlags.Instance)
                ?.SetValue(gameStateController, null);

            // У нас есть panel
            dummyPanelGO.SetActive(false);

            // Плитка => >= threshold => win
            var go = new GameObject("Cell", typeof(CellView));
            var cv = go.GetComponent<CellView>();
            cv.Init(new Cell(new Vector2Int(0,0),2), dummyGrid);
            gameField.Field[0,0] = cv;

            gameStateController.CheckGameState();

            dummyPanelGO.activeSelf.Should().BeTrue("должна показаться панель Win");
            gameField.enabled.Should().BeFalse();
        }

        [Test]
        public void CheckGameState_NotEmptyButCanMerge_ShouldNotShowGameOver()
        {
            // (0,0)->1, (1,0)->1 => можно слиться
            var go1 = new GameObject("Cell1", typeof(CellView));
            var cv1 = go1.GetComponent<CellView>();
            cv1.Init(new Cell(new Vector2Int(0,0),1), dummyGrid);
            gameField.Field[0,0] = cv1;

            var go2 = new GameObject("Cell2", typeof(CellView));
            var cv2 = go2.GetComponent<CellView>();
            cv2.Init(new Cell(new Vector2Int(1,0),1), dummyGrid);
            gameField.Field[1,0] = cv2;

            gameStateController.CheckGameState();
            dummyPanelGO.activeSelf.Should().BeFalse("можно объединить => нет gameover");
        }

        // ========================== ДОБАВЛЕННЫЕ ТЕСТЫ ==========================
        
    }
}
