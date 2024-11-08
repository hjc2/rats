using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// using SwitchController;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Vector3 respawnPoint;
    private Vector2 currentInput;
    private Vector2 movementDirection;
    public Transform targetPosition;
    private Vector3 boxTarget;
    public LayerMask UnwalkableLayer;
    public LayerMask MoveableLayer;
    private Vector3 resetPosition;
    private bool shift = false;
    private bool pulling = false;
    private bool pushing = false;
    private GameObject result;
    private bool moving = false;


    private void Awake() {
      targetPosition.position = transform.position;
      respawnPoint = transform.position;
    }

    void Start() {
      resetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
      if(Vector3.Distance(transform.position, targetPosition.position) < 0.01f &&
         !Physics2D.OverlapCircle(targetPosition.position + new Vector3(movementDirection.x, movementDirection.y, 0f), 0.1f, UnwalkableLayer)) {
          //if the player is not near an unwalkable layer
            if(Physics2D.OverlapCircle(targetPosition.position + new Vector3(movementDirection.x, movementDirection.y, 0f), 0.1f, MoveableLayer)) {
              //if there is a box in front of the player
              if(!Physics2D.OverlapCircle(targetPosition.position + new Vector3(2*movementDirection.x, 2*movementDirection.y, 0f), 0.1f, UnwalkableLayer) &&
                 !Physics2D.OverlapCircle(targetPosition.position + new Vector3(2*movementDirection.x, 2*movementDirection.y, 0f), 0.1f, MoveableLayer)) {//can't push box through wall
                  pushing = true;
                  result = findBox(transform.position, movementDirection); //CHANGED
                  targetPosition.position = new Vector3(targetPosition.position.x + movementDirection.x, targetPosition.position.y + movementDirection.y, 0f);
              }
            } else if(Physics2D.OverlapCircle(transform.position + new Vector3(-movementDirection.x, -movementDirection.y, 0f), 0.1f, MoveableLayer) && shift) {
              //if a box is in the opposite direction that the player is moving and shift is held
              pulling = true;
              targetPosition.position = new Vector3(targetPosition.position.x + movementDirection.x, targetPosition.position.y + movementDirection.y, 0f);
              result = findBox(transform.position, movementDirection);
              //result.transform.position = new Vector3(targetPosition.position.x - movementDirection.x, 0, 0f);
              //transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
            } else {
              pushing = false;
              targetPosition.position = new Vector3(targetPosition.position.x + movementDirection.x, targetPosition.position.y + movementDirection.y, 0f);
            }
         }


         if(pulling && moving) {
          boxTarget = new Vector3(targetPosition.position.x - movementDirection.x, targetPosition.position.y - movementDirection.y);
          //result.transform.position = Vector3.MoveTowards(result.transform.position, boxTarget, speed * Time.deltaTime);
         }
         else if(pushing && moving) {
          //result = findBox(transform.position);
          boxTarget = new Vector3(targetPosition.position.x + movementDirection.x, targetPosition.position.y + movementDirection.y);
          //result.transform.position = Vector3.MoveTowards(result.transform.position, boxTarget, speed * Time.deltaTime);
         }
         
         if(result != null){
          result.transform.position = Vector3.MoveTowards(result.transform.position, boxTarget, speed * Time.deltaTime);
         }
         transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
      //transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
      //transform.Translate(Vector3.Normalize(movementDirection) * speed * Time.deltaTime);
    }

    private GameObject findBox(Vector2 position, Vector2 direction) {
      Debug.Log(direction);
      Collider2D [] results = Physics2D.OverlapBoxAll(position, toSquare(direction), 0f);
      bool found = false;
      int i = 0;
      while(i < results.Length && !found) {
        if((results[i]).gameObject.CompareTag("Box")) {
          found = true;
          result = (results[i]).gameObject;
        }
        i++;
      }
      return result;
    }

private void activateTile()
{
    // Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);

    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3f);

    foreach (Collider2D collider in colliders)
    {
        if (collider.CompareTag("switch"))
        {
            Debug.Log("SWITCH!");
            SwitchController switchController = collider.GetComponent<SwitchController>();
            if (switchController != null)
            {
                switchController.ToggleState();
                Debug.Log($"Switch Clicked - State: {switchController.isActivated}");
            }
        }
    }
}


    //creates overlap square to make sure the correct box is pulled
    private Vector2 toSquare(Vector2 direction) {
      if(direction.x == 0) { return new Vector2(0.1f, Mathf.Abs(direction.y)); }
      else if(direction.y == 0) { return new Vector2(Mathf.Abs(direction.x), 0.1f); }
      else { return direction; }
    }

    void OnMove(InputValue value) {

      Debug.Log("moving!");
      if(value.Get() != null) { //start to move
        moving = true; //toggle moving
      }
      else {
        moving = false;
      }
      currentInput = value.Get<Vector2>().normalized;
      movementDirection = GetDirection(currentInput);
    }

    Vector2 GetDirection(Vector2 input)
    {
      Vector2 finalDirection = Vector2.zero;
            if (input.y > 0.01f)
            {
                //lastDirection = "Up";
                finalDirection = new Vector2(0, 1);
            }
            else if (input.y < -0.01f)
            {
                //lastDirection = "Down";
                finalDirection = new Vector2(0, -1);
            }
            else if (input.x > 0.01f)
            {
                //lastDirection = "Right";
                finalDirection = new Vector2(1, 0);
            }
            else if (input.x < -0.01f)
            {
                //lastDirection = "Left";
                finalDirection = new Vector2(-1, 0);
            }
            else
                finalDirection = Vector2.zero;

            return finalDirection;
    }

    void OnPull(InputValue value) {
      if(value.Get() != null) { shift = true; } //pressed
      else {
        shift = false;
        pulling = false;
      }
    }

    void OnToggle(InputValue value) {
      if(value.isPressed){
        Debug.Log("Pressed");
        activateTile();
      }
    }

    public void ResetPlayerPosition()
    {
      transform.position = respawnPoint;
      targetPosition.position = respawnPoint;
    }

    // void OnTriggerEnter2D(Collider2D other) {
    //   ResetPlayerPosition();
    // }
}