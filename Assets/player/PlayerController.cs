using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Vector2 movement;
    public Transform targetPosition;
    public LayerMask UnwalkableLayer;
    public LayerMask MoveableLayer;
    private Vector3 resetPosition;
    private bool shift = false;
    private bool pulling = false;
    private GameObject result;

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
                 !Physics2D.OverlapCircle(targetPosition.position + new Vector3(2*movement.x, 2*movement.y, 0f), 0.1f, MoveableLayer)) {
                targetPosition.position = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y, 0f);
              }
            } else if(Physics2D.OverlapCircle(transform.position + new Vector3(-movement.x, -movement.y, 0f), 0.1f, MoveableLayer) && shift) {
              //if a box is in the opposite direction that the player is moving and shift is held
              Debug.Log("pull?");
              pulling = true;
              targetPosition.position = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y, 0f);
              Collider2D [] results = Physics2D.OverlapCircleAll(transform.position, 1f);
              foreach(Collider2D coll in results){
                if(coll.gameObject.CompareTag("Box")) {
                  result = coll.gameObject;
                }
              }
              //result.transform.position = new Vector3(targetPosition.position.x - movement.x, 0, 0f);
              Debug.Log(result);
              //transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
            } else {
              targetPosition.position = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y, 0f);
            }
         }
         if(pulling) {
          result.transform.position = Vector3.MoveTowards(result.transform.position, new Vector3(targetPosition.position.x - movement.x, targetPosition.position.y - movement.y), speed * Time.deltaTime);
         }
         transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
      //transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
      //transform.Translate(Vector3.Normalize(movement) * speed * Time.deltaTime);
    }

    void OnMove(InputValue value) {
      movement = value.Get<Vector2>();
    }

    void OnPull(InputValue value) {
      if(value.Get() != null) { //pressed
        shift = true;
        Debug.Log("Down");
      }
      else {
        shift = false;
        pulling = false;
        Debug.Log("Up");
      }
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if(other.CompareTag("Box")) {
        Debug.Log("trigger");
        other.gameObject.transform.position = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y, 0f);
      }
      if(other.CompareTag("light")) {
        transform.position = resetPosition;
        targetPosition.position = transform.position;
      }
    }
}