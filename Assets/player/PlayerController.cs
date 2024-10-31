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
              //if the target
              if(!Physics2D.OverlapCircle(targetPosition.position + new Vector3(2*movement.x, 2*movement.y, 0f), 0.1f, UnwalkableLayer) &&
                 !Physics2D.OverlapCircle(targetPosition.position + new Vector3(2*movement.x, 2*movement.y, 0f), 0.1f, MoveableLayer)) {
                targetPosition.position = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y, 0f);
              }
            } else {
              targetPosition.position = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y, 0f);
            }
         }
      transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
      //transform.Translate(Vector3.Normalize(movement) * speed * Time.deltaTime);
    }

    void OnMove(InputValue value) {
      movement = value.Get<Vector2>();
    }

    

    private void OnCollisionEnter2D(Collision2D other) {
      if(other.gameObject.CompareTag("Box")) {
        other.gameObject.transform.position = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y, 0f);
      }
      if(other.gameObject.CompareTag("light")) {
        transform.position = resetPosition;
        targetPosition.position = transform.position;
      }
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if(other.CompareTag("Box")) {
        other.gameObject.transform.position = new Vector3(targetPosition.position.x + movement.x, targetPosition.position.y + movement.y, 0f);
      }
      if(other.CompareTag("light")) {
        transform.position = resetPosition;
        targetPosition.position = transform.position;
      }
    }
}