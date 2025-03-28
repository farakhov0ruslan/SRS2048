using System;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour
{
  public GridRow[] Rows { get; private set; }
  public CellPlace[] CellPlaces { get; private set; }

  public Dictionary<Vector2Int, Transform> CellsTransforms = new();

  public int Size => CellPlaces.Length;

  public int Height => Rows.Length;

  public int Width => Size / Height;

  private void Awake()
  {
    Rows = GetComponentsInChildren<GridRow>();
    CellPlaces = GetComponentsInChildren<CellPlace>();
  }

  private void Start()
  {
    for (int y = 0; y < Rows.Length; y++)
    {
      for (int x = 0; x < Rows[y].CellPlaces.Length; x++)
      {
        Rows[y].CellPlaces[x].Position = new Vector2Int(x, y);
        CellsTransforms[new Vector2Int(x, y)] = Rows[y].CellPlaces[x].transform;
        // Debug.Log($"{new Vector2Int(x, y)}: {Rows[y].CellPlaces[x].transform.position}");
      }
    }
  }
}