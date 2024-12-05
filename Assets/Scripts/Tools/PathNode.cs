using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathNode : MonoBehaviour
{
  private Pathfinding pathfinding;
  private List<PathNode> neighbours = new List<PathNode>();
  private List<PathNode> nextNodes = new List<PathNode>();

  public int steps = int.MaxValue;

#region GETTERS

  public (int x, int y)
  GetPosition() => ((int)transform.position.x, (int)transform.position.z);

  public List<PathNode>
  GetNeighbours() => neighbours;
  
  public int
  GetNeighbourCount() => neighbours.Count;

  public List<PathNode>
  GetNextNodes() => nextNodes;

  public int
  GetNextNodeCount() => nextNodes.Count;

#endregion

#region SETTETS

  public void
  SetPathfinding(Pathfinding pathfinding) {
    this.pathfinding = pathfinding;
  }

#endregion

#region ADDERS

  public void
  AddNeighbour(PathNode neighbour) {
    if (!neighbours.Contains(neighbour))
      neighbours.Add(neighbour);
  }

  public void
  AddNeighbours(List<PathNode> neighbours) {
    foreach (PathNode neighbour in neighbours)
      AddNeighbour(neighbour);
  }

  public void
  AddNextNode(PathNode node) {
    if (node == null)
      return;

    if (!nextNodes.Contains(node))
      nextNodes.Add(node);
  }

  public void
  AddNextNodes(List<PathNode> nodes) {
    if (nodes == null)
      return;
      
    foreach (PathNode node in nodes)
      AddNextNode(node);
  }

#endregion

#if UNITY_EDITOR
  private void
  OnValidate() {
    transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
  }

  private void
  OnDrawGizmos() {
    if (nextNodes == null)
      return;

    Vector3 offset = new Vector3(0, 0.25f, 0);

    foreach (PathNode nextNode in nextNodes) {
      if (nextNodes == null)
        continue;

      Vector3 origin = transform.position + offset;
      Vector3 target = nextNode.transform.position + offset;
      Vector3 direction = (target - origin).normalized;

      Vector3 head = origin + direction * 0.5f;
      Vector3 right = head + Quaternion.Euler(0, 135, 0) * direction * 0.25f;
      Vector3 left = head + Quaternion.Euler(0, -135, 0) * direction * 0.25f;

      Gizmos.color = Color.green;

      Gizmos.DrawLine(origin, head);
      Gizmos.DrawLine(head,   right);
      Gizmos.DrawLine(head,   left);
    }
  }
#endif
}