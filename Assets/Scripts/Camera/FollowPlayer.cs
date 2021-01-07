using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public static FollowPlayer instance;

    private Transform player;

    private bool isAbleToClamp = true;

    private float cameraDelay;
    private float playerClampX;
    private float playerClampY;

    private Vector3 lastPlayerPositionReached;

    Vector3 leftPoint;
    Vector3 rightPoint;
    Vector3 bottomPoint;
    Vector3 topPoint;

    private Camera cam;

    void Start() {
        cam = Camera.main;

        cameraDelay = Time.deltaTime * 2;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        instance = this;
    }

    void Update() {
        HandleEnteredBounds();
        FollowPlayerPos();
    }

    void HandleEnteredBounds() {
        foreach (BoundaryManager boundaryManager in FindObjectsOfType<BoundaryManager>()) {
            if (boundaryManager.enteredBounds) {
                FollowPlayerUntilPassBounds(boundaryManager);
            }
        }
    }

    private Vector3 boundsMax;
    private Vector3 boundsMin;

    void FollowPlayerUntilPassBounds(BoundaryManager currentBounds) {
        // Freely following player
        if (!isAbleToClamp) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, player.position.y, transform.position.z), cameraDelay);
        }
        
        // Until the player enters the area, continue following
        if (!currentBounds.alreadyClampedThisBounds) {
            isAbleToClamp = false;
        }

        // Each camera limit point
        Vector3 leftPoint = cam.ScreenToWorldPoint(new Vector3(0f, cam.pixelHeight / 2));
        Vector3 rightPoint = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight / 2));
        Vector3 bottomPoint = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, 0f));
        Vector3 topPoint = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, cam.pixelHeight));

        // Debug
        boundsMax = currentBounds.managerBox.bounds.max;
        boundsMin = currentBounds.managerBox.bounds.min;

        // Checking if the player is within the limit area
        if (
            currentBounds.managerBox.bounds.min.x < leftPoint.x 
            && currentBounds.managerBox.bounds.max.x > rightPoint.x
            && currentBounds.managerBox.bounds.min.y < topPoint.y
            && currentBounds.managerBox.bounds.max.y > bottomPoint.y
            && !currentBounds.alreadyClampedThisBounds
        ) {
            currentBounds.alreadyClampedThisBounds = true;
            isAbleToClamp = true;
        }
    }

    void FollowPlayerPos() {
        GameObject boundary = GameObject.Find("Boundary");

        if (boundary != null) {
            playerClampX = Mathf.Clamp(player.position.x, boundary.GetComponent<BoxCollider2D>().bounds.min.x + Vector3.Distance(transform.position, leftPoint), boundary.GetComponent<BoxCollider2D>().bounds.max.x - Vector3.Distance(transform.position, rightPoint));
            playerClampY = Mathf.Clamp(player.position.y, boundary.GetComponent<BoxCollider2D>().bounds.min.y + Vector3.Distance(transform.position, bottomPoint), boundary.GetComponent<BoxCollider2D>().bounds.max.y - Vector3.Distance(transform.position, topPoint));
        }

        // Following the player with the Clamp at the current limit
        if (GameObject.Find("Boundary") && isAbleToClamp) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(playerClampX, playerClampY, transform.position.z), cameraDelay);
        }
    }

    void OnDrawGizmos() {
        // Camera limits
        Gizmos.color = Color.green;

        leftPoint = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight / 2));
        rightPoint = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight / 2));
        bottomPoint = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, 0f));
        topPoint = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight));

        Gizmos.DrawWireSphere(rightPoint, 0.5f);
        Gizmos.DrawWireSphere(leftPoint, 0.5f);
        Gizmos.DrawWireSphere(topPoint, 0.5f);
        Gizmos.DrawWireSphere(bottomPoint, 0.5f);

        // BoundaryManager limits
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(boundsMax, 0.5f);
        Gizmos.DrawWireSphere(boundsMin, 0.5f);
        Gizmos.DrawWireSphere(new Vector3(boundsMin.x, boundsMax.y, 0f), 0.5f);
        Gizmos.DrawWireSphere(new Vector3(boundsMax.x, boundsMin.y, 0f), 0.5f);
    }

}
