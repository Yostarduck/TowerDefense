using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemy : BaseEnemy
{
#region FLY_ENEMY_PROPERTIES
  [Header("Fly Enemy Properties")]

  [SerializeField]
  protected float m_flyPause = 0.2f;
  public float flyPause => m_flyPause;

  [SerializeField]
  protected float m_flyDuration = 0.5f;
  public float flyDuration => m_flyDuration;

  protected bool flyIsOnPause = true;
#endregion

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
      yield return new WaitForSeconds(flyPause);
      
      StartCoroutine(FlyPause());

      while (!flyIsOnPause) {
        FollowPath();
        
        yield return null;
      }
      
      yield return null;
    }
  }

  private IEnumerator
  FlyPause() {
    flyIsOnPause = false;

    yield return new WaitForSeconds(flyDuration);

    flyIsOnPause = true;
  }

#endregion
}