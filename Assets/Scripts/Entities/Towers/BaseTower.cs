using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseTower : BaseEntity
{
#region UNITY_METHODS

  /// <summary>
  /// Start is called before the first frame update.
  /// </summary>
  new protected void
  Start() {
    base.Start();
  }

  /// <summary>
  /// Update is called once per frame.
  /// </summary>
  new protected void
  Update() {
    base.Update();

    UpdateFocusList();
  }

#endregion

#region PROPERTIES

  /// <summary>
  /// Updates the enemies to focus on.
  /// </summary>
  private void
  UpdateFocusList() {
    if (!EntitiesHandler.isInitialized) {
      Debug.LogWarning("EntitiesHandler not initialized", gameObject);
      return;
    }

    if (attackRange <= 0)
      return;

    HashSet<BaseEnemy> enemiesInRange = EntitiesHandler.instance.GetClosestEnemies(this, attackRange);

    if (enemiesInRange == null) {
      if (focusList.Count > 0) {
        OnFocusLostEvent?.Invoke();

        focusList.Clear();
      }
      
      return;
    }

    if (enemiesInRange == null) {
      if (focusList.Count > 0) {
        OnFocusLostEvent?.Invoke();

        focusList.Clear();
      }
      
      return;
    }
    
    int newEnemiesToCount = enemiesInRange.Where(enemy => !focusList.Contains(enemy)).Count();
    int oldEnemiesToCount = focusList.Where(enemy => !enemiesInRange.Contains(enemy)).Count();

    if (newEnemiesToCount > 0)
      OnFocusGainEvent?.Invoke();

    if (oldEnemiesToCount > 0)
      OnFocusLostEvent?.Invoke();

    focusList = enemiesInRange.Cast<BaseEntity>().ToHashSet();
  }

#endregion

#if UNITY_EDITOR

  private void
  OnDrawGizmos() {
    int circleResolution = 64;

    Gizmos.color = Color.red;

    for (int i = 0; i < circleResolution; i++) {
      float angle = i * 2 * Mathf.PI / circleResolution;
      Vector3 pointA = new Vector3(Mathf.Cos(angle) * attackRange, 0, Mathf.Sin(angle) * attackRange);
      angle = (i + 1) * 2 * Mathf.PI / circleResolution;
      Vector3 pointB = new Vector3(Mathf.Cos(angle) * attackRange, 0, Mathf.Sin(angle) * attackRange);

      Gizmos.DrawLine(transform.position + pointA, transform.position + pointB);
    }
  }

#endif
}