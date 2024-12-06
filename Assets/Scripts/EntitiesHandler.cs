using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntitiesHandler : MonoBehaviour
{
#region SINGLETON

  public static EntitiesHandler instance {
    get => m_instance;
    private set => m_instance = value;
  }

  private static EntitiesHandler m_instance = null;

  public static bool isInitialized => instance != null;

#endregion

#region PROPERTIES

  public List<BaseEntity> entities = new();
  public List<BaseTower> towers = new();
  public List<BaseCharacter> characters = new();
  public List<BaseEnemy> enemies = new();

#endregion

#region UNITY_METHODS

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  private void
  Awake() {
    if (instance == null) {
      instance = this;
    }
    else {
      Debug.LogWarning("Multiple instances of EntitiesHandler found", gameObject);
      Destroy(gameObject);
    }
  }

  /// <summary>
  /// Start is called before the first frame update
  /// </summary>
  void
  Start() {
    Initialize();
  }

#endregion

#region METHODS

  /// <summary>
  /// Initialize the handler.
  /// </summary>
  private void
  Initialize() {
    entities = new();
    characters = new();
    enemies = new();
  }

  /// <summary>
  /// Register an entity to the handler.
  /// </summary>
  /// <param name="entity">The entity to register.</param>
  public void
  RegisterEntity(BaseEntity entity) {
    entities.Add(entity);

    if (entity is BaseTower tower) {
      towers.Add(tower);
    }

    if (entity is BaseCharacter character) {
      characters.Add(character);
    }

    if (entity is BaseEnemy enemy) {
      enemies.Add(enemy);
    }
  }
  
  /// <summary>
  /// Unregister an entity from the handler.
  /// </summary>
  /// <param name="entity">The entity to unregister.</param>
  public void
  UnregisterEntity(BaseEntity entity) {
    entities.Remove(entity);

    if (entity is BaseCharacter character) {
      characters.Remove(character);
    }

    if (entity is BaseEnemy enemy) {
      enemies.Remove(enemy);
    }
  }
  
  /// <summary>
  /// Get the closest object to an entity.
  /// </summary>
  /// <param name="entityReference">Entity from which to compare distance.</param>
  /// <param name="entities">List of entities to consider.</param>
  /// <param name="range">[Optional param] Maximum range to consider.</param>
  /// <returns>The closest entity to the entity, or null if none is found.</returns>
  private BaseEntity
  GetClosestEntity(BaseEntity entityReference, in List<BaseEntity> entities, float? range = null) {
    if (entityReference == null)
      return null;
    
    if (entities.Count == 0)
      return null;
    
    List<BaseEntity> searchList = entities.FindAll(entity => entity != entityReference);

    Vector3 position = entityReference.transform.position;
    BaseEntity closestEntity = null;
    float closestDistance = float.MaxValue;

    foreach (BaseEntity entity in searchList) {
      float distance = Vector3.Distance(entity.transform.position, position);

      if (distance < closestDistance) {
        closestEntity = entity;
        closestDistance = distance;
      }
    }

    if (range.HasValue && closestDistance > range.Value)
      return null;

    return closestEntity;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="entityReference"></param>
  /// <param name="range"></param>
  /// <returns></returns>
  public BaseTower
  GetClosestTower(BaseEntity entityReference, float? range = null) {
    if (entityReference == null)
      return null;

    if (towers.Count == 0)
      return null;
    
    List<BaseEntity> towerList = towers.Cast<BaseEntity>().ToList();
    return GetClosestEntity(entityReference, towerList, range) as BaseTower;
  }
  
  /// <summary>
  /// Get the closest character to an entity.
  /// </summary>
  /// <param name="entityReference">Entity from which to compare distance.</param>
  /// <param name="range">[Optional param] Maximum range to consider.</param>
  /// <returns>The closest character to the position, or null if none is found.</returns>
  public BaseCharacter
  GetClosestCharacter(BaseEntity entityReference, float? range = null) {
    if (entityReference == null)
      return null;

    if (characters.Count == 0)
      return null;
    
    List<BaseEntity> characterList = characters.Cast<BaseEntity>().ToList();
    return GetClosestEntity(entityReference, characterList, range) as BaseCharacter;
  }
  
  /// <summary>
  /// Get the closest enemy to an entity.
  /// </summary>
  /// <param name="entityReference">Entity from which to compare distance.</param>
  /// <param name="range">[Optional param] Maximum range to consider.</param>
  /// <returns>The closest enemy to the position, or null if none is found.</returns>
  public BaseEnemy
  GetClosestEnemy(BaseEntity entityReference, float? range = null) {
    if (entityReference == null)
      return null;

    if (enemies.Count == 0)
      return null;
    
    List<BaseEntity> enemyList = enemies.Cast<BaseEntity>().ToList();
    return GetClosestEntity(entityReference, enemyList, range) as BaseEnemy;
  }

#endregion
}