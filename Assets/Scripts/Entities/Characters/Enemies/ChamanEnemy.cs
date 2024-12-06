using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamanEnemy : BaseEnemy
{
#region CHAMAN_ENEMY_PROPERTIES

  [Header("Chaman Enemy Properties")]

  [SerializeField]
  protected int m_healAmount = 30;
  public int healAmount => m_healAmount;

  [SerializeField]
  protected float m_healRange = 2.0f;
  public float healRange => m_healRange;

  [SerializeField]
  protected float m_healDuration = 0.5f;
  public float healDuration => m_healDuration;
  
  [SerializeField]
  protected float m_healCooldown = 8.0f;
  public float healCooldown => m_healCooldown;

  protected bool canHeal = true;

#endregion

#region UNITY_METHODS
  
  /// <summary>
  /// Start is called before the first frame update
  /// </summary>
  new protected void
  Start() {
    base.Start();

    StartCoroutine(Logic());
  }

  /// <summary>
  /// Update is called once per frame
  /// </summary>
  new protected void
  Update() {
    base.Update();
  }

#endregion

#region METHODS

  /// <summary>
  /// Logic to move and heal enemies.
  /// </summary>
  private IEnumerator
  Logic() {
    while (true) {
      if (HealEnemies()) {
        yield return new WaitForSeconds(healDuration);
        
        StartCoroutine(CooldownHeal());
      }
      else {
        FollowPath();
      }

      yield return null;
    }
  }

  /// <summary>
  /// Heals all enemies in range.
  /// </summary>
  private bool
  HealEnemies() {
    if (!canHeal)
      return false;
    
    if (!EntitiesHandler.isInitialized) {
      Debug.LogWarning("EntitiesHandler not initialized", gameObject);
      return false;
    }

    HashSet<BaseEnemy> enemies = EntitiesHandler.instance.GetClosestEnemies(this, healRange);

    if (enemies == null)
      return false;

    if (enemies.Count == 0)
      return false;

    foreach (BaseEnemy enemy in enemies)
      enemy.Heal(healAmount);

    return true;
  }

  private IEnumerator
  CooldownHeal() {
    canHeal = false;

    yield return new WaitForSeconds(healCooldown);

    canHeal = true;
  }

#endregion
}