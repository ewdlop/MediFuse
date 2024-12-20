using MediFuseConsoleApp1;
using static MediFuseConsoleApp1.Tree;

Console.WriteLine("Hello Ray!");
//Console.WriteLine("Inorder: " + string.Join(", ", ino));

//Construct a simple tree:
//        10
//       /  \
//      5   15
//     / \    \
//    2   7    20

INode<int> root = new Node<int>(10, [
    new Node<int>(5,[ new Node<int>(2, []),new Node<int>(7, [])]),
    new Node<int>(15,[new Node<int>(20, [])])]);

// Preorder traversal
List<int> pre = root.Preorder().ToList();
// Inorder traversal
List<int> ino = root.Inorder().ToList();
// Postorder traversal
List<int> post = root.Postorder().ToList();

Console.WriteLine($"Preorder: {string.Join(", ", pre)}");
Console.WriteLine($"Inorder: {string.Join(", ", ino)}");
Console.WriteLine($"Postorder: {string.Join(", ", post)}");

// Using LINQ selector on a preorder traversal to select only even values
List<int> evenValues = root.Traverse(Tree.Preorder)
                     .Where(x => x % 2 == 0)
                     .ToList();
Console.WriteLine($"We are the Even values in Preorder: {string.Join(", ", evenValues)}");


/***
Hello Ray!
Preorder: 10, 5, 2, 7, 15, 20
Inorder: 5, 2, 7, 10, 15, 20
Postorder: 2, 7, 5, 20, 15, 10
We are the Even values in in Preorder: 10, 2, 20
***/