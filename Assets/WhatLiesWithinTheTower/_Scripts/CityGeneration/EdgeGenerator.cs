using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EdgeGenerator : MonoBehaviour
{
    public NodeGenerator nodeGenerator;
    public List<(Node, Node)> edges = new List<(Node, Node)>();

    public GameObject roadPrefab;

    public float connectionRadius = 40f;
    public float minAngle = 60f;
    public int maxDegree = 4;

    public float edgeLength = 4f;

    void Start()
    {
        GenerateEdges();
    }

    public void GenerateEdges()
    {
        List<Node> openNodes = new List<Node>(nodeGenerator.nodes);

        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes[UnityEngine.Random.Range(0, openNodes.Count)];
            openNodes.Remove(currentNode);

            if (GetNodeDegree(currentNode) >= maxDegree)
                continue;

            if (TryCreateStreet(currentNode))
            {
                openNodes.RemoveAll(n => GetNodeDegree(n) >= maxDegree);
            }
        }
    }

    private bool TryCreateStreet(Node startNode)
    {
        bool createdEdge = false;
        Node currentNode = startNode;

        while (true)
        {
            Node bestNode = FindBestNode(currentNode);
            if (bestNode == null)
                break;

            edges.Add((currentNode, bestNode));
            createdEdge = true;

            CreateRoad(currentNode, bestNode);

            if (GetNodeDegree(bestNode) >= maxDegree)
                break;

            currentNode = bestNode;
        }

        return createdEdge;
    }

    private void CreateRoad(Node startNode, Node endNode)
    {
        Vector3 startPos = startNode.Position;
        Vector3 endPos = endNode.Position;

        GameObject road = Instantiate(roadPrefab);
        road.transform.position = (startPos + endPos) / 2f;
        road.transform.LookAt(endPos);

        float distance = Vector3.Distance(startPos, endPos);
        road.transform.localScale = new Vector3(road.transform.localScale.x, road.transform.localScale.y, distance);
    }

    private Node FindBestNode(Node fromNode)
    {
        List<Node> candidates = new List<Node>();

        foreach (var candidate in nodeGenerator.nodes)
        {
            if (candidate == fromNode || GetNodeDegree(candidate) >= maxDegree)
                continue;

            float distance = Vector3.Distance(fromNode.Position, candidate.Position);
            if (distance > connectionRadius || EdgeExists(fromNode, candidate))
                continue;

            if (ViolatesConstraints(fromNode, candidate))
                continue;

            candidates.Add(candidate);
        }

        if (candidates.Count == 0)
            return null;

        candidates.Sort((a, b) =>
            Vector3.Distance(fromNode.Position, a.Position).CompareTo(Vector3.Distance(fromNode.Position, b.Position)));

        return candidates[0];
    }

    private bool ViolatesConstraints(Node fromNode, Node toNode)
    {
        if (WouldCreateTriangle(fromNode, toNode))
            return true;

        if (!HasValidAngle(fromNode, toNode))
            return true;

        if(IsCrossing(fromNode, toNode)) 
            return true;

        return false;
    }

    private bool IsCrossing(Node fromNode, Node toNode)
    {
        foreach (var edge in edges)
        {
            Node edgeStart = edge.Item1;
            Node edgeEnd = edge.Item2;

            /* if two edges are connected at one of the ends it's not possible to cross each other so we continue*/
            if (edgeStart == fromNode || edgeStart == toNode || edgeEnd == fromNode || edgeEnd == toNode)
                continue;

            if (DoSegmentsIntersect(fromNode.Position, toNode.Position, edgeStart.Position, edgeEnd.Position))
                return true;
        }
        return false;
    }

    private bool DoSegmentsIntersect(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
    {
        /* helper function to find the orientation of an ordered triplet (p, q, r) */
        int Orientation(Vector3 p, Vector3 q, Vector3 r)
        {
            float val = (q.z - p.z) * (r.x - q.x) - (q.x - p.x) * (r.z - q.z);
            if (Mathf.Approximately(val, 0)) return 0; 
            return (val > 0) ? 1 : 2;                 
        }

        /* check if point r lies on segment pq */
        bool OnSegment(Vector3 p, Vector3 q, Vector3 r)
        {
            return Mathf.Min(p.x, q.x) <= r.x && r.x <= Mathf.Max(p.x, q.x) &&
                   Mathf.Min(p.z, q.z) <= r.z && r.z <= Mathf.Max(p.z, q.z);
        }

        int o1 = Orientation(p1, q1, p2);
        int o2 = Orientation(p1, q1, q2);
        int o3 = Orientation(p2, q2, p1);
        int o4 = Orientation(p2, q2, q1);

        if (o1 != o2 && o3 != o4)
            return true;

        /* p1, q1, and p2 are collinear, and p2 lies on segment p1q1 */
        if (o1 == 0 && OnSegment(p1, q1, p2)) return true;

        /* p1, q1, and q2 are collinear, and q2 lies on segment p1q1 */
        if (o2 == 0 && OnSegment(p1, q1, q2)) return true;

        /* p2, q2, and p1 are collinear, and p1 lies on segment p2q2 */
        if (o3 == 0 && OnSegment(p2, q2, p1)) return true;

        /* p2, q2, and q1 are collinear, and q1 lies on segment p2q2 */
        if (o4 == 0 && OnSegment(p2, q2, q1)) return true;

        return false;
    }


    private bool WouldCreateTriangle(Node a, Node b)
    {
        foreach (var edge1 in edges)
        {
            foreach (var edge2 in edges)
            {
                if ((edge1.Item1 == a) && (edge2.Item1 == b) && (edge1.Item2 == edge2.Item2)
                    || (edge1.Item2 == a) && (edge2.Item2 == b) && (edge1.Item1 == edge2.Item1)
                    || (edge1.Item1 == a) && (edge2.Item2 == b) && (edge1.Item2 == edge2.Item1)
                    || (edge1.Item2 == a) && (edge2.Item1 == b) && (edge1.Item1 == edge2.Item1))
                    return true;
            }
        }
        return false;
    }

    private bool HasValidAngle(Node fromNode, Node toNode)
    {
        Vector3 edgeDirection = (toNode.Position - fromNode.Position).normalized;

        foreach (var edge in edges)
        {
            if (edge.Item1 == fromNode || edge.Item2 == fromNode)
            {
                Node connectedNode = edge.Item1 == fromNode ? edge.Item2 : edge.Item1;
                Vector3 connectedDirection = (connectedNode.Position - fromNode.Position).normalized;

                float angle = Vector3.Angle(edgeDirection, connectedDirection);
                if (angle < minAngle)
                    return false;
            }
        }

        return true;
    }

    private int GetNodeDegree(Node node)
    {
        int degree = 0;
        foreach (var edge in edges)
        {
            if (edge.Item1 == node || edge.Item2 == node)
                degree++;
        }
        return degree;
    }

    private bool EdgeExists(Node a, Node b)
    {
        return edges.Exists(e =>
            (e.Item1 == a && e.Item2 == b) ||
            (e.Item1 == b && e.Item2 == a));
    }

    private void SplitEdges()
    {
        List<(Node, Node)> newEdges = new List<(Node, Node)>();

        foreach (var edge in edges)
        {
            Vector3 startNode = edge.Item1.Position;
            Vector3 endNode = edge.Item2.Position;

            float distance = Vector3.Distance(startNode, endNode);
            int numberOfSplits = Mathf.CeilToInt(distance / edgeLength);

            for (int i = 1; i < numberOfSplits; i++)
            {
                float t = i / (float)numberOfSplits;
                Vector3 splitPosition = Vector3.Lerp(startNode, endNode, t);

                Node newNode = new Node(splitPosition);
                nodeGenerator.nodes.Add(newNode);

                newEdges.Add((newNode, edge.Item1));
                newEdges.Add((newNode, edge.Item2));
            }
        }
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.black;

    //    foreach (var edge in edges)
    //    {
    //        Gizmos.DrawLine(edge.Item1.Position, edge.Item2.Position);
    //    }
    //}
}
