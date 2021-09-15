using System.Collections.Generic;

namespace GalacticWaez.Navigation
{
    // can't find a minheap/priority queue in the game files, and I don't want
    // to bring in additional dependencies
    // only reason this is remotely generic is because i haven't written the
    // pathing algo yet, so i don't know what the type will be. the ordering
    // will be based on a float cost.
    public class Minheap<ItemType>
    {
        class Node
        {
            public readonly ItemType Item;
            public readonly float Priority;
            public Node(ItemType item, float priority)
            {
                Item = item;
                Priority = priority;
            }
        }

        private readonly List<Node> nodes;

        public int Count { get => nodes.Count; }

        public Minheap()
        {
            nodes = new List<Node>();
        }

        public void Insert(ItemType item, float priority)
        {
            nodes.Add(new Node(item, priority));
            Bubble(Count - 1);
        }

        public ItemType RemoveMin()
        {
            if (Count < 1) return default;
            ItemType min = nodes[0].Item;
            int index = Count - 1;
            nodes[0] = nodes[index];
            nodes.RemoveAt(index);
            Sink();
            return min;
        }

        public void Clear()
        {
            nodes.Clear();
        }

        void Bubble(int index)
        {
            if (index > 0 && index < Count)
            {
                int parent = Parent(index);
                if (nodes[index].Priority.CompareTo(nodes[parent].Priority) < 0)
                {
                    Swap(index, parent);
                    Bubble(parent);
                }
            }
        }

        void Sink(int index = 0)
        {
            int left = Left(index);
            int right = Right(index);
            int child;

            if (left >= Count) return;
            if (right < Count
                && nodes[right].Priority.CompareTo(nodes[left].Priority) < 0)
            {
                child = right;
            }
            else child = left;

            if (nodes[index].Priority.CompareTo(nodes[child].Priority) > 0)
            {
                Swap(index, child);
                Sink(child);
            }
        }

        int Left(int index) => 2 * index + 1;
        int Right(int index) => 2 * index + 2;
        int Parent(int index) => (index + 1) / 2 - 1;

        void Swap(int a, int b)
        {
            Node temp = nodes[a];
            nodes[a] = nodes[b];
            nodes[b] = temp;
        }
    }
}
