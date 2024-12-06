using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
#region SINGLETON

  public static GameController instance {
    get => m_instance;
    private set => m_instance = value;
  }

  private static GameController m_instance = null;

  public static bool isInitialized => instance != null;

#endregion

#region PROPERTIES
  [SerializeField]
  PlayerTower PlayerTower;

  [SerializeField]
  GameObject PreviewCube;
  
  [SerializeField]
  Material PreviewCubeMaterial;

  [SerializeField]
  GameObject CannonTowerPrefab;

  [SerializeField]
  GameObject WizardTowerPrefab;

  [SerializeField]
  GameObject LaserTowerPrefab;

  GameObject SelectedTowerPrefab;

  [SerializeField]
  TMPro.TextMeshProUGUI LifeText;

  [SerializeField]
  TMPro.TextMeshProUGUI WaveText;

  [SerializeField]
  TMPro.TextMeshProUGUI TowersText;

  [SerializeField]
  GameObject EditContainer;
  
  [SerializeField]
  GameObject TitleScreen;

  [SerializeField]
  GameObject GameScreen;
  
  [SerializeField]
  GameObject LoseScreen;
  
  [SerializeField]
  GameObject VictoryScreen;

  [SerializeField]
  int towersPerRound = 5;

  int towerPoints = 0;

  private bool editMode = false;

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
      Debug.LogWarning("Multiple instances of GameController found", gameObject);
      Destroy(gameObject);
    }
  }

  /// <summary>
  /// Start is called before the first frame update.
  /// </summary>
  void
  Start() {
    ShowTitleScreen();

    if (LifeText == null) {
      Debug.LogWarning("Life text not set", gameObject);
      return;
    }
    
    if (PlayerTower == null) {
      Debug.LogWarning("Player tower not set", gameObject);
      return;
    }
    
    PlayerTower.OnDamageEvent += UpdateLifeText;
    PlayerTower.OnHealEvent += UpdateLifeText;
    PlayerTower.OnDeathEvent += Lose;

    if (PreviewCube != null) {
      PreviewCube.SetActive(false);

      MeshRenderer meshRenderer = PreviewCube.GetComponent<MeshRenderer>();
      if (meshRenderer == null) {
        meshRenderer = PreviewCube.GetComponentInChildren<MeshRenderer>(true);
        if (meshRenderer == null) {
          Debug.LogWarning("MeshRenderer not found", gameObject);
          return;
        }
      }
      
      PreviewCubeMaterial = meshRenderer.material;
    }
  }

  /// <summary>
  /// Update is called once per frame.
  /// This is where the tower building logic is.
  /// </summary>
  void
  Update() {
    if (Keyboard.current.escapeKey.wasPressedThisFrame) {
      editMode = false;
      SelectedTowerPrefab = null;
      PreviewCube.SetActive(false);
    }

    if (!editMode)
      return;

    Mouse mouse = Mouse.current;

    Vector2 mousePosition = mouse.position.ReadValue();

    Plane plane = new Plane(Vector3.up, Vector3.zero);

    Ray ray = Camera.main.ScreenPointToRay(mousePosition);

    Vector2Int position = Vector2Int.zero;

    if (plane.Raycast(ray, out float distance)) {
      Vector3 point = ray.GetPoint(distance);

      position = new(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.z));

      PreviewCube.transform.position = new Vector3(position.x, 0, position.y);
    }

    {
      if (PreviewCubeMaterial != null) {
        PreviewCubeMaterial.color = Color.green;
      }

      if (towerPoints <= 0) {
        if (PreviewCubeMaterial != null) {
          PreviewCubeMaterial.color = Color.red;
        }
      }

      if (EntitiesHandler.isInitialized) {
        HashSet<BaseTower> towers = EntitiesHandler.instance.towers;

        BaseTower tower = towers.FirstOrDefault(x => Vector2Int.Distance(new Vector2Int(Mathf.RoundToInt(x.transform.position.x),
                                                                                        Mathf.RoundToInt(x.transform.position.z)),
                                                                        position) <= 0.1f);

        if (tower != null) {
          if (PreviewCubeMaterial != null) {
            PreviewCubeMaterial.color = Color.red;
          }
        }
      }
      
      if (Pathfinding.isInitialized) {
        if (Pathfinding.instance.GetNode(position) != null) {
          if (PreviewCubeMaterial != null) {
            PreviewCubeMaterial.color = Color.red;
          }
        }
      }
    }

    if (towerPoints > 0 && mouse.leftButton.wasPressedThisFrame && !IsPointerOverUIObject()) {
      BuildTower(position);
    }

    if (mouse.rightButton.wasPressedThisFrame && !IsPointerOverUIObject()) {
      DestroyTower(position);
    }
  }

#endregion

#region BUILD_METHODS

  /// <summary>
  /// Builds a tower at the given position.
  /// If the tower prefab is not set, it does nothing.
  /// If there is already a tower at the given position, it does nothing.
  /// If the pathfinding is not initialized, it does nothing.
  /// </summary>
  /// <param name="position">The position to build the tower at.</param>
  private void
  BuildTower(Vector2Int position) {
    if (SelectedTowerPrefab == null) {
      Debug.LogWarning("Tower prefab not set", gameObject);
      return;
    }

    HashSet<BaseTower> towers = EntitiesHandler.instance.towers;
    
    BaseTower tower = towers.FirstOrDefault(x => Vector2Int.Distance(new Vector2Int(Mathf.RoundToInt(x.transform.position.x),
                                                                                    Mathf.RoundToInt(x.transform.position.z)),
                                                                     position) <= 0.1f);

    if (tower != null) {
      return;
    }

    if (!Pathfinding.isInitialized) {
      Debug.LogWarning("Pathfinding not initialized", gameObject);
      return;
    }

    if (Pathfinding.instance.GetNode(position) != null) {
      return;
    }

    Instantiate(SelectedTowerPrefab, new(position.x, 0.0f, position.y), Quaternion.identity);

    towerPoints--;
    UpdateTowersText();
  }
  
  /// <summary>
  /// Destroys a tower at the given position.
  /// If the tower is a player tower, it does nothing.
  /// If the tower is not found, it does nothing.
  /// </summary>
  /// <param name="position">The position of the tower to destroy.</param>
  private void
  DestroyTower(Vector2Int position) {
    if (!EntitiesHandler.isInitialized) {
      Debug.LogWarning("EntitiesHandler not initialized", gameObject);
      return;
    }

    HashSet<BaseTower> towers = EntitiesHandler.instance.towers;
    
    BaseTower tower = towers.FirstOrDefault(x => Vector2Int.Distance(new Vector2Int(Mathf.RoundToInt(x.transform.position.x),
                                                                                    Mathf.RoundToInt(x.transform.position.z)),
                                                                     position) <= 0.1f);

    if (tower == null)
      return;

    if (tower is PlayerTower)
      return;

    tower.Die();

    towerPoints++;
    UpdateTowersText();
  }

  /// <summary>
  /// Selects the a tower and enables build mode.
  /// </summary>
  /// <param name="tower">The tower to select</param>
  private void
  SelectTower(GameObject tower) {
    if (tower == null) {
      Debug.LogWarning("Tower prefab not set", gameObject);
      return;
    }

    SelectedTowerPrefab = tower;
    editMode = true;
    PreviewCube.SetActive(true);
  }
  
  /// <summary>
  /// Selects the cannon tower.
  /// </summary>
  public void
  SelectCannonTower() {
    SelectTower(CannonTowerPrefab);
  }
  
  /// <summary>
  /// Selects the wizard tower.
  /// </summary>
  public void
  SelectWizardTower() {
    SelectTower(WizardTowerPrefab);
  }
  
  /// <summary>
  /// Selects the laser tower.
  /// </summary>
  public void
  SelectLaserTower() {
    SelectTower(LaserTowerPrefab);
  }
#endregion

#region METHODS

  /// <summary>
  /// Tells the GameController that the next wave is ready to start.
  /// </summary>
  public void
  EnableNextWave() {
    if (!WavesController.isInitialized) {
      Debug.LogWarning("WavesController not initialized", gameObject);
      return;
    }

    if (EditContainer == null) {
      Debug.LogWarning("EditContainer not set", gameObject);
      return;
    }
    
    EditContainer.SetActive(true);

    towerPoints += towersPerRound;

    UpdateWaveText();
    UpdateTowersText();
  }

  /// <summary>
  /// Start the next wave.
  /// </summary>
  public void
  StartNextWave() {
    if (!WavesController.isInitialized) {
      Debug.LogWarning("WavesController not initialized", gameObject);
      return;
    }

    editMode = false;
    SelectedTowerPrefab = null;
    PreviewCube.SetActive(false);

    WavesController.instance.StartNextWave();
  }

  /// <summary>
  /// Shows the win screen.
  /// </summary>
  public void
  Win() {
    ShowVictoryScreen();
  }

  /// <summary>
  /// Shows the lose screen.
  /// </summary>
  public void
  Lose() {
    if (LifeText == null) {
      Debug.LogWarning("Life text not set", gameObject);
      return;
    }

    LifeText.text = "Dead";

    ShowLoseScreen();
  }
  
  /// <summary>
  /// Restarts the game.
  /// </summary>
  public void
  RestartGame() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  /// <summary>
  /// Cast a ray to test if Input.mousePosition is over any UI object in EventSystem.current. This is a replacement
  /// for IsPointerOverGameObject() which does not work on Android in 4.6.0f3
  /// </summary>
  private bool
  IsPointerOverUIObject() {
      // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
      // the ray cast appears to require only eventData.position.
      PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
      eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
  
      List<RaycastResult> results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
      return results.Count > 0;
  }

#endregion

#region TEXT_METHODS

  /// <summary>
  /// Updates the life text.
  /// </summary>
  private void
  UpdateLifeText() {
    if (LifeText == null) {
      Debug.LogWarning("Life text not set", gameObject);
      return;
    }

    if (PlayerTower == null) {
      Debug.LogWarning("Player tower not set", gameObject);
      return;
    }

    LifeText.text = "Life: " + PlayerTower.health.ToString() + "/" + PlayerTower.maxHealth.ToString();
  }

  /// <summary>
  /// Updates the wave text.
  /// </summary>
  private void
  UpdateWaveText() {
    if (WaveText == null) {
      Debug.LogWarning("Wave text not set", gameObject);
      return;
    }

    if (!WavesController.isInitialized) {
      Debug.LogWarning("WavesController not initialized", gameObject);
      return;
    }

    WaveText.text = "Wave: " + (WavesController.instance.CurrentWave + 1).ToString() + "/" + WavesController.instance.GetMaxWaves();
  }
  
  private void
  UpdateTowersText() {
    if (TowersText == null) {
      Debug.LogWarning("Towers text not set", gameObject);
      return;
    }

    TowersText.text = "Towers Available: " + towerPoints.ToString();
  }
#endregion

#region SCREENS_METHODS

  /// <summary>
  /// Hides all screens.
  /// </summary>
  private void
  HideAllScreens() {
    if (TitleScreen == null) {
      Debug.LogWarning("Title screen not set", gameObject);
      return;
    }

    if (GameScreen == null) {
      Debug.LogWarning("Game screen not set", gameObject);
      return;
    }

    if (LoseScreen == null) {
      Debug.LogWarning("Lose screen not set", gameObject);
      return;
    }

    if (VictoryScreen == null) {
      Debug.LogWarning("Victory screen not set", gameObject);
      return;
    }

    TitleScreen.SetActive(false);
    GameScreen.SetActive(false);
    LoseScreen.SetActive(false);
    VictoryScreen.SetActive(false);
  }
  
  /// <summary>
  /// Shows the title screen.
  /// Hides all other screens.
  /// </summary>
  public void
  ShowTitleScreen() {
    HideAllScreens();
    
    if (TitleScreen != null)
      TitleScreen.SetActive(true);
  }
  
  /// <summary>
  /// Shows the game screen.
  /// Hides all other screens.
  /// </summary>
  public void
  ShowGameScreen() {
    HideAllScreens();
    
    if (GameScreen != null)
      GameScreen.SetActive(true);

    UpdateLifeText();
    UpdateWaveText();
  }

  /// <summary>
  /// Shows the lose screen.
  /// Hides all other screens.
  /// </summary>
  public void
  ShowLoseScreen() {
    HideAllScreens();

    if (LoseScreen != null)
      LoseScreen.SetActive(true);
  }

  /// <summary>
  /// Shows the victory screen.
  /// Hides all other screens.
  /// </summary>
  public void
  ShowVictoryScreen() {
    HideAllScreens();

    if (VictoryScreen != null)
      VictoryScreen.SetActive(true);
  }

#endregion
}