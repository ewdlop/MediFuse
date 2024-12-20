namespace MediFuseConsoleApp1;

public static partial class Tree
{
    public interface IReadOnlyNode<T>
    {
        T? Value { get; }
        IReadOnlyList<IReadOnlyNode<T>> AsReadOnly { get; }
    }

    public interface INode<T> : IReadOnlyNode<T>
    {
        INode<T>[] Nodes { get; }
    }

    public interface ITrieNode<T> : INode<T>
    {
        bool IsEndOfWord { get; }
    }

    public class Node<T>(T? Value, Node<T>[] Nodes) : INode<T>
    {
        public T? Value { get; set; } = Value;
        public INode<T>[] Nodes { get; set; } = Nodes;

        INode<T>[] INode<T>.Nodes => Nodes;

        T? IReadOnlyNode<T>.Value => Value;

        IReadOnlyList<IReadOnlyNode<T>> IReadOnlyNode<T>.AsReadOnly => Nodes.AsReadOnly();
    }

    public class TrieNode<T>(T? Value, Node<T>[] Nodes, bool IsEndOfWord) : Node<T>(Value, Nodes)
    {
        public bool IsEndOfWord { get; set; } = IsEndOfWord;
    }

    // Preorder Traversal: Root -> Left -> Right
    public static IEnumerable<T?> Preorder<T>(this INode<T>? node)
    {
        if (node is null) yield break;
        yield return node.Value;
        for(int i = 0; i < node.Nodes.Length; i++)
        {
            foreach (T? val in Preorder<T>(node.Nodes[i])) yield return val;
        }
    }

    // Inorder Traversal: Left -> Root(mid) -> Right
    public static IEnumerable<T?> Inorder<T>(this INode<T>? node)
    {
        if (node is null) yield break;
        int mid = node.Nodes.Length / 2;
        for (int i = 0; i < mid; i++)
        {
            foreach (T? val in Preorder(node.Nodes[i])) yield return val;

        }
        yield return node.Value;
        for (int i = mid; i < node.Nodes.Length; i++)
        {
            foreach (T? val in Preorder(node.Nodes[i])) yield return val;
        }
    }

    // Postorder Traversal: Children -> Root
    public static IEnumerable<T?> Postorder<T>(this INode<T>? node)
    {
        if (node is null) yield break;
        for (int i = 0; i < node.Nodes.Length; i++)
        {
            foreach (T? val in Postorder(node.Nodes[i])) yield return val;
        }
        yield return node.Value;
    }
    
    public static IEnumerable<TResult?> Traverse<T, TResult>(
        this INode<T> node,
        Func<INode<T>, IEnumerable<TResult>> traversalFunc)
    {
        return traversalFunc(node);
    }

    public static IEnumerable<TResult?> PreOrderTraverse<TResult>(
        this INode<TResult?> node)
    {
        return Preorder(node);
    }

    public static IEnumerable<TResult?> InOrderTraverse<TResult>(
        this INode<TResult?> node)
    {
        return Inorder(node);
    }

    public static IEnumerable<TResult?> PostOrderTraverse<TResult>(
        this INode<TResult?> node)
    {
        return Postorder(node);
    }
}