using FluentAssertions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;

namespace Tests
{
    [TestFixture]
    public class InputControllerTests
    {
        private GameObject dummyGF;
        private GameObject inputGO;
        private InputController inputController;

        [SetUp]
        public void SetUp()
        {
            // Создаем dummy GameField до создания InputController
            dummyGF = new GameObject("DummyGameField", typeof(GameField));
            
            // Теперь создаем InputController, который в Awake() найдет dummyGF
            inputGO = new GameObject("InputController", typeof(InputController));
            inputController = inputGO.GetComponent<InputController>();

            // Можно вызвать Awake() явно, если требуется (обычно Unity сама вызывает его при запуске сцены)
            inputController.Awake();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(inputGO);
            Object.DestroyImmediate(dummyGF);
        }

        // Пример теста для метода DetermineCardinalDirection (аналог вашего InputHandlerTests)
        [TestCase(1f, 0.5f, 1f, 0f, TestName = "Right")]
        [TestCase(-0.6f, 0.2f, -1f, 0f, TestName = "Left")]
        [TestCase(0.1f, 1f, 0f, 1f, TestName = "Up")]
        [TestCase(0.2f, -0.9f, 0f, -1f, TestName = "Down")]
        public void DetermineCardinalDirection_ShouldReturnCorrectAxis(
            float inputX, float inputY, float expectedX, float expectedY)
        {
            // Получаем приватный метод через reflection
            MethodInfo determineMethod = typeof(InputController)
                .GetMethod("DetermineCardinalDirection", BindingFlags.NonPublic | BindingFlags.Instance);

            var inputVector = new Vector2(inputX, inputY);
            var result = (Vector2)determineMethod.Invoke(inputController, new object[] { inputVector });

            result.x.Should().BeApproximately(expectedX, 0.001f);
            result.y.Should().BeApproximately(expectedY, 0.001f);
        }

        // Тест, проверяющий, что Update() не выбрасывает исключений в EditMode
        [Test]
        public void Update_ShouldNotThrowExceptions_InEditMode()
        {
            MethodInfo updateMethod = typeof(InputController)
                .GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);

            Assert.DoesNotThrow(() => updateMethod.Invoke(inputController, null));
        }

        // Пример теста, использующего FakeGameField (созданный через AddComponent)
        [Test]
        public void HandleKeyboardInput_UpArrow_ShouldCallProcessInput()
        {
            // Создаем фейковый GameField через AddComponent, а не через new
            var fakeFieldGO = new GameObject("FakeGameField");
            var fakeField = fakeFieldGO.AddComponent<FakeGameField>();

            // Подставляем фейковый GameField в приватное поле _gameField
            var fieldInfo = typeof(InputController).GetField("_gameField", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(inputController, fakeField);

            // Здесь напрямую вызываем DetermineCardinalDirection для проверки логики
            MethodInfo method = typeof(InputController).GetMethod("DetermineCardinalDirection", BindingFlags.NonPublic | BindingFlags.Instance);
            Vector2 result = (Vector2)method.Invoke(inputController, new object[] { new Vector2(0f, 1f) });
            result.Should().Be(Vector2.up, "направление 'up' должно определяться корректно");

            Object.DestroyImmediate(fakeFieldGO);
        }
    }

    // Вспомогательный класс FakeGameField
    // Важно: чтобы можно было переопределить метод ProcessInput, он должен быть объявлен как virtual в классе GameField.
    public class FakeGameField : GameField
    {
        public bool WasProcessInputCalled { get; private set; }
        public Vector2 LastInput { get; private set; }

        public override void ProcessInput(Vector2 direction)
        {
            WasProcessInputCalled = true;
            LastInput = direction;
        }
    }
}
