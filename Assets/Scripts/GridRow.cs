using UnityEngine;


public class GridRow : MonoBehaviour
{
  public CellPlace[] CellPlaces { get; private set; }


  private void Awake()
  {
    CellPlaces = GetComponentsInChildren<CellPlace>();
    Debug.Log($"Получил компоненты CEllPlace {CellPlaces.Length}");
  }
}