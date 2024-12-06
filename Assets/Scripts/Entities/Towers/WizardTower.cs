using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardTower : BaseTower
{
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

      foreach (BaseEntity target in focusList) {
        BaseEnemy enemy = target as BaseEnemy;

        if (enemy == null) {
          Debug.LogWarning("Target is not a enemy", gameObject);
          continue;
        }

        if (enemy.canFly)
          continue;
        
        enemy.Damage(attackDamage);
      }
    }
  }

#endregion
}