using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
#region PROPERTIES
  [Header("Base Entity Properties")]
  
  protected HashSet<BaseEntity> focusList = new();
  public bool isDead => health <= 0;
  public bool isAttacking { get; protected set; } = false;
  
  [Header("Health Properties")]

  [SerializeField]
  protected int maxHealth = 100;
  public int health { get; protected set; }
  
  [Header("Attack Properties")]

  [SerializeField] protected int m_attackDamage = 10;
  public int attackDamage => m_attackDamage;
  [SerializeField] protected float m_attackRange = 1.0f;
  public float attackRange => m_attackRange;
  [SerializeField] protected float m_attackCooldown = 1.0f;
  public float attackCooldown => m_attackCooldown;
  [SerializeField] protected float m_attackDuration = 0.5f;
  public float attackDuration => m_attackDuration;
#endregion

#region ACTIONS
  // Called when the entity spawns
  public Action OnSpawnEvent;

  // Called when the entity focuses a target
  public Action OnFocusGainEvent;

  // Called when the entity loses focus on a target
  public Action OnFocusLostEvent;

  // Called when the entity attacks
  public Action OnAttackEvent;

  // Called when the entity is damaged
  public Action OnDamageEvent;

  // Called when the entity dies
  public Action OnDeathEvent;

  // Called when the entity is healed
  public Action OnHealEvent;
  
  // Called when the entity is destroyed
  public Action OnDestroyEvent;
#endregion

#region UNITY_METHODS

  /// <summary>
  /// Start is called before the first frame update.
  /// </summary>
  protected void
  Start() {
    if (!EntitiesHandler.isInitialized) {
      Debug.LogWarning("EntitiesHandler not initialized", gameObject);
      return;
    }
    else {
      EntitiesHandler.instance.RegisterEntity(this);
    }

    health = maxHealth;
    OnSpawnEvent?.Invoke();
  }

  /// <summary>
  /// Update is called once per frame.
  /// </summary>
  protected void
  Update() {
  }
  
  /// <summary>
  /// 
  /// </summary>
  protected void
  OnDestroy() {
    OnDestroyEvent?.Invoke();
  }

#endregion

#region BASE_ENTITY_METHODS
  /// <summary>
  /// Attack target.
  /// 
  /// Calls OnAttackEvent event.
  /// </summary>
  /// <param name="target">Target to attack</param>
  protected void
  Attack(BaseEntity target) {
    if (target == null)
      return;

    target.Damage(attackDamage);

    OnAttackEvent?.Invoke();
  }
  
  /// <summary>
  /// Damages this entity.
  /// If health reaches 0, entity dies and Die() is called.
  /// 
  /// Calls OnDamageEvent event.
  /// </summary>
  /// <param name="damage">Amount of damage to receive</param>
  public void
  Damage(int damage) {
    damage = Math.Min(0, -damage);

    health += damage;

    if (DamageParticles.isInitialized)
      DamageParticles.instance.PlayDamageParticles(transform.position, damage);

    OnDamageEvent?.Invoke();

    if (health <= 0)
      Die();
  }
  
  /// <summary>
  /// Heals this entity, healt is capped at maxHealth
  /// 
  /// Calls OnHealEvent event.
  /// </summary>
  /// <param name="heal">Amount to heal</param>
  public void
  Heal(int heal) {
    heal = Math.Max(0, heal);

    OnHealEvent?.Invoke();
    
    if (DamageParticles.isInitialized)
      DamageParticles.instance.PlayDamageParticles(transform.position, heal);

    health = Math.Min(health + heal, maxHealth);
  }
  
  /// <summary>
  /// Kills this entity.
  /// 
  /// Calls OnDeathEvent and OnDestroyEvent events.
  /// </summary>
  protected void
  Die() {
    if (!EntitiesHandler.isInitialized) {
      Debug.LogWarning("EntitiesHandler not initialized", gameObject);
      return;
    }
    else {
      EntitiesHandler.instance.UnregisterEntity(this);
    }

    OnDeathEvent?.Invoke();
    
    Destroy(gameObject);
  }
#endregion
}