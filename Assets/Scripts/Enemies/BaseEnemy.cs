using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseCharacter
{
  private List<Vector2Int> path;

  /// <summary>
  /// Start is called before the first frame update
  /// </summary>
  void
  Start() {
    if (!Pathfinding.isInitialized) {
      Debug.LogWarning("Pathfinding not initialized", gameObject);
      return;
    }

    if (!Pathfinding.instance.built) {
      Debug.LogWarning("Pathfinding not built", gameObject);
      return;
    }
      
    path = Pathfinding.instance.GetPathFromPosition(new((int)transform.position.x, (int)transform.position.z));
  }

  /// <summary>
  /// Update is called once per frame
  /// </summary>
  void
  Update() {
    if (path == null)
      return;

    if (path.Count == 0)
      return;

    Vector2Int targetPosition = path[0];
    Vector3 target = new Vector3(targetPosition.x, transform.position.y, targetPosition.y);
    transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);

    if (transform.position == target) {
      path.RemoveAt(0);
    }
  }
}