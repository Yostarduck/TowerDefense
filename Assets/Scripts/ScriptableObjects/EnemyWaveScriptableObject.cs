using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EnemyBundleSettings
{
  public EnemyBundleScriptableObject bundle;
  public float startDelay;
}

[CreateAssetMenu(fileName = "EnemyWaveScriptableObject", menuName = "TowerDefense/Enemies/EnemyWave")]
public class EnemyWaveScriptableObject : ScriptableObject
{
  public List<EnemyBundleSettings> bundles = new();
  
  /// <summary>
  /// Get the total number of bundles in the wave.
  /// </summary>
  /// <returns>The total number of bundles in the wave.</returns>
  public int
  GetBundleCount() => bundles.Count;
  
  /// <summary>
  /// Get the total number of enemies in the wave.
  /// </summary>
  /// <returns>The total number of enemies in the wave.</returns>
  public int
  GetEnemyCount() {
    int count = 0;

    foreach (EnemyBundleSettings bundleSetting in bundles) {
      if (bundleSetting.bundle != null)
        count += bundleSetting.bundle.GetEnemyCount();
    }
    
    return count;
  }
}