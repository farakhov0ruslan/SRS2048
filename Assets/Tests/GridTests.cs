using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
  [TestFixture]
  public class GridTests
  {
    private GameObject gridGO;
    private Grid grid;
    private GameObject rowGO;
    private GameObject cellPlaceGO;
    
    [SetUp]
    public void SetUp() {
      // Создаем объект Grid с компонентом Grid
      gridGO = new GameObject("Grid", typeof(Grid));
      grid = gridGO.GetComponent<Grid>();
        
      // Создаем дочерний объект Row с компонентом GridRow
      rowGO = new GameObject("Row", typeof(GridRow));
      rowGO.transform.SetParent(gridGO.transform);
        
      // Создаем дочерний объект CellPlace с компонентом CellPlace
      cellPlaceGO = new GameObject("CellPlace", typeof(CellPlace));
      cellPlaceGO.transform.SetParent(rowGO.transform);
        
      // Явно вызываем Awake для GridRow, чтобы он нашёл свои дочерние CellPlace
      rowGO.GetComponent<GridRow>().Awake();
    }
    
    [TearDown]
    public void TearDown() {
      GameObject.DestroyImmediate(gridGO);
    }
    
    [Test]
    public void Start_ShouldPopulateCellsTransforms()
    {
      // Вызываем Awake и Start для Grid
      grid.Awake();
      grid.Start();
        
      // Теперь CellsTransforms должен содержать хотя бы один элемент
      grid.CellsTransforms.Should().NotBeEmpty("because at least one CellPlace exists in the scene");
    }
  }
}