using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
  [TestFixture]
  public class GridRowTests
  {
    private GameObject rowGO;
    private GridRow gridRow;
    
    [SetUp]
    public void SetUp() {
      rowGO = new GameObject("GridRow", typeof(GridRow));
      gridRow = rowGO.GetComponent<GridRow>();
      var cpGO = new GameObject("CellPlace", typeof(CellPlace));
      cpGO.transform.SetParent(rowGO.transform);
    }
    
    [TearDown]
    public void TearDown() {
      GameObject.DestroyImmediate(rowGO);
    }
    
    [Test]
    public void Awake_ShouldPopulateCellPlaces()
    {
      gridRow.Awake();
      gridRow.CellPlaces.Should().NotBeEmpty();
    }
  }
}