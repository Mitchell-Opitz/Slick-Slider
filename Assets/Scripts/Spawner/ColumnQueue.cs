using UnityEngine;
using System.Collections.Generic;

public sealed class ColumnQueue : MonoBehaviour
{
    [SerializeField] _DummyDecisionMaker decisionMaker;
    [SerializeField] int prewarmColumns = 5;

    readonly Queue<ColumnData> queue = new();

    void Start()
    {
        for (int i = 0; i < prewarmColumns; i++)
            queue.Enqueue(decisionMaker.GenerateNextColumn());
    }

    public ColumnData GetNext()
    {
        if (queue.Count < 3)
            queue.Enqueue(decisionMaker.GenerateNextColumn());

        return queue.Dequeue();
    }

    public ColumnData PeekNext()
    {
        if (queue.Count == 0)
            queue.Enqueue(decisionMaker.GenerateNextColumn());

        return queue.Peek();
    }
}
