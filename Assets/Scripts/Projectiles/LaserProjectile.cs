using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
#region PROPERTIES

  [Header("Laser Projectile Properties")]

  [SerializeField]
  protected LineRenderer lineRenderer;
  
  [HideInInspector]
  public BaseEntity owner = null;
  [HideInInspector]
  public BaseEntity target = null;

  protected BaseCharacter characterTarget = null;

  protected Vector3 sourcePosition;
  protected Vector3 targetPosition;

#endregion

#region UNITY_METHODS

  /// <summary>
  /// Start is called before the first frame update.
  /// </summary>
  protected void
  Start() {
    sourcePosition = transform.position;

    characterTarget = target as BaseCharacter;

    lineRenderer.positionCount = 2;
    UpdateLaserLine();
  }

  /// <summary>
  /// Update is called once per frame.
  /// </summary>
  protected void
  Update() {
    if (target == null) {
      Destroy(gameObject);
      return;
    }
    
    UpdateLaserLine();
  }

#endregion

#region METHODS

  /// <summary>
  /// Update the line renderer.
  /// </summary>
  public void
  UpdateLaserLine() {
    sourcePosition = transform.position;

    lineRenderer.SetPosition(0, sourcePosition);

    if (characterTarget != null)
      targetPosition = characterTarget.transform.position + Vector3.up * characterTarget.headOffset;
    else
      targetPosition = target.transform.position;

    lineRenderer.SetPosition(1, targetPosition);
  }

#endregion
}