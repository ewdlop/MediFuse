using MediFuseConsoleApp1;
using static MediFuseConsoleApp1.Tree;

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

INode<int> root = new Node<int>(10, [
    new Node<int>(5,[ new Node<int>(2, []),new Node<int>(7, [])]),
    new Node<int>(15,[new Node<int>(20, [])])]);

// Pre-order traversal
List<int> pre = root.Preorder().ToList();
Console.WriteLine($"Pre-order: {string.Join(", ", pre)}");
// In-order traversal
List<int> ino = root.InOrder().ToList();
Console.WriteLine($"In-order: {string.Join(", ", ino)}");
// Post-order traversal
List<int> post = root.PostOrder().ToList();
Console.WriteLine($"Post-order: {string.Join(", ", post)}");
// Depth First traversal
List<int> dfs = root.DepthFirstSearch().ToList();
Console.WriteLine($"Depth First Search: {string.Join(", ", dfs)}");
// Breadth First traversal
List<int> bfs = root.BreadthFirstSearch().ToList();
Console.WriteLine($"Breadth First Search: {string.Join(", ", bfs)}");
// Depth Limited Search
List<int> dls = root.DepthLimitedSearchTraverse().ToList();
Console.WriteLine($"Depth Limited Search(No Depth Limit): {string.Join(", ", dls)}");
// Iterative Deepening Depth First Search
List<int> iddfs = root.IterativeDeepeningDepthFirstSearch().ToList();
Console.WriteLine($"Iterative Deepening Depth First Search: {string.Join(", ", iddfs)}");


// Using LINQ selector on a preorder traversal to select only even values
List<int> evenValues = root.Traverse(Tree.Preorder)
                     .Where(x => x % 2 == 0)
                     .ToList();

Console.WriteLine($"We are the even values in Preorder: {string.Join(", ", evenValues)}");

// Using LINQ selector on a depth first search traversal to search a list of values
List<int> searchValues = root.Traverse(Tree.DepthFirstSearch)
                     .Where(x => x == 7 || x == 20)
                     .ToList();
Console.WriteLine($"We are the search values in Depth First Search: {string.Join(", ", searchValues)}");

//Console output:
/***
Hello Ray!
Pre-order: 10, 5, 2, 7, 15, 20
In-order: 5, 2, 7, 10, 15, 20
Post-order: 2, 7, 5, 20, 15, 10
Depth First Search: 10, 15, 20, 5, 7, 2
Breadth First Search: 10, 5, 15, 2, 7, 20
Depth Limited Search: 10, 5, 2, 7, 15, 20
Iterative Deepening Depth First Search: 10, 5, 15, 2, 7, 20
We are the Even values in Preorder: 10, 2, 20
We are the search values in Depth First Search: 20, 7
***/