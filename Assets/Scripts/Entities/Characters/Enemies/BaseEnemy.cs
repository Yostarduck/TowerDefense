using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseCharacter
{
#region UNITY_METHODS

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

    if (!Pathfinding.instance.isBuilt) {
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
  }
  
  /// <summary>
  /// Called when the object enters a trigger collider.
  /// </summary>
  /// <param name="other"></param>
  protected void
  OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
      PlayerTower playerTower = other.GetComponent<PlayerTower>();

      playerTower.Damage(attackDamage);
      
      Die();
    }
  }

#endregion
}