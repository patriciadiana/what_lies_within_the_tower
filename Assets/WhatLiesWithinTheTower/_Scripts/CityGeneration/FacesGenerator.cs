using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FacesGenerator : MonoBehaviour
{
    public NodeGenerator nodeGenerator;
    public EdgeGenerator edgeGenerator;
    public GameObject[] buildingsPrefabs;

    List<List<(Node, Node)>> faces = new List<List<(Node, Node)>>();

    void Start()
    {
        GenerateFaces();
    }

    public List<List<(Node, Node)>> GenerateFaces()
    {
        HashSet<(Node, Node)> allEdgesTwice = new HashSet<(Node, Node)>();

        foreach (var edge in edgeGenerator.edges)
        {
            allEdgesTwice.Add((edge.Item1, edge.Item2));
            allEdgesTwice.Add((edge.Item2, edge.Item1));
        }

        while (allEdgesTwice.Count > 0)
        {
            var edge = allEdgesTwice.First();
            Node startNode = edge.Item1;
            Node endNode = edge.Item2;

            List<(Node, Node)> currentFace = new List<(Node, Node)>();

            Node currentNode = startNode;
            Node previousNode = null;

            do
            {
                currentFace.Add((currentNode, endNode));
                allEdgesTwice.Remove((currentNode, endNode));
                allEdgesTwice.Remove((endNode, currentNode));

                Node nextNode = FindRightTurnNode(currentNode, previousNode);

                if (nextNode == null)
                {
                    break;
                }

                previousNode = currentNode;
                currentNode = nextNode;

                endNode = FindNextEdge(currentNode, endNode);

                if (endNode == null)
                {
                    break;
                }

            } while (currentNode != startNode);

            /* if a valid face is completed, add it to the list */
            if (currentNode == startNode && currentFace.Count > 0)
            {
                faces.Add(currentFace);

                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    continue;

                CreateCubesForFace(currentFace);
            }
        }

        Debug.Log($"Generated {faces.Count} faces.");
        return faces;
    }
    void CreateCubesForFace(List<(Node, Node)> faceEdges)
    {
        Vector3 center = Vector3.zero;
        foreach (var edge in faceEdges)
        {
            center += (edge.Item1.Position + edge.Item2.Position) / 2f;
        }
        center /= faceEdges.Count;

        float radius = 10f; 
        int buildingCount = 1;

        List<Vector3> placedBuildings = new List<Vector3>();

        for (int i = 0; i < buildingCount; i++)
        {
            GameObject randomBuildingPrefab = buildingsPrefabs[UnityEngine.Random.Range(0, buildingsPrefabs.Length)];
            Vector3 newBuildingPosition;

            int maxAttempts = 100;
            int attempts = 0;

            do
            {
                newBuildingPosition = center + (Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0) * new Vector3(radius, 0, 0));
                attempts++;
            }
            while (attempts < maxAttempts && placedBuildings.Any(pos => Vector3.Distance(pos, newBuildingPosition) < radius));

            Instantiate(randomBuildingPrefab, newBuildingPosition, Quaternion.identity);
            placedBuildings.Add(newBuildingPosition);
        }
    }

    private Node FindRightTurnNode(Node currentNode, Node previousNode)
    {
        Vector3 prevDirection = previousNode != null ? currentNode.Position - previousNode.Position : Vector3.zero;
        Node bestNode = null;
        float bestAngle = float.MaxValue;

        foreach (var edge in edgeGenerator.edges)
        {
            Node candidate = edge.Item1 == currentNode ? edge.Item2 : edge.Item2 == currentNode ? edge.Item1 : null;
            if (candidate != null)
            {
                Vector3 direction = candidate.Position - currentNode.Position;
                float angle = Vector3.SignedAngle(prevDirection, direction, Vector3.up);

                if (angle > 0 && angle < bestAngle)
                {
                    bestNode = candidate;
                    bestAngle = angle;
                }
            }
        }

        return bestNode;
    }

    private Node FindNextEdge(Node currentNode, Node endNode)
    {
        foreach (var edge in edgeGenerator.edges)
        {
            if (edge.Item1 == currentNode && edge.Item2 != endNode)
            {
                return edge.Item2;
            }
            if (edge.Item2 == currentNode && edge.Item1 != endNode)
            {
                return edge.Item1;
            }
        }
        return null;
    }

    //void OnDrawGizmos()
    //{
    //    if (faces != null)
    //    {
    //        // Create a palette of colors or generate random ones
    //        Color[] colors = { Color.green, Color.blue, Color.yellow, Color.cyan };

    //        for (int i = 0; i < faces.Count; i++)
    //        {
    //            // Assign a unique color for each face
    //            Gizmos.color = colors[i % colors.Length];

    //            // Draw all edges in the face using the same color
    //            foreach (var edge in faces[i])
    //            {
    //                Gizmos.DrawLine(edge.Item1.Position, edge.Item2.Position);
    //            }
    //        }
    //    }
    //}

}
