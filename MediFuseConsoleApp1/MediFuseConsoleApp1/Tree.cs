﻿using System.Collections;
using static MediFuseConsoleApp1.Tree;

namespace MediFuseConsoleApp1;

public static partial class Tree
{
    //public interface IDebugNode<T>
    //{
    //    IDebugNode<T> Debug { get; }
    //}


    public interface IValue<T>
    {
        abstract T? Value { get; }
    }

    public interface IReadOnlyNode<T> : IValue<T>
    {
        abstract IReadOnlyNode<T?> AsReadOnlyNode { get; }
    }

    public interface IReadOnlyNodeNeighborHood<T>
    {
        abstract IReadOnlyList<INode<T?>?> AsReadOnlyNodeList { get; }
        abstract IReadOnlyList<IReadOnlyNode<T?>?> AsReadOnlyReadOnlyNodeList { get; }
        abstract IReadOnlyNode<T?>?[] AsReadOnlyNodes { get; }
    }

    public interface INodeNeighborHood<T> : IReadOnlyNodeNeighborHood<T>
    {
        abstract INode<T?>?[] Nodes { get; }
    }

    public interface INodeNeighbor<T> : IReadOnlyNode<T>, IReadOnlyNodeNeighborHood<T> { }

    public interface INode<T> : INodeNeighborHood<T>, INodeNeighbor<T>, IEnumerable<INode<T?>?>, IReadOnlyNode<T> { }

    public interface ITrieNode<T> : INode<T?>
    {
        abstract bool IsEndOfWord { get; }
    }

    public class Node<T>(T? Value, Node<T?>?[] Nodes) : INode<T?>
    {

        public static readonly Lazy<INode<T?>> DebugNode = new(() => new Node<T?>(default, []));

        public virtual INode<T?>?[] Nodes { get; set; } = Nodes;

        public virtual IReadOnlyNode<T?> AsReadOnlyNode => this;

        public virtual T? Value { get; set; } = Value;

        public virtual IReadOnlyList<INode<T?>?> AsReadOnlyNodeList => Nodes;

        public virtual IReadOnlyList<IReadOnlyNode<T?>?> AsReadOnlyReadOnlyNodeList => Nodes;

        public virtual IReadOnlyNode<T?>?[] AsReadOnlyNodes => Nodes;

        public virtual IEnumerator<INode<T?>?> GetEnumerator()
        {
            yield return this;
            foreach (INode<T?>? node in Nodes)
            {
                yield return node;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class TrieNode<T>(T? Value, Node<T?>[] Nodes, bool IsEndOfWord) : Node<T?>(Value, Nodes)
    {
        public virtual bool IsEndOfWord { get; set; } = IsEndOfWord;
    }


    /// <summary>
    /// Pre-order Traversal: Root -> Left -> Right
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>> Preorder<T>(this INode<T?>? node)
    {
        if (node is null) yield break;
        yield return node;
        foreach(INode<T?>? val in node?.Nodes.SelectMany(child => Preorder(child)) ?? [])
        {
            yield return val;
        }
    }

    /// <summary>
    /// In-order Traversal: Left -> Root(mid) -> Right
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> InOrder<T>(this INode<T?>? node)
    {
        if (node is null) yield break;
        int mid = node.Nodes.Length / 2;
        foreach (INode<T?>? val in Enumerable.Range(0, mid).SelectMany(i => InOrder(node.Nodes[i]))) yield return val;
        yield return node;
        foreach (INode<T?>? val in Enumerable.Range(mid, node.Nodes.Length - mid).SelectMany(i => InOrder(node.Nodes[i]))) yield return val;
    }

    /// <summary>
    /// Post-order Traversal: Children -> Root
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> PostOrder<T>(this INode<T?>? node)
    {
        if (node is null) yield break;
        foreach (INode<T?>? val in node?.Nodes.SelectMany(child => PostOrder(child)) ?? [])
        {
            yield return val;
        }
        yield return node;
    }

    /// <summary>
    /// Depth First Search (DFS) Traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> DepthFirstSearch<T>(this INode<T?>? node,
        Func<INode<T?>?, int>? orderByFunc = null)
    {
        if (node is null) yield break;
        Stack<INode<T?>?> stack = new();
        stack.Push(node);
        while (stack.Count > 0)
        {
            INode<T?>? current = stack.Pop();
            if(current is null) continue;
            yield return current;
            IEnumerable<INode<T?>?> orderedNodes = orderByFunc is null ?
                current.Nodes.Reverse() :
                current.Nodes.OrderBy(orderByFunc);
            foreach (INode<T?>? child in orderedNodes)
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
    public static IEnumerable<INode<T?>?> BreadthFirstSearch<T>(this INode<T?>? node)
    {
        if (node is null) yield break;
        Queue<INode<T?>?> queue = new();
        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            INode<T?>? current = queue.Dequeue();
            if (current is null) continue;
            yield return current;
            foreach (INode<T?>? child in current.Nodes)
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
    public static IEnumerable<INode<T?>?> DepthLimitedSearch<T>(INode<T?>? node, int? depth = null)
    {
        if (node is null) yield break;
        if (depth == 0)
        {
            yield return node;
        }
        else if (depth > 0)
        {
            foreach (INode<T?>? val in (node?.Nodes ?? Enumerable.Empty<INode<T?>?>()).SelectMany(child => DepthLimitedSearch(child, depth - 1)))
            {
                yield return val;
            }
        }
        else if (depth is null)
        {
            yield return node;
            foreach (INode<T?>? val in (node?.Nodes ?? Enumerable.Empty<INode<T?>?>()).SelectMany(child => DepthLimitedSearch(child, depth)))
            {
                yield return val;
            }
        }
        else if (depth < 0)
        {


            // Do nothing
            // Go Complex Plane
            // Contact the Doctor Dhir Patel
            // Or the Doctor Strange
            // Or the Doctor Who
            // Or the Doctor Doom
            // Or the Doctor Octopus
            // Or the Doctor Manhattan
            // Or the Doctor Jekyll
            // Or the Doctor Faustus
            // Or the Doctor Frankenstein
            // Or the Doctor Moreau
            // Or the Doctor Dolittle
            // Or the Doctor Zhivago

            //using(var doctor = new Doctor())
            //{
            //    doctor.ContactDoctorDhirPatel();
            //    doctor.ContactDoctorStrange();
            //    doctor.ContactDoctorWho();
            //    doctor.ContactDoctorDoom();
            //    doctor.ContactDoctorOctopus();
            //    doctor.ContactDoctorManhattan();
            //    doctor.ContactDoctorJekyll();
            //    doctor.ContactDoctorFaustus();
            //    doctor.ContactDoctorFrankenstein();
            //    doctor.ContactDoctorMoreau();
            //    doctor.ContactDoctorDolittle();
            //    doctor.ContactDoctorZhivago();
            //}

            //TODO : sent a message to the doctor
            //smpt and pop3 
            //send and receive
            //email and password
            //from and to
            //subject and body
            //attachment and attachment

            //https://www.codeproject.com/Articles/10649/Sending-Emails-in-NET-3-5-using-SMTP-and-POP3
            //twilio
            //sendgrid
            //smpt
            //pop3server
            //email
            //Rebex

            //send message to boss or your self
            //yield break;

            {

                //new Node<T>(default(T), []);
                //yield break;
            }
            yield return Node<T?>.DebugNode.Value;
        }
    }

    /// <summary>
    /// Iterative Deepening Depth First Search (IDDFS) Traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> IterativeDeepeningDepthFirstSearch<T>(this INode<T?>? node)
    {
        if (node is null) yield break;
        int depth = 0;
        while (true)
        {
            bool hasNeighbors = false; // Flag to check if there are neighbors at the current depth level or not
            foreach (INode<T?>? val in DepthLimitedSearch(node, depth))
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
    /// <param name="node"></param>
    /// <param name="traversalFunc"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> Traverse<T>(
        this INode<T?>? node,
        Func<INode<T?>?, IEnumerable<INode<T?>?>> traversalFunc)
    {
        return traversalFunc(node);
    }

    /// <summary>
    /// Pre-order traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> PreOrderTraverse<T>(
        this INode<T?>? node)
    {
        return Preorder(node);
    }

    /// <summary>
    /// In-order traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> InOrderTraverse<T>(
        this INode<T?>? node)
    {
        return InOrder(node);
    }


    /// <summary>
    /// Post-order traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> PostOrderTraverse<T>(
        this INode<T?>? node)
    {
        return PostOrder(node);
    }

    /// <summary>
    /// Depth First Search traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> DepthFirstSearchTraverse<T>(
        this INode<T?>? node)
    {
        return DepthFirstSearch(node);
    }


    /// <summary>
    /// Breadth First Search traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> BreathFirstSearchTraverse<T>(
        this INode<T?>? node)
    {
        return BreadthFirstSearch(node);
    }


    /// <summary>
    /// Depth Limited Search traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> DepthLimitedSearchTraverse<T>(
        this INode<T?>? node, int? depth = null)
    {
        return DepthLimitedSearch(node, depth);
    }


    /// <summary>
    /// Iterative Deepening Depth First Search traversal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode<T?>?> IterativeDeepeningDepthFirstSearchTraverse<T>(this INode<T?>? node)
    {
        return IterativeDeepeningDepthFirstSearch(node);
    }
}

//public static partial class Tree
//{

//    public interface IReadOnlyReadOnlyNeighborNode<T1,T2>
//    {
//        IReadOnlyList<IReadOnlyNode<T1, T2>> AsReadOnlyNodeList { get; }
//    }

//    public interface IReadOnlyNeighbor<T1,T2> : IReadOnlyReadOnlyNeighborNode<T1, T2>
//    {
//        IReadOnlyList<INode<T1,T2>> AsReadOnlyList { get; }
//        IReadOnlyNeighbor<T1, T2> AsReadOnly { get; }
//    }

//    public interface IValue<T1,T2>
//    {
//        T Value { get; }
//    }

//    public interface IReadOnlyNode<T1, T2> : IValue<T1>, IReadOnlyNeighbor<T1, T2>
//    {
//        IReadOnlyNode<T1, T2> AsReadOnlyNode { get; }
//    }

//    public interface INeighbor<T1, T2> : IReadOnlyNeighbor<T1, T2>
//    {
//        INode<T1, T2>[] Nodes { get; }
//    }

//    public interface INode<T1,T2> : INeighbor<T1,T2>, IReadOnlyNode<T1,T2>;

//    public interface ITrieNode<T1,T2> : INode<T1,T2>
//    {
//        bool IsEndOfWord { get; }
//    }

//    public class Node<T1,T2>(T1? Value, Node<T1?,T2?>[] Nodes) : INode<T1?,T2?>
//    {
//        public T1? Value { get; set; } = Value;
//        public INode<T1?,T2?>[] Nodes { get; set; } = Nodes;

//        public IReadOnlyNode<T1?, T2?> AsReadOnlyNode => this;

//        public IReadOnlyList<INode<T1?, T2?>> AsReadOnlyList => AsReadOnlyList;

//        public IReadOnlyNeighbor<T1?, T2?> AsReadOnly => AsReadOnly;

//        public IReadOnlyList<IReadOnlyNode<T1?, T2?>> AsReadOnlyNodeList => AsReadOnlyNodeList;
//    }

//    public class TrieNode<T1,T2>(T1? Value, Node<T1?,T2?>[] Nodes, bool IsEndOfWord) : Node<T1,T2>(Value, Nodes)
//    {
//        public bool IsEndOfWord { get; set; } = IsEndOfWord;
//    }

//    /// <summary>
//    /// Pre-order Traversal: Root -> Left -> Right
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<> Preorder<T1,T2>(this INode<T1,T2>? node)
//    {
//        if (node is null) yield break;
//        yield return node.Value;
//        for(int i = 0; i < node.Nodes.Length; i++)
//        {
//            foreach (T? val in Preorder<T>(node.Nodes[i])) yield return val;
//        }
//    }

//    /// <summary>
//    /// In-order Traversal: Left -> Root(mid) -> Right
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<T?> InOrder<T>(this INode<T?>? node)
//    {
//        if (node is null) yield break;
//        int mid = node.Nodes.Length / 2;
//        for (int i = 0; i < mid; i++)
//        {
//            foreach (T? val in Preorder(node.Nodes[i])) yield return val;

//        }
//        yield return node.Value;
//        for (int i = mid; i < node.Nodes.Length; i++)
//        {
//            foreach (T? val in Preorder(node.Nodes[i])) yield return val;
//        }
//    }

//    /// <summary>
//    /// Post-order Traversal: Children -> Root
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<T?> PostOrder<T>(this INode<T?>? node)
//    {
//        if (node is null) yield break;
//        for (int i = 0; i < node.Nodes.Length; i++)
//        {
//            foreach (T? val in PostOrder(node.Nodes[i])) yield return val;
//        }
//        yield return node.Value;
//    }
    
//    /// <summary>
//    /// Depth First Search (DFS) Traversal
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<T?> DepthFirstSearch<T>(this INode<T?>? node)
//    {
//        if (node is null) yield break;
//        Stack<INode<T?>> stack = new();
//        stack.Push(node);
//        while (stack.Count > 0)
//        {
//            INode<T?> current = stack.Pop();
//            yield return current.Value;
//            foreach (INode<T?> child in current.Nodes)
//            {
//                stack.Push(child);
//            }
//        }
//    }

//    /// <summary>
//    /// Breadth First Search (BFS) Traversal
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<T?> BreadthFirstSearch<T>(this INode<T?>? node)
//    {
//        if (node is null) yield break;
//        Queue<INode<T?>> queue = new();
//        queue.Enqueue(node);
//        while (queue.Count > 0)
//        {
//            INode<T?> current = queue.Dequeue();
//            yield return current.Value;
//            foreach (INode<T?> child in current.Nodes)
//            {
//                queue.Enqueue(child);
//            }
//        }
//    }

//    /// <summary>
//    /// Depth Limited Search (DLS) Traversal
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="node"></param>
//    /// <param name="depth"></param>
//    /// <returns></returns>
//    public static IEnumerable<T?> DepthLimitedSearch<T>(INode<T?>? node, int? depth = null)
//    {
//        if (node is null) yield break;
//        if (depth == 0)
//        {
//            yield return node.Value;
//        }
//        else if (depth > 0)
//        {
//            foreach (INode<T?> child in node?.Nodes?? Enumerable.Empty<INode<T?>>())
//            {
//                foreach (T? val in DepthLimitedSearch(child, depth - 1))
//                {
//                    yield return val;
//                }
//            }
//        }
//        else if(depth is null)
//        {
//            yield return node.Value;
//            foreach (INode<T?> child in node?.Nodes ?? Enumerable.Empty<INode<T?>>())
//            {
//                foreach (T? val in DepthLimitedSearch(child, depth))
//                {
//                    yield return val;
//                }
//            }
//        }
//        else if (depth < 0)
//        {
//            yield break;
//        }
//    }

//    /// <summary>
//    /// Iterative Deepening Depth First Search (IDDFS) Traversal
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<T?> IterativeDeepeningDepthFirstSearch<T>(this INode<T?>? node)
//    {
//        if (node is null) yield break;
//        int depth = 0;
//        while (true)
//        {
//            bool hasNeighbors = false; // Flag to check if there are neighbors at the current depth level or not
//            foreach (T? val in DepthLimitedSearch(node, depth))
//            {
//                hasNeighbors = true;
//                yield return val;
//            }
//            // If there are no neighbors at the current depth level, break the loop, else increment the depth.
//            // This is the exit condition for the loop
//            // Else, the loop will continue to run indefinitely
//            if (!hasNeighbors) break;
//            depth++;
//        }
//    }

//    /// <summary>
//    /// Generic traversal function
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <typeparam name="TResult"></typeparam>
//    /// <param name="node"></param>
//    /// <param name="traversalFunc"></param>
//    /// <returns></returns>
//    public static IEnumerable<TResult?> Traverse<T, TResult>(
//        this INode<T?>? node,
//        Func<INode<T?>?, IEnumerable<TResult?>> traversalFunc)
//    {
//        return traversalFunc(node);
//    }

//    /// <summary>
//    /// Pre-order traversal
//    /// </summary>
//    /// <typeparam name="TResult"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<TResult?> PreOrderTraverse<TResult>(
//        this INode<TResult?>? node)
//    {
//        return Preorder(node);
//    }

//    /// <summary>
//    /// In-order traversal
//    /// </summary>
//    /// <typeparam name="TResult"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<TResult?> InOrderTraverse<TResult>(
//        this INode<TResult?>? node)
//    {
//        return InOrder(node);
//    }


//    /// <summary>
//    /// Post-order traversal
//    /// </summary>
//    /// <typeparam name="TResult"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<TResult?> PostOrderTraverse<TResult>(
//        this INode<TResult?>? node)
//    {
//        return PostOrder(node);
//    }

//    /// <summary>
//    /// Depth First Search traversal
//    /// </summary>
//    /// <typeparam name="TResult"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<TResult?> DepthFirstSearchTraverse<TResult>(
//        this INode<TResult?>? node)
//    {
//        return DepthFirstSearch(node);
//    }


//    /// <summary>
//    /// Breadth First Search traversal
//    /// </summary>
//    /// <typeparam name="TResult"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<TResult?> BreathFirstSearchTraverse<TResult>(
//        this INode<TResult?>? node)
//    {
//        return BreadthFirstSearch(node);
//    }


//    /// <summary>
//    /// Depth Limited Search traversal
//    /// </summary>
//    /// <typeparam name="TResult"></typeparam>
//    /// <param name="node"></param>
//    /// <param name="depth"></param>
//    /// <returns></returns>
//    public static IEnumerable<TResult?> DepthLimitedSearchTraverse<TResult>(
//        this INode<TResult?>? node, int? depth = null)
//    {
//        return DepthLimitedSearch(node, depth);
//    }


//    /// <summary>
//    /// Iterative Deepening Depth First Search traversal
//    /// </summary>
//    /// <typeparam name="TResult"></typeparam>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static IEnumerable<TResult?> IterativeDeepeningDepthFirstSearchTraverse<TResult>(this INode<TResult?>? node)
//    {
//        return IterativeDeepeningDepthFirstSearch(node);
//    }
//}