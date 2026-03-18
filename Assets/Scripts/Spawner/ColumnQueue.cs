using UnityEngine;
using System.Collections.Generic;
public sealed class ColumnQueue : MonoBehaviour
{
    [SerializeField] DecisionMaker decisionMaker;
    [SerializeField] int prewarmColumns = 5;
    readonly Queue<ColumnData> queue = new();
    void Start()
    {
        for (int i = 0; i < prewarmColumns; i++)
            decisionMaker.EnqueueNext(this);
    }
    public void Enqueue(ColumnData data)
    {
        queue.Enqueue(data);
    }
    public ColumnData GetNext()
    {
        if (queue.Count < 3)
            decisionMaker.EnqueueNext(this);
        return queue.Dequeue();
    }
    public ColumnData PeekNext()
    {
        if (queue.Count == 0)
            decisionMaker.EnqueueNext(this);
        if (queue.Count == 0) return default;
        return queue.Peek();
    }
}