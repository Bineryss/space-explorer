using System.Collections.Generic;

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
        public int Order = 0;
        protected int currentChild;

        public Node(string name = "Node")
        {
            Name = name;
            CurrentStatus = Status.Success;
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
        
    }
    public class Leaf : Node
    {
        readonly IStrategy strategy;

        public Leaf(string name, IStrategy strategy) : base(name)
        {
            this.strategy = strategy;
        }

        public override Status Process() => strategy.Process();
        public override void Reset() => strategy.Reset();
        public override string ToString() => $"{Name} (Leaf)";
    }
}