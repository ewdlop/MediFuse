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

    /// <summary>
    /// Pre-order Traversal: Root -> Left -> Right
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<T?> Preorder<T>(this INode<T>? node)
    {
        if (node is null) yield break;
        yield return node.Value;
        for(int i = 0; i < node.Nodes.Length; i++)
        {
            foreach (T? val in Preorder<T>(node.Nodes[i])) yield return val;
        }
    }

    /// <summary>
    /// In-order Traversal: Left -> Root(mid) -> Right
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<T?> InOrder<T>(this INode<T>? node)
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

    /// <summary>
    /// Post-order Traversal: Children -> Root
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<T?> PostOrder<T>(this INode<T>? node)
    {
        if (node is null) yield break;
        for (int i = 0; i < node.Nodes.Length; i++)
        {
            foreach (T? val in PostOrder(node.Nodes[i])) yield return val;
        }
        yield return node.Value;
    }
    
    /// <summary>
    /// Depth First Search (DFS) Traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<T?> DepthFirstSearch<T>(this INode<T>? node)
    {
        if (node is null) yield break;
        Stack<INode<T>> stack = new();
        stack.Push(node);
        while (stack.Count > 0)
        {
            INode<T> current = stack.Pop();
            yield return current.Value;
            foreach (INode<T> child in current.Nodes)
            {
                stack.Push(child);
            }
        }
    }

    /// <summary>
    /// Breadth First Search (BFS) Traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<T?> BreadthFirstSearch<T>(this INode<T>? node)
    {
        if (node is null) yield break;
        Queue<INode<T>> queue = new();
        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            INode<T> current = queue.Dequeue();
            yield return current.Value;
            foreach (INode<T> child in current.Nodes)
            {
                queue.Enqueue(child);
            }
        }
    }

    /// <summary>
    /// Depth Limited Search (DLS) Traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    private static IEnumerable<T?> DepthLimitedSearch<T>(INode<T>? node, int? depth = null)
    {
        if (node is null) yield break;
        if (depth == 0)
        {
            yield return node.Value;
        }
        else if (depth > 0)
        {
            foreach (INode<T> child in node?.Nodes?? Enumerable.Empty<INode<T>>())
            {
                foreach (T? val in DepthLimitedSearch(child, depth - 1))
                {
                    yield return val;
                }
            }
        }
        else if(depth is null)
        {
            yield return node.Value;
            foreach (INode<T> child in node?.Nodes ?? Enumerable.Empty<INode<T>>())
            {
                foreach (T? val in DepthLimitedSearch(child, depth))
                {
                    yield return val;
                }
            }
        }
        else if (depth < 0)
        {
            yield break;
        }
    }

    /// <summary>
    /// Iterative Deepening Depth First Search (IDDFS) Traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<T?> IterativeDeepeningDepthFirstSearch<T>(this INode<T>? node)
    {
        if (node is null) yield break;
        int depth = 0;
        while (true)
        {
            bool hasNeighbors = false; // Flag to check if there are neighbors at the current depth level or not
            foreach (T? val in DepthLimitedSearch(node, depth))
            {
                hasNeighbors = true;
                yield return val;
            }
            // If there are no neighbors at the current depth level, break the loop, else increment the depth.
            // This is the exit condition for the loop
            // Else, the loop will continue to run indefinitely
            if (!hasNeighbors) break;
            depth++;
        }
    }

    /// <summary>
    /// Generic traversal function
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="node"></param>
    /// <param name="traversalFunc"></param>
    /// <returns></returns>
    public static IEnumerable<TResult?> Traverse<T, TResult>(
        this INode<T> node,
        Func<INode<T>, IEnumerable<TResult>> traversalFunc)
    {
        return traversalFunc(node);
    }

    /// <summary>
    /// Pre-order traversal
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<TResult?> PreOrderTraverse<TResult>(
        this INode<TResult?> node)
    {
        return Preorder(node);
    }

    /// <summary>
    /// In-order traversal
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<TResult?> InOrderTraverse<TResult>(
        this INode<TResult?> node)
    {
        return InOrder(node);
    }


    /// <summary>
    /// Post-order traversal
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<TResult?> PostOrderTraverse<TResult>(
        this INode<TResult?> node)
    {
        return PostOrder(node);
    }

    /// <summary>
    /// Depth First Search traversal
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<TResult?> DepthFirstSearchTraverse<TResult>(
        this INode<TResult?> node)
    {
        return DepthFirstSearch(node);
    }


    /// <summary>
    /// Breadth First Search traversal
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<TResult?> BreathFirstSearchTraverse<TResult>(
        this INode<TResult?> node)
    {
        return BreadthFirstSearch(node);
    }


    /// <summary>
    /// Depth Limited Search traversal
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="node"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public static IEnumerable<TResult?> DepthLimitedSearchTraverse<TResult>(
        this INode<TResult?> node, int? depth = null)
    {
        return DepthLimitedSearch(node, depth);
    }


    /// <summary>
    /// Iterative Deepening Depth First Search traversal
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<TResult?> IterativeDeepeningDepthFirstSearchTraverse<TResult>(this INode<TResult?> node)
    {
        return IterativeDeepeningDepthFirstSearch(node);
    }
}