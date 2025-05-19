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
            public int[] Neighbors = new int[] { -1, -1, -1, -1 };

            public Edge(int number, Point position)
            {
                this.number = number;
                Position = position;
            }
        }

        private List<Edge> _Edges = new List<Edge>();

        public Hamiltonian(Settings settings, Graphics graphics) : 
            base(settings.Rows, settings.Columns, settings.BlockSize, graphics)
        {
            for (int i = 0; i < settings.WallPositions.Count; i++)
            {
                DrawBlock(settings.WallPositions[i], BlockTypes.WallBlock);
                _Block[settings.WallPositions[i].X, settings.WallPositions[i].Y] = BlockTypes.WallBlock;
            }

            SetEdges(settings.WallPositions);
            int[,] graph = new int[_Edges.Count, _Edges.Count];
            graph = GetVectorGraph();
            ListVectorGraph(graph);
            Debug.WriteLine("Number of Edges: " + _Edges.Count);

            HamiltonianCycle hamiltonianCycle = new HamiltonianCycle(_Edges.Count, graph);
            int[] ints = hamiltonianCycle.GetPath();


            string textForBelow = "";
            int numberCounter = 0;
            foreach (int i in ints)
            {
                numberCounter++;
                if (numberCounter % 15 == 0)
                {
                    textForBelow += "\n";
                }
                textForBelow += i.ToString() + "->";
                Debug.WriteLine("Hamiltonian: " + i.ToString() + ", Position:" + _Edges[i].Position.ToString());
            }
            textForBelow = textForBelow.Substring(0, textForBelow.Length - 2);


            DrawPath(ints);
            Point textBelow = new Point(0, settings.BlockSize * settings.Rows + 5);
            DrawText(textForBelow, textBelow);
        }

        /*
        // souradnice bunky, typ bunky
        public Hamiltonian(int Rows, int Columns, int BlockSize, Graphics graphics, List<Point> Walls) : base(Rows, Columns, BlockSize, graphics)
        {
            for (int i = 0; i < Walls.Count; i++)
            {
                DrawBlock(Walls[i], BlockTypes.WallBlock);
                _Block[Walls[i].X, Walls[i].Y] = BlockTypes.WallBlock;
            }

            SetEdges(Walls);
            int[,] graph = new int[_Edges.Count, _Edges.Count];
            graph = GetVectorGraph();
            ListVectorGraph(graph);
            Debug.WriteLine("Number of Edges: " + _Edges.Count);

            //HamiltonianCycle hamiltonianCycle = new HamiltonianCycle(graph, Edges.Count);
            //int[] ints = hamiltonianCycle.GetPath();
            HamiltonianCycle hamiltonianCycle = new HamiltonianCycle(_Edges.Count, graph);
            int[] ints = hamiltonianCycle.GetPath();

            
            string textForBelow = "";
            int numberCounter = 0;
            foreach (int i in ints)
            {
                numberCounter++;
                if (numberCounter % 15 == 0)
                {
                    textForBelow += "\n";
                }
                textForBelow += i.ToString() + "->";
                Debug.WriteLine("Hamiltonian: " + i.ToString() + ", Position:" + _Edges[i].Position.ToString());
            }
            textForBelow = textForBelow.Substring(0, textForBelow.Length - 2);


            DrawPath(ints);
            Point textBelow = new Point(0, BlockSize * Rows + 5);
            DrawText(textForBelow, textBelow);
            
        }
        */

        private void DrawText(string text, Point position)
        {
            using (Font font = new Font("Arial", 10)) // Specify font and size
            using (Brush brush = new SolidBrush(Color.Black)) // Specify text color
            {
                _Graphics.DrawString(text, font, brush, position.X, position.Y);
            }
        }

        private void DrawPath(int[] path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                if (i != 0)
                {
                    Point start = getMidPixelOfBlock(_Edges[path[i - 1]].Position);
                    Point end = getMidPixelOfBlock(_Edges[path[i]].Position);
                    DrawLine(start, end);

                    Point block = _Edges[path[i - 1]].Position;
                    DrawText(path[i - 1].ToString(), new Point(block.Y * this._BlockSize, block.X * this._BlockSize));
                }
            }
            Point lastBlock = _Edges[path[path.Length - 1]].Position;
            DrawText(path[path.Length - 1].ToString(), new Point(lastBlock.Y * this._BlockSize, lastBlock.X * this._BlockSize));

        }

        private void DrawLine(Point start, Point end)
        {
            Pen pen = new Pen(Color.BlueViolet, 3);
            _Graphics.DrawLine(pen, start, end);
        }

        private Point getMidPixelOfBlock(Point position)
        {
            Point p = new Point();
            p.X = (position.Y * _BlockSize) + (_BlockSize / 2);
            p.Y = (position.X * _BlockSize) + (_BlockSize / 2);
            return p;
        }

        private void SetEdges(List<Point> Walls)
        {
            //Vytvoreni Edges, kde kazdy edge je bunkou, ktera neni zed a obsahuje pozici a prirazene cislo
            int index = 0;
            for (int i = 0; i < _Rows; i++)
            {
                for (int j = 0; j < _Columns; j++)
                {
                    if (_Block[i, j] != BlockTypes.WallBlock)
                    {
                        _Edges.Add(new Edge(index++, new Point(i, j)));
                    }
                }
            }

            // priradeni sousedu
            for (int i = 0; i < _Edges.Count; i++)
            {
                int x = _Edges[i].Position.X;
                int y = _Edges[i].Position.Y;
                if (x > 0 && _Block[x - 1, y] != BlockTypes.WallBlock)
                    _Edges[i].Neighbors[0] = GetEdgeNumber(x - 1, y);
                if (x < _Rows - 1 && _Block[x + 1, y] != BlockTypes.WallBlock)
                    _Edges[i].Neighbors[1] = GetEdgeNumber(x + 1, y);
                if (y > 0 && _Block[x, y - 1] != BlockTypes.WallBlock)
                    _Edges[i].Neighbors[2] = GetEdgeNumber(x, y - 1);
                if (y < _Columns - 1 && _Block[x, y + 1] != BlockTypes.WallBlock)
                    _Edges[i].Neighbors[3] = GetEdgeNumber(x, y + 1);
            }
        }

        private int GetEdgeNumber(int x, int y)
        {
            for (int i = 0; i < _Edges.Count; i++)
            {
                if (_Edges[i].Position.X == x && _Edges[i].Position.Y == y)
                    return _Edges[i].number;
            }
            return -1;
        }

        public int[,] GetVectorGraph()
        {
            int[,] graph = new int[_Edges.Count, _Edges.Count];
            for (int i = 0; i < _Edges.Count; i++)
            {
                for (int j = 0; j < _Edges.Count; j++)
                {
                    graph[i, j] = 0;
                }
            }
            for (int i = 0; i < _Edges.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (_Edges[i].Neighbors[j] != -1)
                    {
                        graph[i, (_Edges[i].Neighbors[j])] = 1;
                    }
                }
            }
            return graph;
        }

        public void ListVectorGraph(int[,] VGraph)
        {
            for (int i = 0; i < _Edges.Count; i++)
            {
                for (int j = 0; j < _Edges.Count; j++)
                {
                    Debug.Write(VGraph[i, j] + " ");
                }
                Debug.WriteLine("");
            }
        }

    }


    public class HamiltonianCycle
    {
        readonly int V = 5;
        int[] path;

        public HamiltonianCycle(int v, int[,] graph)
        {
            V = v;
            path = new int[V];
            this.hamCycle(graph);

        }

        public int[] GetPath()
        {
            return path;
        }

        /* A utility function to check 
        if the vertex v can be added at 
        index 'pos'in the Hamiltonian Cycle
        constructed so far (stored in 'path[]') */
        bool isSafe(int v, int[,] graph,
                    int[] path, int pos)
        {
            /* Check if this vertex is 
            an adjacent vertex of the
            previously added vertex. */
            if (graph[path[pos - 1], v] == 0)
                return false;

            /* Check if the vertex has already 
            been included. This step can be
            optimized by creating an array
            of size V */
            for (int i = 0; i < pos; i++)
                if (path[i] == v)
                    return false;

            return true;
        }

        /* A recursive utility function
        to solve hamiltonian cycle problem */
        bool hamCycleUtil(int[,] graph, int[] path, int pos)
        {
            /* base case: If all vertices 
            are included in Hamiltonian Cycle */
            if (pos == V)
            {
                // And if there is an edge from the last included
                // vertex to the first vertex
                if (graph[path[pos - 1], path[0]] == 1)
                    return true;
                else
                    return false;
            }

            // Try different vertices as a next candidate in
            // Hamiltonian Cycle. We don't try for 0 as we
            // included 0 as starting point in hamCycle()
            for (int v = 1; v < V; v++)
            {
                /* Check if this vertex can be 
                added to Hamiltonian Cycle */
                if (isSafe(v, graph, path, pos))
                {
                    path[pos] = v;

                    /* recur to construct rest of the path */
                    if (hamCycleUtil(graph, path, pos + 1) == true)
                        return true;

                    /* If adding vertex v doesn't 
                    lead to a solution, then remove it */
                    path[pos] = -1;
                }
            }

            /* If no vertex can be added to Hamiltonian Cycle
            constructed so far, then return false */
            return false;
        }

        /* This function solves the Hamiltonian 
        Cycle problem using Backtracking. It 
        mainly uses hamCycleUtil() to solve the
        problem. It returns false if there
        is no Hamiltonian Cycle possible, 
        otherwise return true and prints the path.
        Please note that there may be more than 
        one solutions, this function prints one 
        of the feasible solutions. */
        int hamCycle(int[,] graph)
        {
            path = new int[V];
            for (int i = 0; i < V; i++)
                path[i] = -1;

            /* Let us put vertex 0 as the first
            vertex in the path. If there is a 
            Hamiltonian Cycle, then the path can be
            started from any point of the cycle 
            as the graph is undirected */
            path[0] = 0;
            if (hamCycleUtil(graph, path, 1) == false)
            {
                Debug.WriteLine("\nSolution does not exist");
                return 0;
            }

            //printSolution(path);
            return 1;
        }

        /* A utility function to print solution */
        void printSolution(int[] path)
        {
            Debug.WriteLine("Solution Exists: Following" +
                            " is one Hamiltonian Cycle");
            for (int i = 0; i < V; i++)
                Debug.Write(" " + path[i] + " ");

            // Let us print the first vertex again
            //  to show the complete cycle
            Debug.WriteLine(" " + path[0] + " ");
        }
    }
}
