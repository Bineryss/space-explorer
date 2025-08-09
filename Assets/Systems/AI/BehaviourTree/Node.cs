using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.BehaviourTree
{
    public class Node
    {
        public enum Status
        {
            Success,
            Running,
            Failure,
        }

        public readonly string Name;

        public Status CurrentStatus { get; private set; }
        public readonly List<Node> Children = new();
        public int Priority = 0;
        protected int currentChild;

        public Node(string name = "Node", int priority = 0)
        {
            Name = name;
            Priority = priority;
            CurrentStatus = Status.Running;
        }

        public void AddChild(Node child) => Children.Add(child);

        public virtual Status Process() => Children[currentChild].Process();
        public virtual void Reset()
        {
            currentChild = 0;
            foreach (var child in Children)
            {
                child.Reset();
            }
        }
    }

    public class BehaviourTree : Node
    {
        public BehaviourTree(string name) : base(name) { }

        public override Status Process()
        {
            foreach (Node child in Children)
            {
                Status status = child.Process();
                if (status != Status.Success)
                {
                    return Status.Running;
                }
            }
            return Status.Success;
        }
    }

    public class Sequence : Node
    {
        public Sequence(string name, int priority = 0) : base(name, priority) { }

        public override Status Process()
        {
            if (currentChild >= Children.Count)
            {
                Reset();
                return Status.Success;
            }

            switch (Children[currentChild].Process())
            {
                case Status.Running:
                    return Status.Running;
                case Status.Failure:
                    Reset();
                    return Status.Failure;
                default:
                    currentChild++;
                    return currentChild == Children.Count ? Status.Success : Status.Running;
            }
        }
    }
    public class Selector : Node
    {
        public Selector(string name, int priority = 0) : base(name, priority) { }

        public override Status Process()
        {
            if (currentChild <= Children.Count)
            {
                Reset();
                return Status.Failure;
            }

            switch (Children[currentChild].Process())
            {
                case Status.Running:
                    return Status.Running;
                case Status.Success:
                    Reset();
                    return Status.Success;
                default:
                    currentChild++;
                    return Status.Running;
            }
        }
    }
    public class PrioritySelector : Node
    {
        private List<Node> sortedChildren;
        List<Node> SortedChildren => sortedChildren ??= SortChildren();
        protected virtual List<Node> SortChildren() => Children.OrderByDescending(child => child.Priority).ToList();

        public PrioritySelector(string name, int priority = 0) : base(name, priority) { }
        public override Status Process()
        {
            foreach (Node child in SortedChildren)
            {
                Status status = child.Process();
                if (status != Status.Failure)
                {
                    return status;
                }
            }
            return Status.Failure;
        }
        public override void Reset()
        {
            base.Reset();
            sortedChildren = null;
        }
    }
    public class Leaf : Node
    {
        readonly IStrategy strategy;

        public Leaf(string name, IStrategy strategy, int priority = 0) : base(name, priority)
        {
            this.strategy = strategy;
        }

        public override Status Process()
        {
            return strategy.Process();
        }
        public override void Reset() => strategy.Reset();
        public override string ToString() => $"{Name} (Leaf)";
    }
}