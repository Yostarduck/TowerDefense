using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CannonBallProjectile : MonoBehaviour
{
#region PROPERTIES

  [Header("Base Projectile Properties")]
  
  [SerializeField] protected float speed = 10.0f;
  [SerializeField] private float lifetime = 15.0f;
  
  [HideInInspector]
  public BaseEntity owner = null;
  [HideInInspector]
  public BaseEntity target = null;

  protected Vector3 sourcePosition;
  protected Vector3 targetPosition;

#endregion

#region UNITY_METHODS

  /// <summary>
  /// Start is called before the first frame update.
  /// </summary>
  protected void
  Start() {
    sourcePosition = transform.position;

    BaseCharacter character = target as BaseCharacter;
    
    if (character != null) {
      CalculateTargetPosition(character);
    }
    else {
      targetPosition = target.transform.position;
    }
    
    transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);

    StartCoroutine(DestroyAfterLifetime());
  }

  /// <summary>
  /// Update is called once per frame.
  /// </summary>
  protected void
  Update() {
    transform.Translate(Vector3.forward * speed * Time.deltaTime);
  }
  
  /// <summary>
  /// OnTriggerEnter is called when the Collider other enters the trigger.
  /// </summary>
  /// <param name="other">The other Collider involved in this collision.</param>
  protected void
  OnTriggerEnter(Collider other) {
    BaseEntity entity = other.attachedRigidbody?.GetComponent<BaseEntity>();

    if (entity == null)
      return;

    if (entity == owner)
      return;
      
    entity.Damage(owner.attackDamage);

    Destroy(gameObject);
  }

#endregion

#region METHODS

  /// <summary>
  /// Calculate the target position based on the character path.
  /// </summary>
  /// <param name="character">The character to predict its position</param>
  private void
  CalculateTargetPosition(BaseCharacter character) {
    if (character == null)
      return;
    
    targetPosition = character.transform.position;

    List<Vector2Int> path = character.GetPath();

    if (path == null)
      return;

    if (path.Count == 0)
      return;

    if (!character.isMoving)
      return;

    float characterSpeed = character.movementSpeed;
    float accumTime = 0.0f;

    Vector3 firstPoint;
    Vector3 secondPoint = character.transform.position;
    
    for (int i = 0; i < path.Count; i++) {
      firstPoint = secondPoint;
      secondPoint = new(path[i].x, character.transform.position.y, path[i].y);

      float bulletTimeToReachFirstPoint = CalculateTimeToReachPoint(sourcePosition, firstPoint, speed);
      float bulletTimeToReachSecondPoint = CalculateTimeToReachPoint(sourcePosition, secondPoint, speed);
      
      float characterTimeToMovefromPoints = CalculateTimeToReachPoint(firstPoint, secondPoint, characterSpeed);

      float characterTimeToReachFirstPoint = accumTime;
      float characterTimeToReachSecondPoint = characterTimeToReachFirstPoint + characterTimeToMovefromPoints;

      if (bulletTimeToReachSecondPoint <= characterTimeToReachSecondPoint) {
        float bulleFirstTime = bulletTimeToReachFirstPoint;
        float bulletSecondTime = bulletTimeToReachSecondPoint;

        float characterFirstTime = characterTimeToReachFirstPoint;
        float characterSecondTime = characterTimeToReachSecondPoint;
        
        float bulletDeltaTime = bulletSecondTime - bulleFirstTime;

        float characterDeltaTime = characterSecondTime - characterFirstTime;
        
        float contactTime = (characterFirstTime - bulleFirstTime) / (bulletDeltaTime - characterDeltaTime);

        Vector3 contactPoint = Vector3.Lerp(firstPoint, secondPoint, contactTime);

        targetPosition = contactPoint;
        targetPosition.y = character.transform.position.y + character.headOffset;

        return;
      }

      accumTime = characterTimeToReachSecondPoint;
    }
    
    // If we reach this point, the bullet won't reach the target before the character reaches the end of the path.

    targetPosition = new(path[path.Count - 1].x,
                         character.transform.position.y + character.headOffset,
                         path[path.Count - 1].y);
  }

  /// <summary>
  /// Utility method to calculate the time to reach a point.
  /// </summary>
  /// <param name="sourcePoint">Origin point</param>
  /// <param name="targetPoint">Target point</param>
  /// <param name="speed">Speed to move from one point to another</param>
  /// <returns>Time required to travel from one point to another</returns>
  private float
  CalculateTimeToReachPoint(Vector3 sourcePoint, Vector3 targetPoint, float speed) {
    float distance = Vector3.Distance(sourcePoint, targetPoint);
    return distance / speed;
  }

  /// <summary>
  /// Destroy the projectile after its lifetime.
  /// </summary>
  private IEnumerator
  DestroyAfterLifetime() {
    yield return new WaitForSeconds(lifetime);
    Destroy(gameObject);
  }
  
#endregion
}