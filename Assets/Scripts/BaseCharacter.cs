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
}