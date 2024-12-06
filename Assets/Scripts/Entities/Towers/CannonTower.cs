using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CannonTower : BaseTower
{
#region PROPERTIES

  [Header("Cannon Tower Properties")]
  [SerializeField]
  private GameObject cannonBallPrefab = null;

  [SerializeField]
  private Transform cannonBallSpawnPoint = null;
#endregion

#region UNITY_METHODS

  /// <summary>
  /// Start is called before the first frame update.
  /// </summary>
  new protected void
  Start() {
    base.Start();

    StartCoroutine(AttackCoroutine());
  }

  /// <summary>
  /// Update is called once per frame.
  /// </summary>
  new protected void
  Update() {
    base.Update();
  }

#endregion

#region METHODS

  /// <summary>
  /// Attack the target.
  /// </summary>
  private IEnumerator
  AttackCoroutine() {
    while (true) {
      yield return new WaitForSeconds(attackCooldown);

      isAttacking = false;

      while (focusList.Count <= 0) {
        yield return null;
      }

      isAttacking = true;

      BaseEntity target = focusList.First(x => x != null);

      BaseEnemy enemy = target as BaseEnemy;

      if (enemy == null) {
        Debug.LogWarning("Target is not a enemy", gameObject);
        continue;
      }

      if (cannonBallPrefab == null) {
        Debug.LogWarning("CannonBall prefab not set", gameObject);
        continue;
      }

      if (cannonBallSpawnPoint == null) {
        Debug.LogWarning("CannonBall spawn point not set", gameObject);
        continue;
      }
      
      GameObject cannonBall = Instantiate(cannonBallPrefab, cannonBallSpawnPoint.position, Quaternion.identity);
      CannonBallProjectile projectile = cannonBall.GetComponent<CannonBallProjectile>();
      if (projectile == null) {
        Debug.LogWarning("CannonBall prefab does not have a BaseProjectile component", gameObject);
        continue;
      }

      projectile.owner = this;
      projectile.target = target;
    }
  }

#endregion
}