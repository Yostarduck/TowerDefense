using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserTower : BaseTower
{
#region LASER_TOWER_PROPERTIES

    [Header("Laser Tower Properties")]

    [SerializeField]
    protected GameObject laserPrefab = null;
  
    [SerializeField]
    protected Transform laserSpawnPoint = null;
    
    protected GameObject laserInstance = null;

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

      bool enemyOnFocus = true;

      if (laserPrefab == null) {
        Debug.LogWarning("Laser prefab is null", gameObject);
        continue;
      }

      if (laserInstance != null) {
        Destroy(laserInstance);
      }

      laserInstance = Instantiate(laserPrefab, laserSpawnPoint.position, Quaternion.identity);
      LaserProjectile laserProjectile = laserInstance.GetComponent<LaserProjectile>();

      if (laserProjectile == null) {
        Debug.LogWarning("Laser projectile is null", gameObject);
        continue;
      }

      laserProjectile.owner = this;
      laserProjectile.target = enemy;

      while (enemyOnFocus) {
        enemy.Damage(attackDamage);

        yield return new WaitForSeconds(attackDuration);
        
        enemyOnFocus = focusList.Contains(enemy);
      }

      if (laserInstance != null) {
        Destroy(laserInstance);
      }
      
      if (enemy != null)
        enemy.Damage(attackDamage);

      yield return new WaitForSeconds(attackCooldown);
    }
  }

#endregion
}