using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
  [SerializeField]
  private PathNode targetNode;

  private Dictionary<(int x, int y), PathNode> gridNodes;

  /// <summary>
  /// Start is called before the first frame update.
  /// </summary>
  private void
  Start() {
    BuildPath();
  }
  
  /// <summary>
  /// Utility function to build the pathfinding grid.
  /// </summary>
  private void
  BuildPath() {
    PathNode[] childNodes = transform.GetComponentsInChildren<PathNode>();
    
    // Safety checks
    {
      if (childNodes.Length == 0) {
        Debug.LogError("No nodes found in child");
        return;
      }

      if (targetNode == null) {
        Debug.LogError("No target node set");
        return;
      }

      if (!childNodes.Contains(targetNode)) {
        Debug.LogError("Target node not found in pathfinding");
        return;
      }
    }
    
    // Temporal context to initialize nodes and neighbours
    {
      gridNodes = new();
      
      // Temporal contextual function
      List<PathNode>
      GetAdjacentNodes(PathNode node) {
        List<PathNode> adjacentNodes = new List<PathNode>();

        (int x, int y)[] adjacentPositions = {
          ( 0,  1),
          ( 0, -1),
          ( 1,  0),
          (-1,  0)
        };

        foreach ((int x, int y) in adjacentPositions) {
          gridNodes.TryGetValue((node.GetPosition().x + x,
                                 node.GetPosition().y + y),
                                 out PathNode adjacentNode);
          
          if (adjacentNode != null)
            adjacentNodes.Add(adjacentNode);
        }

        return adjacentNodes;
      }
      
      // Initialize nodes
      foreach (PathNode node in childNodes) {
        node.SetPathfinding(this);
        
        gridNodes.Add(node.GetPosition(), node);
      }
      
      // Initialize neighbours
      foreach (PathNode node in childNodes) {
        node.AddNeighbours(GetAdjacentNodes(node));
      }
    }

    // Flow pathfinding algorithm
    {
      HashSet<PathNode> nodesToUpdate = new();
      
      targetNode.steps = 0;
      nodesToUpdate.Add(targetNode);
      
      // Update node steps
      while (nodesToUpdate.Count > 0) {
        PathNode currentNode = nodesToUpdate.First();

        nodesToUpdate.Remove(currentNode);

        foreach (PathNode neighbour in currentNode.GetNeighbours()) {
          if (neighbour.steps > currentNode.steps + 1) {
            neighbour.steps = currentNode.steps + 1;
            
            nodesToUpdate.Add(neighbour);
          }
        }
      }
      
      // Update next nodes
      foreach (PathNode node in childNodes) {
        foreach (PathNode neighbour in node.GetNeighbours()) {
          if (neighbour.steps < node.steps)
            node.AddNextNode(neighbour);
        }
      }
    }
  }
}