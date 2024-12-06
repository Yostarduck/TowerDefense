using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WaveOrigin
{
  public GameObject origin;
  public List<EnemyWaveScriptableObject> waves;
}

public class WavesController : MonoBehaviour
{
#region SINGLETON
  public static WavesController instance {
    get => m_instance;
    private set => m_instance = value;
  }

  private static WavesController m_instance = null;

  public static bool isInitialized => instance != null;
#endregion

#region PROPERTIES
  public WaveOrigin[] waveOrigins;

  private bool startNextWave = false;

  private int currentWave = 0;
  public int CurrentWave => currentWave;

  private int pendingWaves = 0;

  private Coroutine wavesCoroutine;

#endregion

#region UNITY_METHODS

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void
  Awake() {
    if (instance == null) {
      instance = this;
    }
    else {
      Debug.LogWarning("Multiple instances of WavesController found", gameObject);
      Destroy(gameObject);
    }
  }

#endregion

#region METHODS

  /// <summary>
  /// Start the waves.
  /// </summary>
  public void
  StartWaves() {
    if (wavesCoroutine != null)
      StopCoroutine(wavesCoroutine);
    
    currentWave = 0;
    pendingWaves = 0;

    wavesCoroutine = StartCoroutine(StartWavesCoroutine());
  }
  
  /// <summary>
  /// Start the next wave.
  /// </summary>
  public void
  StartNextWave() {
    startNextWave = true;
  }

  /// <summary>
  /// Get the max number of waves.
  /// </summary>
  /// <returns>Max number of waves</returns>
  public int
  GetMaxWaves() {
    int maxWaves = 0;

    foreach (WaveOrigin waveOrigin in waveOrigins) {
      if (waveOrigin.origin == null)
        continue;
        
      maxWaves = Mathf.Max(maxWaves, waveOrigin.waves.Count);
    }

    return maxWaves;
  }

  private IEnumerator
  StartWavesCoroutine() {
    int maxWaves = GetMaxWaves();

    for (currentWave = 0; currentWave < maxWaves; currentWave++) {

      GameController.instance?.EnableNextWave();

      while (!startNextWave) {
        yield return null;
      }

      startNextWave = false;

      foreach (WaveOrigin waveOrigin in waveOrigins) {
        if (waveOrigin.origin == null)
          continue;

        if (waveOrigin.waves.Count <= currentWave)
          continue;

        EnemyWaveScriptableObject wave = waveOrigin.waves[currentWave];

        if (wave == null)
          continue;

        Vector3 origin = waveOrigin.origin.transform.position;  
        
        StartCoroutine(SpawnWave(wave, origin));
      }
      
      while (pendingWaves > 0) {
        yield return new WaitForSeconds(1.0f);
      }

      while (EntitiesHandler.instance.enemies.Count > 0) {
        yield return null;
      }
      
      yield return new WaitForSeconds(3.0f);
    }

    GameController.instance?.Win();
  }

  private IEnumerator
  SpawnWave(EnemyWaveScriptableObject wave, Vector3 origin) {
    if (wave == null)
      yield break;

    pendingWaves++;

    foreach (EnemyBundleSettings bundleSettings in wave.bundles) {
      yield return new WaitForSeconds(bundleSettings.startDelay);
      
      if (bundleSettings.bundle != null)
        yield return StartCoroutine(SpawnBundle(bundleSettings.bundle, origin));
    }

    pendingWaves--;
  }
  
  private IEnumerator
  SpawnBundle(EnemyBundleScriptableObject bundle, Vector3 origin) {
    if (bundle == null)
      yield break;

    foreach (EnemySpawnSettings enemySetting in bundle.enemies) {
      yield return new WaitForSeconds(enemySetting.spawnDelay);
      
      if (enemySetting.enemyPrefab != null)
        Instantiate(enemySetting.enemyPrefab, origin, Quaternion.identity);
    }
  }
  #endregion
}
