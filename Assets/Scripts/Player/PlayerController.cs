using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rb;

    public float speed;
    private Vector2 movement;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        movement.x = Input.GetAxis("Horizontal"); // -1, 0, 1
        movement.y = Input.GetAxis("Vertical");

        #region Own implementation test
        /*
        if (Input.GetKey(KeyCode.D))
            movement.x = Mathf.Lerp(movement.x, 1f, Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
            movement.x = Mathf.Lerp(movement.x, -1f, Time.deltaTime);
        
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
            movement.x = 0f;

        Debug.Log("X: " + movement.x + " Y: " + movement.y);
        */
        #endregion

        rb.velocity = new Vector2(movement.x, movement.y) * speed;
    }

}
