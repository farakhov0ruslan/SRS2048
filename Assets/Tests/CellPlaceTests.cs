using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
  [TestFixture]
  public class CellPlaceTests
  {
    [Test]
    public void CanSetAndGetPosition()
    {
      var go = new GameObject();
      var cellPlace = go.AddComponent<CellPlace>();
      var pos = new Vector2Int(5, 6);
      cellPlace.Position = pos;
      cellPlace.Position.Should().Be(pos);
      Object.DestroyImmediate(go);
    }
  }
}