using MediFuseConsoleApp1;
using static MediFuseConsoleApp1.Tree;
using static MediFuseClassLibrary.MyUnion;

Console.WriteLine("Hello Ray!");
//Console.WriteLine("In-order: " + string.Join(", ", ino));

//Construct a simple tree:
//        10
//       /  \
//      5   15
//     / \    \
//    2   7    20
Console.WriteLine("Constructing a simple tree:");
Console.WriteLine("       10");
Console.WriteLine("      /  \\");
Console.WriteLine("     5   15");
Console.WriteLine("    / \\    \\");
Console.WriteLine("   2   7    20");


Console.WriteLine("Constructing a simple tree:");
Console.WriteLine("       10");
Console.WriteLine("      /  \\");
Console.WriteLine("     5   15");
Console.WriteLine("    / \\    \\");
Console.WriteLine("   2   7    20");
Console.WriteLine("/ is right child, \\ is left child");


//chemical bonding has angle and is 3d!
//double bond and triple bond are also 3d!


//NOT THE ABOVE TREE
INode<int> root = new Node<int>(10, [
    new Node<int>(5,[ new Node<int>(2, []),new Node<int>(7, [])]),
    new Node<int>(15,[new Node<int>(20, [])])]);

// Pre-order traversal
List<int?> pre = root.Preorder().Select(n => n?.Value ?? null).ToList();
Console.WriteLine($"Pre-order: {string.Join(", ", pre)}");
// In-order traversal
List<int?> ino = root.InOrder().Select(n => n?.Value ?? null).ToList();
Console.WriteLine($"In-order: {string.Join(", ", ino)}");
// Post-order traversal
List<int?> post = root.PostOrder().Select(n => n?.Value ?? null).ToList();
Console.WriteLine($"Post-order: {string.Join(", ", post)}");
// Depth First traversal
List<int?> dfs = root.DepthFirstSearch().Select(n => n?.Value ?? null).ToList();
Console.WriteLine($"Depth First Search: {string.Join(", ", dfs)}");
// Breadth First traversal
List<int?> bfs = root.BreadthFirstSearch().Select(n => n?.Value ?? null).ToList();
Console.WriteLine($"Breadth First Search: {string.Join(", ", bfs)}");
// Depth Limited Search
List<int?> dls = root.DepthLimitedSearchTraverse().Select(n => n?.Value ?? null).ToList();
Console.WriteLine($"Depth Limited Search(No Depth Limit): {string.Join(", ", dls)}");
// Iterative Deepening Depth First Search
List<int?> iddfs = root.IterativeDeepeningDepthFirstSearch().Select(n => n?.Value ?? null).ToList();
Console.WriteLine($"Iterative Deepening Depth First Search: {string.Join(", ", iddfs)}");


// Using LINQ selector on a preorder traversal to select only even values
List<int?> evenValues = root.Traverse(Tree.Preorder)
                     .Select(x => x?.Value ?? null)
                     .Where(x => x % 2 is 0)
                     .ToList();

Console.WriteLine($"We are the even values in Preorder: {string.Join(", ", evenValues)}");

// Using LINQ selector on a depth first search traversal to search a list of values
List<int?> searchValues = root.Traverse(x=>Tree.DepthFirstSearch(x))
                     .Select(x => x?.Value ?? null)
                     .Where(x => x is 7 || x is 20)
                     .ToList();
Console.WriteLine($"We are the search values in Depth First Search: {string.Join(", ", searchValues)}");


Tree.Preorder(root).ToList().ForEach(x => Console.Write($"{x?.Value}, "));
Console.WriteLine();
Tree.InOrder(root).ToList().ForEach(x => Console.Write($"{x?.Value}, "));
Console.WriteLine();
Tree.PostOrder(root).ToList().ForEach(x => Console.Write($"{x?.Value}, "));
Console.WriteLine();
Tree.DepthFirstSearch(root).ToList().ForEach(x => Console.Write($"{x?.Value}, "));
Console.WriteLine();
Tree.BreadthFirstSearch(root).ToList().ForEach(x => Console.Write($"{x?.Value}, "));
Console.WriteLine();
Tree.DepthLimitedSearch(root).ToList().ForEach(x => Console.Write($"{x?.Value}, "));


y<string, int>? aCase = y<string, int>.NewA("Hello");
y<string, int>? bCase = y<string, int>.NewB(42);

switch(aCase)
{
    case y<string, int>.A A:
        Console.WriteLine($"A: {A.Item}");
        break;
    case y<string, int>.B B:
        Console.WriteLine($"B: {B.Item}");
        break;
}
if (aCase is y<string, int>.A a)
{
    Console.WriteLine($"A: {a.Item}");
}
else if (bCase is y<string, int>.B b)
{
    Console.WriteLine($"B: {b.Item}");
}

static IEnumerable<y<T1?,T2?>> Ys<T1, T2>(IEnumerable<y<T1, T2>> ys)
{
    yield return y<T1?, T2?>.NewA(default(T1));
    yield return y<T1?, T2?>.NewB(default(T2));
}

//Console output:
/***
 * Hello Ray!
Constructing a simple tree:
       10
      /  \
     5   15
    / \    \
   2   7    20
Constructing a simple tree:
       10
      /  \
     5   15
    / \    \
   2   7    20
/ is right child, \ is left child
Pre-order: 10, 5, 2, 7, 15, 20
In-order: 2, 5, 7, 10, 15, 20
Post-order: 2, 7, 5, 20, 15, 10
Depth First Search: 10, 5, 2, 7, 15, 20
Breadth First Search: 10, 5, 15, 2, 7, 20
Depth Limited Search(No Depth Limit): 10, 5, 2, 7, 15, 20
Iterative Deepening Depth First Search: 10, 5, 15, 2, 7, 20
We are the even values in Preorder: 10, 2, 20
We are the search values in Depth First Search: 7, 20
10, 5, 2, 7, 15, 20,
2, 5, 7, 10, 15, 20,
2, 7, 5, 20, 15, 10,
10, 5, 2, 7, 15, 20,
10, 5, 15, 2, 7, 20,
10, 5, 2, 7, 15, 20, A: Hello
A: Hello
***/