using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : BaseEntity
{
  #region PROPERTIES
  [Header("Base Character Properties")]
  [SerializeField]
  protected float movementSpeed = 5.0f;
  #endregion

  /// <summary>
  /// Start is called before the first frame update
  /// </summary>
  void
  Start() {
  }

  /// <summary>
  /// Update is called once per frame
  /// </summary>
  void
  Update() {
  }
}