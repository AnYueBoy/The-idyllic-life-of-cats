using System;
using System.Collections.Generic;

public class BinaryHeap<T> where T : IComparable<T>
{
    private readonly List<T> itemList;
    private int count;

    public BinaryHeap()
    {
        itemList = new List<T>();
        count = 0;
    }

    public void Push(T node)
    {
        itemList.Add(node);
        count++;

        int currentIndex = count - 1;
        Ascent(currentIndex);
    }

    private void Ascent(int currentIndex)
    {
        int parentIndex = currentIndex >> 1;

        while (currentIndex > 0 && itemList[parentIndex].CompareTo(itemList[currentIndex]) > 0)
        {
            SwapNode(parentIndex, currentIndex);
            currentIndex = parentIndex;
            parentIndex = currentIndex >> 1;
        }
    }

    public T Pop()
    {
        count--;
        T targetNode = itemList[0];

        itemList[0] = itemList[count];
        int current = 0;
        Sink(current);
        return targetNode;
    }

    private void Sink(int currentIndex)
    {
        while ((currentIndex << 1) + 2 <= count)
        {
            int leftChild = (currentIndex << 1) + 1;
            int rightChild = leftChild + 1;

            int minChildIndex = itemList[leftChild].CompareTo(itemList[rightChild]) > 0 ? rightChild : leftChild;
            if (itemList[minChildIndex].CompareTo(itemList[currentIndex]) > 0)
            {
                break;
            }

            SwapNode(currentIndex, minChildIndex);

            currentIndex = minChildIndex;
        }
    }

    public void UpdateHead(int index)
    {
        Ascent(index);
        Sink(index);
    }

    public void UpdateHead(T node)
    {
        if (node == null || !itemList.Contains(node))
        {
            return;
        }

        int index = itemList.IndexOf(node);
        UpdateHead(index);
    }

    public bool Contains(T node)
    {
        return itemList.Contains(node);
    }

    public void Clear()
    {
        count = 0;
        itemList.Clear();
    }

    public int Count => count;

    private void SwapNode(int index1, int index2)
    {
        var temp = itemList[index1];
        itemList[index1] = itemList[index2];
        itemList[index2] = temp;
    }
}