using FluentAssertions;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    [TestFixture]
    public class CellViewTests
    {
        private GameObject _cellViewGo;
        private CellView _cellView;
        private GameObject _dummyGridGo;
        private Grid _dummyGrid;
        private TextMeshProUGUI _tmpText;
        private Image _image;

        [SetUp]
        public void SetUp() {
            // Создаем объект для CellView
            _cellViewGo = new GameObject("CellView", typeof(CellView));
            // Добавляем дочерний компонент TextMeshProUGUI (TMP_Text абстрактный, поэтому используем конкретную реализацию)
            var textGo = new GameObject("Text", typeof(TextMeshProUGUI));
            textGo.transform.SetParent(_cellViewGo.transform);
            _tmpText = textGo.GetComponent<TextMeshProUGUI>();

            // Добавляем компонент Image на тот же объект
            _image = _cellViewGo.AddComponent<Image>();
            _cellView = _cellViewGo.GetComponent<CellView>();

            // Создаем фиктивный Grid и заполняем CellsTransforms
            _dummyGridGo = new GameObject("DummyGrid", typeof(Grid));
            _dummyGrid = _dummyGridGo.GetComponent<Grid>();
            _dummyGrid.CellsTransforms = new System.Collections.Generic.Dictionary<Vector2Int, Transform>();
            var dummyCellPlace = new GameObject("CellPlace").transform;
            _dummyGrid.CellsTransforms[new Vector2Int(0, 0)] = dummyCellPlace;
        }

        [TearDown]
        public void TearDown() {
            Object.DestroyImmediate(_cellViewGo);
            Object.DestroyImmediate(_dummyGridGo);
        }

        [Test]
        public void Init_ShouldSetTextAndColor()
        {
            // Создаем клетку с начальным значением 1 (отображается как 2^1 = 2)
            var cell = new Cell(new Vector2Int(0, 0), 1);
            _cellView.Init(cell, _dummyGrid);

            // Проверяем, что текст установлен верно
            _tmpText.text.Should().Be("2");

            // Ожидаемый цвет вычисляется через Lerp(startColor, endColor, t)
            // Для newValue == 1 => t == 0, поэтому ожидаем startColor.
            var startColorField = typeof(CellView)
                .GetField("startColor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            startColorField.Should().NotBeNull();
            Color expectedColor = (Color)startColorField.GetValue(_cellView);

            // Проверяем компоненты цвета по отдельности
            _image.color.r.Should().BeApproximately(expectedColor.r, 0.001f);
            _image.color.g.Should().BeApproximately(expectedColor.g, 0.001f);
            _image.color.b.Should().BeApproximately(expectedColor.b, 0.001f);
            _image.color.a.Should().BeApproximately(expectedColor.a, 0.001f);
        }
    }
}
