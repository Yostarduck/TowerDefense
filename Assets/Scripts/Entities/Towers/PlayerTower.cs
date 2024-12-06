using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTower : BaseTower
{
#region UNITY_METHODS

  /// <summary>
  /// Start is called before the first frame update.
  /// </summary>
  new protected void
  Start() {
    base.Start();

    OnDeathEvent += OnDeath;
  }

#endregion

#region METHODS

  /// <summary>
  /// Method called when the entity dies.
  /// Notifies the game controller that the player tower has died.
  /// </summary>
  protected void
  OnDeath() {
  }

#endregion
}