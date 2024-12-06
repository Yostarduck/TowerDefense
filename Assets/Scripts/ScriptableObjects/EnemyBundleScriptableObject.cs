using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct EnemySpawnSettings
{
  public GameObject enemyPrefab;
  public float spawnDelay;
}

[CreateAssetMenu(fileName = "EnemyBundleScriptableObject", menuName = "TowerDefense/Enemies/EnemyBundle")]
public class EnemyBundleScriptableObject : ScriptableObject
{
  [SerializeField]
  private List<EnemySpawnSettings> m_enemies = new();
  public List<EnemySpawnSettings> enemies { get => m_enemies; }
  
  /// <summary>
  /// Get the total number of enemies in the bundle.
  /// </summary>
  /// <returns>The total number of enemies in the bundle.</returns>
  public int
  GetEnemyCount() => enemies.Count(x => x.enemyPrefab != null);
  
  /// <summary>
  /// Get the total spawn time of the bundle.
  /// </summary>
  /// <returns>The total spawn time of the bundle.</returns>
  public float
  GetTotalSpawnTime() {
    float totalSpawnTime = 0.0f;

    foreach (EnemySpawnSettings enemy in enemies) {
      totalSpawnTime += enemy.spawnDelay;
    }

    return totalSpawnTime;
  }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(EnemySpawnSettings))]
public class EnemySpawnSettingsDrawer : PropertyDrawer
{
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);
    
    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
    
    var indent = EditorGUI.indentLevel;
    EditorGUI.indentLevel = 0;
    
    float offset = 0.0f;
    float size;
    
    size = 80.0f;
    var delayLabelRect = new Rect(position.x, position.y, size, position.height);

    offset += size;
    size = 40.0f;
    var delayRect = new Rect(position.x + offset, position.y, size, position.height);
    
    offset += size + 20.0f;
    size = 85.0f;
    var enemyLabelRect = new Rect(position.x + offset, position.y, size, position.height);

    offset += size;
    size = (position.width - offset) - 5.0f;
    var enemyRect = new Rect(position.x + offset, position.y, size, position.height);
    
    EditorGUI.LabelField(delayLabelRect, "Spawn Delay");
    EditorGUI.PropertyField(delayRect, property.FindPropertyRelative("spawnDelay"), GUIContent.none);

    EditorGUI.LabelField(enemyLabelRect, "Enemy prefab");
    EditorGUI.PropertyField(enemyRect, property.FindPropertyRelative("enemyPrefab"), GUIContent.none);

    EditorGUI.indentLevel = indent;

    EditorGUI.EndProperty();
  }
}

#endif