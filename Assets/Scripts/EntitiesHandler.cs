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

  public HashSet<BaseEntity> entities = new();
  public HashSet<BaseTower> towers = new();
  public HashSet<BaseCharacter> characters = new();
  public HashSet<BaseEnemy> enemies = new();

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

    if (entity is BaseTower tower) {
      towers.Remove(tower);
    }

    if (entity is BaseCharacter character) {
      characters.Remove(character);
    }

    if (entity is BaseEnemy enemy) {
      enemies.Remove(enemy);
    }
  }
  
#endregion

#region ENTITY_HANDLING_METHODS

  /// <summary>
  /// Get the closest object to an entity.
  /// </summary>
  /// <param name="entityReference">Entity from which to compare distance.</param>
  /// <param name="entities">List of entities to consider.</param>
  /// <param name="range">[Optional param] Maximum range to consider.</param>
  /// <returns>The closest entity to the entity, or null if none is found.</returns>
  private BaseEntity
  GetClosestEntity(BaseEntity entityReference, in HashSet<BaseEntity> entities, float? range = null) {
    if (entityReference == null)
      return null;
    
    if (entities.Count == 0)
      return null;

    if (range.HasValue && range.Value <= 0)
      return null;
    
    HashSet<BaseEntity> searchList = entities;
    searchList.Remove(entityReference);

    Vector2 position = new(entityReference.transform.position.x, entityReference.transform.position.z);
    BaseEntity closestEntity = null;
    float closestDistance = float.MaxValue;

    foreach (BaseEntity entity in searchList) {
      Vector2 entityPosition = new(entity.transform.position.x, entity.transform.position.z);
      float distance = Vector2.Distance(entityPosition, position);

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
  /// Get the entities in range of an entity.
  /// </summary>
  /// <param name="entityReference">Entity from which to compare distance.</param>
  /// <param name="entities">List of entities to consider.</param>
  /// <param name="range">Maximum range to consider.</param>
  /// <returns>The closest entity to the entity, or null if none is found.</returns>
  private HashSet<BaseEntity>
  GetClosestEntities(BaseEntity entityReference, in HashSet<BaseEntity> entities, float range) {
    if (entityReference == null)
      return null;
    
    if (entities.Count == 0)
      return null;

    if (range <= 0)
      return null;
    
    HashSet<BaseEntity> searchList = entities;
    searchList.Remove(entityReference);

    Vector2 position = new(entityReference.transform.position.x, entityReference.transform.position.z);
    HashSet<BaseEntity> closestEntities = new();

    foreach (BaseEntity entity in searchList) {
      Vector2 entityPosition = new(entity.transform.position.x, entity.transform.position.z);
      float distance = Vector2.Distance(entityPosition, position);

      if (distance < range) {
        closestEntities.Add(entity);
      }
    }

    return closestEntities;
  }

  /// <summary>
  /// Get the closest tower to an entity.
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

    if (range.HasValue && range.Value <= 0)
      return null;
    
    HashSet<BaseEntity> towerList = towers.Cast<BaseEntity>().ToHashSet();
    return GetClosestEntity(entityReference, towerList, range) as BaseTower;
  }

  /// <summary>
  /// Get the closests towers to an entity.
  /// </summary>
  /// <param name="entityReference"></param>
  /// <param name="range"></param>
  /// <returns></returns>
  public HashSet<BaseTower>
  GetClosestTowers(BaseEntity entityReference, float range) {
    if (entityReference == null)
      return null;

    if (towers.Count == 0)
      return null;

    if (range <= 0)
      return null;
    
    HashSet<BaseEntity> towerList = towers.Cast<BaseEntity>().ToHashSet();
    return GetClosestEntities(entityReference, towerList, range)?.Cast<BaseTower>().ToHashSet();
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

    if (range.HasValue && range.Value <= 0)
      return null;
    
    HashSet<BaseEntity> characterList = characters.Cast<BaseEntity>().ToHashSet();
    return GetClosestEntity(entityReference, characterList, range) as BaseCharacter;
  }
  
  /// <summary>
  /// Get the closest characters to an entity.
  /// </summary>
  /// <param name="entityReference">Entity from which to compare distance.</param>
  /// <param name="range">[Optional param] Maximum range to consider.</param>
  /// <returns>The closest characters to the position, or null if none is found.</returns>
  public HashSet<BaseCharacter>
  GetClosestCharacters(BaseEntity entityReference, float range) {
    if (entityReference == null)
      return null;

    if (characters.Count == 0)
      return null;

    if (range <= 0)
      return null;
    
    HashSet<BaseEntity> characterList = characters.Cast<BaseEntity>().ToHashSet();
    return GetClosestEntities(entityReference, characterList, range)?.Cast<BaseCharacter>().ToHashSet();
  }
  
  /// <summary>
  /// Get the closest enemiy to an entity.
  /// </summary>
  /// <param name="entityReference">Entity from which to compare distance.</param>
  /// <param name="range">[Optional param] Maximum range to consider.</param>
  /// <returns>The closest enemy to the position, or null if none is found.</returns>
  public BaseEnemy
  GetClosestEnemy(BaseEntity entityReference, float? range) {
    if (entityReference == null)
      return null;

    if (enemies.Count == 0)
      return null;

    if (range.HasValue && range.Value <= 0)
      return null;
    
    HashSet<BaseEntity> enemyList = enemies.Cast<BaseEntity>().ToHashSet();
    return GetClosestEntity(entityReference, enemyList, range) as BaseEnemy;
  }

  /// <summary>
  /// Get the closest enemies to an entity.
  /// </summary>
  /// <param name="entityReference">Entity from which to compare distance.</param>
  /// <param name="range">[Optional param] Maximum range to consider.</param>
  /// <returns>The closest enemy to the position, or null if none is found.</returns>
  public HashSet<BaseEnemy>
  GetClosestEnemies(BaseEntity entityReference, float range) {
    if (entityReference == null)
      return null;

    if (enemies.Count == 0)
      return null;

    if (range <= 0)
      return null;
    
    HashSet<BaseEntity> enemyList = enemies.Cast<BaseEntity>().ToHashSet();
    return GetClosestEntities(entityReference, enemyList, range).Cast<BaseEnemy>().ToHashSet();
  }

#endregion
}