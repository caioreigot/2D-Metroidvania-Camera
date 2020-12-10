using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    private BoxCollider2D cameraBox; // The BoxCollider of camera
    private Transform player; // Position of player
    private float currentResolutionRatio; // Will track the current screen resolution ratio

    void Start() {
        cameraBox = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        currentResolutionRatio = Camera.main.aspect;

        ChangeAspectRatioBox();
    }

    void Update() {
        if (currentResolutionRatio != Camera.main.aspect)
            ChangeAspectRatioBox();
        
        FollowPlayerPos();
    }

    void ChangeAspectRatioBox() {
        currentResolutionRatio = Camera.main.aspect;

        // 16:10 ratio
        if (Camera.main.aspect >= 1.59f && Camera.main.aspect < 1.7f) {
            cameraBox.size = new Vector2(16f, 10f);
        }
        // 16:9 ratio
        if (Camera.main.aspect >= 1.7f && Camera.main.aspect < 1.8f) {
            cameraBox.size = new Vector2(17.78f, 10f);
        }
        // 5:4 ratio
        if (Camera.main.aspect >= 1.2f && Camera.main.aspect < 1.3f) {
            cameraBox.size = new Vector2(12.49f, 10f);
        }
        // 4:3 ratio
        if (Camera.main.aspect >= 1.3f && Camera.main.aspect < 1.4f) {
            cameraBox.size = new Vector2(13.34f, 10f);
        }
        // 3:2 ratio
        if (Camera.main.aspect >= 1.5f && Camera.main.aspect < 1.59f) {
            cameraBox.size = new Vector2(14.97f, 10f);
        }
    }

    void FollowPlayerPos() {
        if (GameObject.Find("Boundary")) {
            transform.position = new Vector3(Mathf.Clamp(
                // X
                player.position.x, GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.min.x + cameraBox.size.x / 2, 
                GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.max.x - cameraBox.size.x / 2),
                
                // Y
                Mathf.Clamp(player.position.y, GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.min.y + cameraBox.size.y / 2, 
                GameObject.Find("Boundary").GetComponent<BoxCollider2D>().bounds.max.y - cameraBox.size.y / 2),
                
                // Z
                transform.position.z
                );
        }
    }

}
