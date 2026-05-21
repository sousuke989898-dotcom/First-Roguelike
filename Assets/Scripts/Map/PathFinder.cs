using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PathFinder
{

    public static List<Vector2Int> GetPath(Vector2Int start, Vector2Int end)
    {
        if (!MapManager.Instance.MapData.IsFloor(end)) return null;

        Dictionary<Vector2Int, Node> openList = new();
        Dictionary<Vector2Int, Node> ClosedList = new();
        //List<Node> ClosedList = new();

        Node startNode = new(start);
        startNode.SetCost(0, Vector2IntExtensions.GetDistance(start,end));
        openList.Add(start,startNode);

        while (openList.Count > 0)
        {
            // OpenListの中で一番Fコストが小さいNodeを探す
            Node currentNode = openList.Values.First();

            foreach (Node node in openList.Values)
            {
                if (node.F < currentNode.F || (node.F == currentNode.F && node.H < currentNode.H))
                {
                    currentNode = node;
                }
            }


            openList.Remove(currentNode.Pos);
            ClosedList.Add(currentNode.Pos,currentNode);

            if (currentNode.Pos == end)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Vector2Int vector in DirectionTool.DirectionVectors.Values)
            {
                if (vector == Vector2Int.zero) continue;
                Vector2Int pos = currentNode.Pos + vector;

                if (!MapManager.Instance.MapData.IsFloor(pos) || ClosedList.ContainsKey(pos)) continue;


                float newG = currentNode.G + 1;
                Node openNode = null;
                if (openList.ContainsKey(pos)) openNode = openList[pos];

                if (openNode == null)
                {
                    Node newNode = new(pos)
                    {
                        parent = currentNode 
                    };
                    newNode.SetCost(newG, Vector2IntExtensions.GetDistance(pos,end));
                    openList.Add(pos,newNode);
                }
                else if (newG < openNode.G)
                {
                    openNode.G = newG;
                    openNode.parent = currentNode;
                }

            }
        }

        return null;
    }

    public static List<Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2Int> path = new();
        Node currentNode = endNode;
        while (currentNode != null && currentNode.Pos != startNode.Pos)
        {
            path.Add(currentNode.Pos);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
}