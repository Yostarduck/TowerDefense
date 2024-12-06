using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonEnemy : BaseEnemy
{
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
      FollowPath();

      yield return null;
    }
  }

#endregion
}