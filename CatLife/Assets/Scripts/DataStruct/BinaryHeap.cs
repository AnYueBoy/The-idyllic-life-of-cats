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
        while ((current + 1 << 1) + 1 <= count)
        {
            int leftChild = current + 1 << 1;
            int rightChild = leftChild + 1;

            int minChildIndex = itemList[leftChild].CompareTo(itemList[rightChild]) > 0 ? rightChild : leftChild;
            if (itemList[minChildIndex].CompareTo(itemList[current]) > 0)
            {
                return targetNode;
            }

            SwapNode(current, minChildIndex);

            current = minChildIndex;
        }

        return targetNode;
    }

    public void UpdateHead(int index)
    {
        while (index > 0)
        {
            int parent = index >> 1;
            if (itemList[index].CompareTo(itemList[parent]) > 0)
            {
                break;
            }

            SwapNode(index, parent);
            index = parent;
        }
    }

    private void SwapNode(int index1, int index2)
    {
        var temp = itemList[index1];
        itemList[index1] = itemList[index2];
        itemList[index2] = temp;
    }
}