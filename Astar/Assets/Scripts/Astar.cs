using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        Debug.Log("Searching path");
        Node startNode = new Node(startPos, null, 0, 0);

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        openList.Add(startNode);

        Node currentNode = startNode;

        while (openList.Count > 0)
        {
            currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FScore < currentNode.FScore)
                { 
                    currentNode = openList[i]; 
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.position == endPos)
            {
                break;
            }

            foreach (Cell neighbour in GetNeighbours(grid[currentNode.position.x, currentNode.position.y], grid))
            {
                if (closedList.Any(n => n.position == neighbour.gridPosition)) 
                { 
                    continue; 
                }

                Node newNode = new Node(neighbour.gridPosition, currentNode, (int)currentNode.GScore + 1, GetDistance(neighbour.gridPosition, endPos));

                if (openList.Any(n => n.position == neighbour.gridPosition && newNode.GScore > n.GScore)) 
                { 
                    continue; 
                }

                openList.Add(newNode);
            }
        }

        return RetracePath(startNode, currentNode);
    }

    private int GetDistance(Vector2Int beginPoint, Vector2Int endPoint)
    {
        int distance = Mathf.Abs(beginPoint.x - endPoint.x) + Mathf.Abs(beginPoint.y - endPoint.y);
        return distance;
    }

    private List<Vector2Int> RetracePath(Node startNode, Node node)
    {
        List<Vector2Int> pathValue = new List<Vector2Int>();
        Node currentNode = node;

        while (currentNode != startNode)
        {
            pathValue.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        pathValue.Reverse();
        return pathValue;
    }

    private List<Cell> GetNeighbours(Cell cell, Cell[,] grid)
    {
        List<Cell> neighbours = new List<Cell>();
        int _xStart, _xEnd, _yStart, _yEnd;
        _xStart = _xEnd = _yStart = _yEnd = 0;

        if (!cell.HasWall(Wall.LEFT)) { _xStart = -1; }
        if (!cell.HasWall(Wall.RIGHT)) { _xEnd = 1; }
        if (!cell.HasWall(Wall.DOWN)) { _yStart = -1; }
        if (!cell.HasWall(Wall.UP)) { _yEnd = 1; }

        for (int x = _xStart; x <= _xEnd; x++)
        {
            for (int y = _yStart; y <= _yEnd; y++)
            {
                int cellX = cell.gridPosition.x + x;
                int cellY = cell.gridPosition.y + y;

                if (cellX < 0 || cellX >= grid.GetLength(0) || cellY < 0 || cellY >= grid.GetLength(1) || Mathf.Abs(x) == Mathf.Abs(y))
                {
                    continue;
                }

                neighbours.Add(grid[cellX, cellY]);
            }
        }

        return neighbours;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
