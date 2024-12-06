using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageParticles : MonoBehaviour
{
#region SINGLETON

  public static DamageParticles instance {
    get => m_instance;
    private set => m_instance = value;
  }

  private static DamageParticles m_instance = null;

  public static bool isInitialized => instance != null;

#endregion

#region PROPERTIES

  [Header("Damage Particles Properties")]

  [SerializeField]
  protected ParticleSystem damageParticlesSystem = null;
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
  /// Start is called before the first frame update.
  /// </summary>
  protected void
  Start() {
    if (damageParticlesSystem == null) {
      Debug.LogError("Damage Particles System is not set", gameObject);
    }
  }

#endregion

#region METHODS

  /// <summary>
  /// Play the damage particles.
  /// </summary>
  public void
  PlayDamageParticles(Vector3 position, int damage) {
    if (damageParticlesSystem == null)
      return;

    ParticleSystem.EmitParams emitParams = new();
    emitParams.position = position;
    emitParams.startSize3D = new(1.0f, 0.25f, damage);
    emitParams.velocity = new(Random.Range(-0.1f, 0.1f), 1.0f, Random.Range(-0.1f, 0.1f));
    emitParams.velocity = emitParams.velocity.normalized * damageParticlesSystem.main.startSpeed.constant;

    damageParticlesSystem.Emit(emitParams, 1);
  }

#endregion
}