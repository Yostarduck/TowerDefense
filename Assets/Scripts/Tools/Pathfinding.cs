using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
#region SINGLETON
  public static Pathfinding instance {
    get => m_instance;
    private set => m_instance = value;
  }

  private static Pathfinding m_instance = null;

  public static bool isInitialized => instance != null;
#endregion

#region PROPERTIES
  [SerializeField]
  private bool autoBuild = true;

  public bool built { get; private set; } = false;

  [SerializeField]
  private PathNode targetNode;

  private Dictionary<Vector2Int, PathNode> gridNodes;
#endregion

#region UNITY_METHODS

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  private void
  Awake() {
    if (instance == null) {
      instance = this;
    }
    else {
      Debug.LogWarning("Multiple instances of Pathfinding found", gameObject);
      Destroy(gameObject);
    }
  }

  /// <summary>
  /// Start is called before the first frame update.
  /// </summary>
  private void
  Start() {
    if (autoBuild)
      BuildPath();
  }
  
#endregion

  /// <summary>
  /// Utility function to build the pathfinding grid.
  /// </summary>
  private void
  BuildPath() {
    PathNode[] childNodes = transform.GetComponentsInChildren<PathNode>();
    
    // Safety checks
    {
      if (childNodes.Length == 0) {
        Debug.LogError("No nodes found in child", gameObject);
        return;
      }

      if (targetNode == null) {
        Debug.LogError("No target node set", gameObject);
        return;
      }

      if (!childNodes.Contains(targetNode)) {
        Debug.LogError("Target node not found in pathfinding", gameObject);
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

        Vector2Int[] adjacentPositions = {
          Vector2Int.up,
          Vector2Int.down,
          Vector2Int.right,
          Vector2Int.left
        };

        foreach (Vector2Int position in adjacentPositions) {
          gridNodes.TryGetValue(node.GetPosition() + position, out PathNode adjacentNode);
          
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

    built = true;
  }

/// <summary>
/// Get a path from a given position to the target node.
/// </summary>
/// <param name="position">Position to start the path from.</param>
/// <returns></returns>
  public List<Vector2Int>
  GetPathFromPosition(Vector2Int position) {
    // Safety checks
    {
      if (!built) {
        Debug.LogWarning("Pathfinding not built", gameObject);
        return null;
      }

      if (targetNode == null) {
        Debug.LogWarning("No target node set", gameObject);
        return null;
      }
    }

    if (!gridNodes.TryGetValue(position, out PathNode startNode)) {
      Debug.LogWarning("Start node not found", gameObject);
      return null;
    }

    List<Vector2Int> path = new();

    PathNode currentNode = startNode;

    while (currentNode != targetNode) {
      path.Add(currentNode.GetPosition());

      List<PathNode> nextNodes = currentNode.GetNextNodes();

      if (nextNodes == null) {
        Debug.LogWarning("No next nodes found", gameObject);
        return null;
      }

      if (nextNodes.Count == 0) {
        Debug.LogWarning("No next nodes found", gameObject);
        return null;
      }

      PathNode nextNode = nextNodes[Random.Range(0, nextNodes.Count)];

      currentNode = nextNode;
    }

    path.Add(targetNode.GetPosition());

    return path;
  }
}