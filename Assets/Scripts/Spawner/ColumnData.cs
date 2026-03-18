using UnityEngine;

public struct ColumnData
{
    public int columnIndex;
    public GameObject[] cells;

    public ColumnData(int index, int height)
    {
        columnIndex = index;
        cells = new GameObject[height];
    }
}
