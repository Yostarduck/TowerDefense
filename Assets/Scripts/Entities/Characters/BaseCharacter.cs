using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : BaseEntity
{
#region PROPERTIES

  [Header("Base Character Properties")]

  [SerializeField]
  private float m_headOffset = 0.5f;
  public float headOffset => m_headOffset;

  [SerializeField]
  private bool m_canFly = false;
  public bool canFly => m_canFly;

  [Header("Movement Properties")]

  [SerializeField]
  protected float m_movementSpeed = 5.0f;
   
  public float movementSpeed => m_movementSpeed;

  public bool isMoving { get; protected set; }

  protected List<Vector2Int> path;

#endregion

  /// <summary>
  /// Returns the path if any, null otherwise.
  /// </summary>
  /// <returns>Path</returns>
  public List<Vector2Int>
  GetPath() {
    return path;
  }
  
  /// <summary>
  /// Moves the enemy along the path.
  /// </summary>
  protected void
  FollowPath() {
    isMoving = false;

    if (path == null)
      return;

    if (path.Count == 0)
      return;
      
    isMoving = true;

    Vector2Int targetPosition = path[0];
    Vector3 target = new Vector3(targetPosition.x, transform.position.y, targetPosition.y);
    transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
    
    float distance = Vector3.Distance(transform.position, target);
    if (distance <= 0.1f) {
      path.RemoveAt(0);
    }
  }

}