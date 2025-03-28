using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using TMPro;
using UnityEngine;

namespace Tests
{
    [TestFixture]
    public class GameFieldTests
    {
        private GameObject gameFieldGO;
        private GameField gameField;
        private GameObject dummyGridGO;
        private Grid dummyGrid;
        
        private GameObject dummyCellPrefab;

        [SetUp]
        public void SetUp()
        {
            gameFieldGO = new GameObject("GameField", typeof(GameField));
            gameField = gameFieldGO.GetComponent<GameField>();
            gameField.fieldSize = new Vector2Int(2, 2);

            dummyGridGO = new GameObject("DummyGrid", typeof(Grid));
            dummyGrid = dummyGridGO.GetComponent<Grid>();
            dummyGrid.CellsTransforms = new Dictionary<Vector2Int, Transform>();

            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    var cellPlaceGO = new GameObject($"CellPlace_{x}_{y}");
                    dummyGrid.CellsTransforms[new Vector2Int(x, y)] = cellPlaceGO.transform;
                }
            }
            dummyGridGO.transform.SetParent(gameFieldGO.transform);

            // Вызов Awake()
            gameField.Awake();

            // Создаём prefab
            dummyCellPrefab = new GameObject("DummyCellPrefab", typeof(CellView));
            var textGO = new GameObject("Text", typeof(TextMeshProUGUI));
            textGO.transform.SetParent(dummyCellPrefab.transform);
            gameField.cellViewPrefab = dummyCellPrefab;

            // Если Start() вызван автоматически (или вызывался), убираем созданные плитки
            if (gameField.cells.Count > 0)
                gameField.cells.Clear();
            for (int y = 0; y < gameField.fieldSize.y; y++)
            {
                for (int x = 0; x < gameField.fieldSize.x; x++)
                {
                    if (gameField.Field[x, y] != null)
                    {
                        Object.DestroyImmediate(gameField.Field[x, y].gameObject);
                        gameField.Field[x, y] = null;
                    }
                }
            }
        }


        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameFieldGO);
            Object.DestroyImmediate(dummyGridGO);
            Object.DestroyImmediate(dummyCellPrefab);
        }

        [Test]
        public void GetEmptyPosition_ShouldReturnValidPosition_WhenFieldIsEmpty()
        {
            Vector2Int pos = gameField.GetEmptyPosition();
            pos.x.Should().BeInRange(0, gameField.fieldSize.x - 1);
            pos.y.Should().BeInRange(0, gameField.fieldSize.y - 1);
        }

        [Test]
        public void CreateCell_ShouldPlaceCellViewInField()
        {
            gameField.CreateCell();
            gameField.cells.Count.Should().Be(1, "должна добавиться 1 новая клетка");

            int countInField = 0;
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    if (gameField.Field[x, y] != null)
                        countInField++;
                }
            }
            countInField.Should().Be(1);
        }

        [Test]
        public void CreateCell_ShouldNotCreate_WhenNoFreePositions()
        {
            // Заполняем всё поле
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    var cellGO = new GameObject($"Cell_{x}_{y}", typeof(CellView));
                    var cellView = cellGO.GetComponent<CellView>();
                    cellView.Init(new Cell(new Vector2Int(x, y), 1), dummyGrid);
                    gameField.Field[x, y] = cellView;
                }
            }
            // Очищаем список cells, чтобы видеть, создаётся ли что-то заново
            gameField.cells.Clear();

            gameField.CreateCell();
            gameField.cells.Should().BeEmpty("нет свободных позиций");
        }

        [Test]
        public void Start_ShouldCreateTwoCells()
        {
            // Убедимся, что prefab есть
            gameField.cells.Clear();
            gameField.Start();

            // После Start() должно быть 2 клетки
            gameField.cells.Count.Should().Be(2);
        }

        [Test]
        public void ProcessInput_ShouldMergeCells_WhenPossible()
        {
            // Две смежные плитки
            var go1 = new GameObject("Cell1", typeof(CellView));
            var v1 = go1.GetComponent<CellView>();
            v1.Init(new Cell(new Vector2Int(0, 0), 1), dummyGrid);
            gameField.Field[0, 0] = v1;
            gameField.cells.Add(v1.Cell);

            var go2 = new GameObject("Cell2", typeof(CellView));
            var v2 = go2.GetComponent<CellView>();
            v2.Init(new Cell(new Vector2Int(1, 0), 1), dummyGrid);
            gameField.Field[1, 0] = v2;
            gameField.cells.Add(v2.Cell);

            // Двигаем влево
            gameField.ProcessInput(Vector2.left);

            // Ожидаем, что (0,0) теперь имеет value=2
            gameField.Field[0, 0].Cell.Value.Should().Be(2);
            // + должна появиться новая плитка => итого 2 в поле (слитая и новая)
            int nonNullCount = 0;
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    if (gameField.Field[x, y] != null)
                        nonNullCount++;
                }
            }
            nonNullCount.Should().Be(2, "после успешного хода появляется новая плитка");
        }

        [TestCase(0, 1)]   // Vector2.up
        [TestCase(0, -1)]  // Vector2.down
        [TestCase(-1, 0)]  // Vector2.left
        public void ProcessInput_OtherDirections(float xDir, float yDir)
        {
            // Две плитки, которые могут объединиться (по вертикали)
            var go1 = new GameObject("Cell1", typeof(CellView));
            var v1 = go1.GetComponent<CellView>();
            v1.Init(new Cell(new Vector2Int(0, 0), 1), dummyGrid);
            gameField.Field[0, 0] = v1;
            gameField.cells.Add(v1.Cell);

            var go2 = new GameObject("Cell2", typeof(CellView));
            var v2 = go2.GetComponent<CellView>();
            v2.Init(new Cell(new Vector2Int(0, 1), 1), dummyGrid);
            gameField.Field[0, 1] = v2;
            gameField.cells.Add(v2.Cell);

            Vector2 direction = new Vector2(xDir, yDir);
            gameField.ProcessInput(direction);

            // При удачном слиянии => 1 плитка с value=2 + появляется новая => итого 2
            int nonNullCount = 0;
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    if (gameField.Field[x, y] != null)
                        nonNullCount++;
                }
            }
            nonNullCount.Should().Be(2, "плитки должны слиться + появится новая");
        }

        [Test]
        public void ProcessInput_ShouldNotChangeField_IfNoMovementPossible()
        {
            // Плитки в неподвижном состоянии
            var go1 = new GameObject("Cell1", typeof(CellView));
            var v1 = go1.GetComponent<CellView>();
            v1.Init(new Cell(new Vector2Int(0, 0), 1), dummyGrid);
            gameField.Field[0, 0] = v1;
            gameField.cells.Add(v1.Cell);

            var go2 = new GameObject("Cell2", typeof(CellView));
            var v2 = go2.GetComponent<CellView>();
            v2.Init(new Cell(new Vector2Int(1, 0), 2), dummyGrid);
            gameField.Field[1, 0] = v2;
            gameField.cells.Add(v2.Cell);

            // Двигаем вверх
            gameField.ProcessInput(Vector2.up); 
            // Нет сдвига и нет слияния => не появляется новая плитка

            int nonNullCount = 0;
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    if (gameField.Field[x, y] != null)
                        nonNullCount++;
                }
            }
            nonNullCount.Should().Be(2, "нет слияния, нет движения => нет новой плитки");
        }

        // ========================== ДОБАВЛЕННЫЕ ТЕСТЫ ==========================

        

        [Test]
        public void ProcessInput_ShouldShiftTilesWithoutMerging()
        {
            // Пример: плитка на (0,0), а (1,0) пустая, плитки не совпадают и не сливаются
            var go1 = new GameObject("Cell1", typeof(CellView));
            var v1 = go1.GetComponent<CellView>();
            v1.Init(new Cell(new Vector2Int(0, 0), 2), dummyGrid); // значение 2^2=4
            gameField.Field[0, 0] = v1;
            gameField.cells.Add(v1.Cell);

            // в (1,0) свободно => после движения вправо плитка должна сдвинуться
            gameField.ProcessInput(Vector2.right);

            // Ожидаем, что плитка сместится в (1,0)
            gameField.Field[1, 0].Should().NotBeNull("плитка должна сдвинуться вправо");
            gameField.Field[1, 0].Cell.Value.Should().Be(2);

            // При успешном движении создаётся новая плитка => итого 2 плитки
            int nonNullCount = 0;
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    if (gameField.Field[x, y] != null)
                        nonNullCount++;
                }
            }
            nonNullCount.Should().Be(2);
        }

        [Test]
        public void ProcessInput_ShouldUpdateScoreCorrectly_AfterMerge()
        {
            // Две плитки, (0,0)->1 => 2^1=2 и (1,0)->1 => 2^1=2
            var go1 = new GameObject("Cell1", typeof(CellView));
            var v1 = go1.GetComponent<CellView>();
            v1.Init(new Cell(new Vector2Int(0, 0), 1), dummyGrid);
            gameField.Field[0, 0] = v1;
            gameField.cells.Add(v1.Cell);

            var go2 = new GameObject("Cell2", typeof(CellView));
            var v2 = go2.GetComponent<CellView>();
            v2.Init(new Cell(new Vector2Int(1, 0), 1), dummyGrid);
            gameField.Field[1, 0] = v2;
            gameField.cells.Add(v2.Cell);

            gameField.ProcessInput(Vector2.left);

            // После слияния в (0,0) => value=2 => 2^2=4 очков
            // Новая плитка тоже прибавит некоторую сумму. Но главное — проверить, что счёт не нулевой
            gameField.score.Should().BeGreaterThanOrEqualTo(4, "должен учесть новую плитку и слияние");
        }
    }
}
