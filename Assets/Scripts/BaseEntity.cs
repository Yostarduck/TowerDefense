using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
#region PROPERTIES
  [Header("Base Entity Properties")]
  
  [SerializeField] protected bool canFly = false;

  public bool isDead => health <= 0;
  public bool isAttacking { get; protected set; } = false;

  protected List<BaseEntity> focusList = new();
  
  [Header("Health Properties")]

  [SerializeField]
  protected int maxHealth = 100;
  public int health { get; protected set; }
  
  [Header("Attack Properties")]

  [SerializeField] protected int attackDamage = 10;
  [SerializeField] protected float attackRange = 1.0f;
  [SerializeField] protected float attackCooldown = 1.0f;
  [SerializeField] protected float attackDuration = 0.5f;
#endregion

#region ACTIONS
  // Called when the entity spawns
  public Action OnSpawn;

  // Called when the entity focuses a target
  public Action OnFocusGain;

  // Called when the entity loses focus on a target
  public Action OnFocusLost;

  // Called when the entity attacks
  public Action OnAttack;

  // Called when the entity is damaged
  public Action OnDamage;

  // Called when the entity dies
  public Action OnDeath;

  // Called when the entity is healed
  public Action OnHeal;
  
  // Called when the entity is destroyed
  public Action OnDestroy;
#endregion

#region UNITY_METHODS

  /// <summary>
  /// Start is called before the first frame update.
  /// </summary>
  void
  Start() {
    health = maxHealth;
    OnSpawn?.Invoke();
  }

  /// <summary>
  /// Update is called once per frame.
  /// </summary>
  void
  Update() {
  }

#endregion

#region BASE_ENTITY_METHODS
  /// <summary>
  /// Attack focused target.
  /// 
  /// Calls OnAttack event.
  /// </summary>
  protected void
  Attack() {
    if (focusList.Count == 0)
      return;

    BaseEntity target = focusList[0];

    target.Damage(attackDamage);

    OnAttack?.Invoke();
  }
  
  /// <summary>
  /// Damages this entity.
  /// If health reaches 0, entity dies and Die() is called.
  /// 
  /// Calls OnDamage event.
  /// </summary>
  /// <param name="damage">Amount of damage to receive</param>
  public void
  Damage(int damage) {
    health -= damage;

    OnDamage?.Invoke();

    if (health <= 0)
      Die();
  }
  
  /// <summary>
  /// Heals this entity, healt is capped at maxHealth
  /// 
  /// Calls OnHeal event.
  /// </summary>
  /// <param name="heal">Amount to heal</param>
  public void
  Heal(int heal) {
    OnHeal?.Invoke();

    health = Math.Min(health + heal, maxHealth);
  }
  
  /// <summary>
  /// Kills this entity.
  /// 
  /// Calls OnDeath and OnDestroy events.
  /// </summary>
  protected void
  Die() {
    OnDeath?.Invoke();
    OnDestroy?.Invoke();
    Destroy(gameObject);
  }
#endregion
}