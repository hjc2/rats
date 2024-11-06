using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// using SwitchController;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Vector2 movement;
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
    }

    void Start() {
      resetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
      if(Vector3.Distance(transform.position, targetPosition.position) < 0.01f &&
         !Physics2D.OverlapCircle(targetPosition.position + new Vector3(movement.x, movement.y, 0f), 0.1f, UnwalkableLayer)) {
          //if the player is not near an unwalkable layer
            if(Physics2D.OverlapCircle(targetPosition.position + new Vector3(movement.x, movement.y, 0f), 0.1f, MoveableLayer)) {
              //if there is a box in front of the player
              if(!Physics2D.OverlapCircle(targetPosition.position + new Vector3(2*movement.x, 2*movement.y, 0f), 0.1f, UnwalkableLayer) &&
                 !Physics2D.OverlapCircle(targetPosition.position + new Vector3(2*movement.x, 2*movement.y, 0f), 0.1f, MoveableLayer)) {//can't push box through wall
                  pushing = true;
                  result = findBox(transform.position, movement); //CHANGED
                  targetPosition.position = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y, 0f);
              }
            } else if(Physics2D.OverlapCircle(transform.position + new Vector3(-movement.x, -movement.y, 0f), 0.1f, MoveableLayer) && shift) {
              //if a box is in the opposite direction that the player is moving and shift is held
              pulling = true;
              targetPosition.position = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y, 0f);
              result = findBox(transform.position, movement);
              //result.transform.position = new Vector3(targetPosition.position.x - movement.x, 0, 0f);
              //transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
            } else {
              pushing = false;
              targetPosition.position = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y, 0f);
            }
         }


         if(pulling && moving) {
          boxTarget = new Vector3(targetPosition.position.x - movement.x, targetPosition.position.y - movement.y);
          //result.transform.position = Vector3.MoveTowards(result.transform.position, boxTarget, speed * Time.deltaTime);
         }
         else if(pushing && moving) {
          //result = findBox(transform.position);
          boxTarget = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y);
          //result.transform.position = Vector3.MoveTowards(result.transform.position, boxTarget, speed * Time.deltaTime);
         }
         
         if(result != null){
          result.transform.position = Vector3.MoveTowards(result.transform.position, boxTarget, speed * Time.deltaTime);
         }
         transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
      //transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
      //transform.Translate(Vector3.Normalize(movement) * speed * Time.deltaTime);
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
    Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);
    foreach (Collider2D collider in colliders)
    {
        if (collider.CompareTag("switch"))
        {
            SwitchController switchController = collider.GetComponent<SwitchController>();
            if (switchController != null)
            {
                switchController.ToggleState();
                Debug.Log($"Switch Clicked - State: {switchController.isActivated}");
            }
        }
    }
}

    private void OnTriggerEnter2D(Collider2D other)
    {   
        switch (other.gameObject.tag)
        {
            case "light":
                Debug.Log("Player Spotted");
                transform.position = resetPosition;
                currentCell = wallTilemap.WorldToCell(transform.position);
                targetCell = currentCell;
                startPosition = transform.position;
                endPosition = wallTilemap.GetCellCenterWorld(targetCell);
                break;
        }
    }

    //creates overlap square to make sure the correct box is pulled
    private Vector2 toSquare(Vector2 direction) {
      if(direction.x == 0) { return new Vector2(0.1f, Mathf.Abs(direction.y)); }
      else if(direction.y == 0) { return new Vector2(Mathf.Abs(direction.x), 0.1f); }
      else { return direction; }
    }

    void OnMove(InputValue value) {
      if(value.Get() != null) { //start to move
        moving = true; //toggle moving
      }
      else {
        moving = false;
      }
      movement = value.Get<Vector2>();
    }

    void OnPull(InputValue value) {
      if(value.Get() != null) { shift = true; } //pressed
      else {
        shift = false;
        pulling = false;
      }
    }
}