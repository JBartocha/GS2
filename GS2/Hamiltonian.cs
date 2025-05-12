using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GS2
{

    public class Hamiltonian : Grid
    {
        public class Edge
        {
            public int number;
            public Point Position;
            public int[] Neighbors = new int[4];
            

            public Edge(int number, Point position)
            {
                this.number = number;
                Position = position;
            }
        }

        private List<Edge> Edges = new List<Edge>();
        //private List<Edge> OrderedPath = new List<Edge>();

        // souradnice bunky, typ bunky
        public Hamiltonian(int Rows, int Columns, int BlockSize, Graphics graphics, List<Point> Walls) : base(Rows, Columns, BlockSize, graphics)
        {
            for (int i = 0; i < Walls.Count; i++)
            {
                DrawBlock(Walls[i], BlockTypes.WallBlock);
                Block[Walls[i].X, Walls[i].Y] = BlockTypes.WallBlock;
            }

            SetEdges(Walls);
            int[,] graph = new int[Edges.Count, Edges.Count];
            graph = GetVectorGraph();
            ListVectorGraph(graph);
            //Debug.WriteLine("Number of Edges: " + Edges.Count);

            HamiltonianCycle hamiltonianCycle = new HamiltonianCycle(graph, Edges.Count);
            int[] ints = hamiltonianCycle.GetPath();

            string textForBelow = "";
            int numberCounter = 0;
            foreach (int i in ints)
            {
                numberCounter++;
                if(numberCounter % 15 == 0)
                {
                    textForBelow += "\n";
                }
                textForBelow += i.ToString() + "->";
                Debug.WriteLine("Hamiltonian: " + i.ToString() + ", Position:" + Edges[i].Position.ToString());
            }
            textForBelow = textForBelow.Substring(0, textForBelow.Length - 2);


            DrawPath(ints);
            Point textBelow = new Point(0, BlockSize * Rows + 5);
            DrawText(textForBelow, textBelow);
        }

        private void DrawText(string text, Point position)
        {
            using (Font font = new Font("Arial", 10)) // Specify font and size
            using (Brush brush = new SolidBrush(Color.Black)) // Specify text color
            {
                Graphics.DrawString(text, font, brush, position.X, position.Y);
            }
        }

        private void DrawPath(int[] path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                if (i != 0)
                {
                    Point start = getMidPixelOfBlock(Edges[path[i - 1]].Position);
                    Point end = getMidPixelOfBlock(Edges[path[i]].Position);
                    DrawLine(start, end);

                    Point block = Edges[path[i - 1]].Position;
                    DrawText(path[i - 1].ToString(), new Point(block.Y * this.BlockSize, block.X * this.BlockSize));
                }
            }
            Point lastBlock = Edges[path[path.Length - 1]].Position;
            DrawText(path[path.Length - 1].ToString(), new Point(lastBlock.Y * this.BlockSize, lastBlock.X * this.BlockSize));

        }

        private void DrawLine(Point start, Point end)
        {
            Pen pen = new Pen(Color.BlueViolet, 3);
            Graphics.DrawLine(pen, start, end);
        }

        private Point getMidPixelOfBlock(Point position)
        {
            Point p = new Point();
            p.X = (position.Y * BlockSize) + (BlockSize / 2);
            p.Y = (position.X * BlockSize) + (BlockSize / 2);
            return p;
        }

        private void SetEdges(List<Point> Walls)
        {
            //Vytvoreni Edges, kde kazdy edge je bunkou, ktera neni zed a obsahuje pozici a prirazene cislo
            int index = 1;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Block[i, j] != BlockTypes.WallBlock)
                    {
                        Edges.Add(new Edge(index++, new Point(i,j)));
                    }
                }
            }

            // priradeni sousedu
            for (int i = 0; i < Edges.Count; i++)
            {
                int x = Edges[i].Position.X;
                int y = Edges[i].Position.Y;
                if (x > 0 && Block[x - 1, y] != BlockTypes.WallBlock)
                    Edges[i].Neighbors[0] = GetEdgeNumber(x - 1, y);
                if (x < Rows - 1 && Block[x + 1, y] != BlockTypes.WallBlock)
                    Edges[i].Neighbors[1] = GetEdgeNumber(x + 1, y);
                if (y > 0 && Block[x, y - 1] != BlockTypes.WallBlock)
                    Edges[i].Neighbors[2] = GetEdgeNumber(x, y - 1);
                if (y < Columns - 1 && Block[x, y + 1] != BlockTypes.WallBlock)
                    Edges[i].Neighbors[3] = GetEdgeNumber(x, y + 1);
            }
        }

        private int GetEdgeNumber(int x, int y)
        {
            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].Position.X == x && Edges[i].Position.Y == y)
                    return Edges[i].number;
            }
            return -1;
        }

        public int[,] GetVectorGraph()
        {
            int[,] graph = new int[Edges.Count, Edges.Count];
            for (int i = 0; i < Edges.Count; i++)
            {
                for (int j = 0; j < Edges.Count; j++)
                {
                    graph[i, j] = 0;
                }
            }
            for (int i = 0; i < Edges.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Edges[i].Neighbors[j] != 0)
                    {
                        graph[i, (Edges[i].Neighbors[j])-1] = 1;
                    }
                }
            }
            return graph;
        }

        public void ListVectorGraph(int[,] VGraph)
        {
            for(int i = 0; i < Edges.Count; i++)
            {
                for (int j = 0; j < Edges.Count; j++)
                {
                    Debug.Write(VGraph[i, j] + " ");
                }
                Debug.WriteLine("");
            }
        }
        
    }


    // Indexes have to start with number 1 (not zero)
    public class HamiltonianCycle
    {
        private static int Nodes;
        private static int[,] graph;

        private static int[] path = new int[Nodes];
        private static bool isPath = false;

        public HamiltonianCycle(int[,] graph, int nodes)
        {
            HamiltonianCycle.graph = graph;
            HamiltonianCycle.Nodes = nodes;

            path = new int[Nodes];
            isPath = hamiltonianCycle();
            //DebugReadValues();
        }

        public int[] GetPath()
        {
            if(isPath)
            {
                return path;
            }
            else
            {
                // TODO osetrit jinak než přes vyhození výjimky
                throw new Exception("Hamiltonian DOES NOT EXIST");
            }
        }

        private void DebugReadValues()
        {
            Debug.WriteLine("Hamiltonian Cycle: ");
            for (int i = 0; i < Nodes; i++)
            {
                Debug.Write(path[i] + " ");
            }
            Debug.WriteLine("");
            Debug.WriteLine("graph.lenght: " + graph.Length);
        }

        // method to display the Hamiltonian cycle
        static void displayCycle()
        {
            Debug.WriteLine("Cycle Found: ");
            /*
            for (int i = 0; i < Nodes; i++)
                Debug.WriteLine(path[i] + " ");
            */
        }

        // method to check if adding vertex v to the path is valid
        static bool isValid(int NODEv, int k)
        {
            // If there is no edge between path[k-1] and v
            if (graph[path[k - 1], NODEv] == 0)
                return false;
            // Check if vertex v is already taken in the path
            for (int i = 0; i < k; i++)
                if (path[i] == NODEv)
                    return false;
            return true;
        }
        // method to find the Hamiltonian cycle
        static bool cycleFound(int k)
        {
            // When all vertices are in the path
            if (k == Nodes)
            {
                // Check if there is an edge between the last and first vertex
                if (graph[path[k - 1], path[0]] == 1)
                    return true;
                else
                    return false;
            }
            // adding each vertex (except the starting point) to the path
            for (int v = 1; v < Nodes; v++)
            {
                if (isValid(v, k))
                {
                    path[k] = v;
                    if (cycleFound(k + 1))
                        return true;
                    // Remove v from the path
                    path[k] = -1;
                }
            }
            return false;
        }
        // method to find and display the Hamiltonian cycle
        static bool hamiltonianCycle()
        {
            for (int i = 0; i < Nodes; i++)
                path[i] = -1;
            // Set the first vertex as 0
            // the first vertex is always the starting point
            path[0] = 0;
            if (!cycleFound(1))
            {
                Debug.WriteLine("Solution does not exist");
                return false;
            }
            displayCycle();
            return true;
        }
    }
}
