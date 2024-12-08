using System;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 Position { get; private set; }

    public Node(Vector3 position)
    {
        this.Position = position;
    }
}

public class NodeGenerator : MonoBehaviour
{
    public List<Node> nodes = new List<Node>();
    public List<Node> openNodes = new List<Node>();

    public Vector3 boundaryCenter = Vector3.zero;
    public float boundaryRadius = 50f;

    public float maxDistance = 70f;
    public float minDistance = 30f;

    public float minSpacing = 5f;

    void Start()
    {
        Node startNode = new Node(Vector3.zero);
        nodes.Add(startNode);
        openNodes.Add(startNode);

        ProcessNodes();
    }

    public void ProcessNodes()
    {
        while (openNodes.Count > 0)
        {
            /* pick the first element and remove it from the list */
            Node currentNode = openNodes[0];
            openNodes.RemoveAt(0);

            GenerateCandidates(currentNode);
        }
    }

    private void GenerateCandidates(Node currentNode)
    {
        int candidateCount = UnityEngine.Random.Range(3, 5);

        /* for each candidate */
        for (int i = 0; i < candidateCount; i++)
        {
            float distance = UnityEngine.Random.Range(minDistance, maxDistance);
            float angle = UnityEngine.Random.Range(0f, 360f);

            Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0,
            Mathf.Sin(angle * Mathf.Deg2Rad)
            ) * distance;

            Vector3 candidatePosition = currentNode.Position + offset;

            if (IsValidNode(candidatePosition))
            {
                Node newNode = new Node(candidatePosition);
                openNodes.Add(newNode);
                nodes.Add(newNode);
            }
        }
    }

    public bool IsValidNode(Vector3 position)
    {
        /* check if it is in the boundary*/
        if (!IsWithinBoundary(position))
            return false;

        /* check if it is distant enough from the nodes*/
        foreach (var existingNode in nodes)
        {
            if (Vector3.Distance(position, existingNode.Position) < minSpacing)
                return false;
        }
        return true;
    }

    private bool IsWithinBoundary(Vector3 position)
    {
        /* check if within a circular boundary */
        return Vector3.Distance(position, boundaryCenter) <= boundaryRadius;
    }

}