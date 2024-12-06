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
  new void
  Start() {
    base.Start();

    StartCoroutine(AttackRoutine());
  }

  /// <summary>
  /// Update is called once per frame.
  /// </summary>
  new void
  Update() {
    base.Update();

    UpdateFocusList();
  }
#endregion

  private IEnumerator
  AttackRoutine() {
    while (true) {
      yield return new WaitForSeconds(attackCooldown);

      Attack();
    }
  }

  private void
  UpdateFocusList() {
    if (!EntitiesHandler.isInitialized) {
      Debug.LogWarning("EntitiesHandler not initialized", gameObject);
      return;
    }

    Vector2 ourPosition = new(transform.position.x, transform.position.z);

    HashSet<BaseEntity> enemiesInRange = new();

    foreach (BaseEnemy enemy in EntitiesHandler.instance.enemies) {
      if (enemy == null)
        continue;
      
      Vector2 enemyPosition = new(enemy.transform.position.x, enemy.transform.position.z);

      float distance = Vector2.Distance(ourPosition, enemyPosition);

      if (distance <= attackRange)
        enemiesInRange.Add(enemy);
    }
    
    int newEnemiesToCount = enemiesInRange.Where(enemy => !focusList.Contains(enemy)).Count();
    int oldEnemiesToCount = focusList.Where(enemy => !enemiesInRange.Contains(enemy)).Count();

    if (newEnemiesToCount > 0)
      OnFocusGainEvent?.Invoke();

    if (oldEnemiesToCount > 0)
      OnFocusLostEvent?.Invoke();

    focusList = enemiesInRange;
  }

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