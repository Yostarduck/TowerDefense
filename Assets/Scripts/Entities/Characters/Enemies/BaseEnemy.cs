using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseCharacter
{

  /// <summary>
  /// Start is called before the first frame update
  /// </summary>
  new protected void
  Start() {
    base.Start();

    if (!Pathfinding.isInitialized) {
      Debug.LogWarning("Pathfinding not initialized", gameObject);
      return;
    }

    if (!Pathfinding.instance.built) {
      Debug.LogWarning("Pathfinding not built", gameObject);
      return;
    }
    
    Vector2Int sourcePosition = new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    path = Pathfinding.instance.GetPathFromPosition(sourcePosition);
  }

  /// <summary>
  /// Update is called once per frame
  /// </summary>
  new protected void
  Update() {
    base.Update();

    FollowPath();
  }
}