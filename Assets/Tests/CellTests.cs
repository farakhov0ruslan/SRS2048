using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
  [TestFixture]
  public class CellTests
  {
    [Test]
    public void Constructor_ShouldInitializeProperties()
    {
      var pos = new Vector2Int(1, 2);
      int initialValue = 1;
      var cell = new Cell(pos, initialValue);
      cell.Position.Should().Be(pos);
      cell.Value.Should().Be(initialValue);
    }

    [Test]
    public void SettingPosition_ShouldInvokeOnPositionChanged()
    {
      var cell = new Cell(new Vector2Int(0, 0), 1);
      var newPos = new Vector2Int(3, 4);
      bool eventFired = false;
      cell.OnPositionChanged += (pos) =>
      {
        pos.Should().Be(newPos);
        eventFired = true;
      };
      cell.Position = newPos;
      eventFired.Should().BeTrue();
    }

    [Test]
    public void SettingValue_ShouldInvokeOnValueChanged()
    {
      var cell = new Cell(new Vector2Int(0, 0), 1);
      int newValue = 2;
      bool eventFired = false;
      cell.OnValueChanged += (val) =>
      {
        val.Should().Be(newValue);
        eventFired = true;
      };
      cell.Value = newValue;
      eventFired.Should().BeTrue();
    }
  }
}