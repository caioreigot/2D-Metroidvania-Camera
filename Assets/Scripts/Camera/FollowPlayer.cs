using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public static FollowPlayer instance;

    private BoxCollider2D cameraBox; // The BoxCollider of camera
    private Transform player; // Position of player
    private float currentResolutionRatio; // Will track the current screen resolution ratio

    private bool isAbleToClamp = true;

    private float cameraDelay;
    private float playerClampX;
    private float playerClampY;

    private Vector3 lastPlayerPositionReached;

    void Start() {
        cameraBox = GetComponent<BoxCollider2D>();
        cameraDelay = Time.deltaTime * 2;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        currentResolutionRatio = Camera.main.aspect;

        instance = this;

        ChangeAspectRatioBox();
    }

    void Update() {
        if (currentResolutionRatio != Camera.main.aspect)
            ChangeAspectRatioBox();
        
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

    // Pra debugar no Gizmos a area do limite atual
    // To debug the current limit area in Gizmos
    private Vector3 boundsMax;
    private Vector3 boundsMin;

    void FollowPlayerUntilPassBounds(BoundaryManager currentBounds) {
        // Seguindo o player livremente
        // Following the player freely
        if (!isAbleToClamp) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, player.position.y, transform.position.z), cameraDelay);
        }

        // Enquanto o player nao entrar na area, continuar deixando a camera "livre"
        // Until the player enters the area, keep leaving the camera "free"
        if (!currentBounds.alreadyClampedThisBounds) {
            isAbleToClamp = false;
        }

        // Cada ponto limite da camera
        // Each limit point of the camera
        Vector3 rightSide = new Vector3(player.transform.position.x + cameraBox.size.x / 2, player.transform.position.y, 0f);
        Vector3 leftSide = new Vector3(player.transform.position.x - cameraBox.size.x / 2, player.transform.position.y, 0f);
        Vector3 topSide = new Vector3(player.transform.position.x, player.transform.position.y + cameraBox.size.y / 2, 0f);
        Vector3 bottomSide = new Vector3(player.transform.position.x, player.transform.position.y - cameraBox.size.y / 2, 0f);

        // Debug
        boundsMax = currentBounds.managerBox.bounds.max;
        boundsMin = currentBounds.managerBox.bounds.min;

        // Verificando se o player esta dentro da area limite
        // Checking if the player is within the limit area
        if (
            currentBounds.managerBox.bounds.min.x < leftSide.x 
            && currentBounds.managerBox.bounds.max.x > rightSide.x
            && currentBounds.managerBox.bounds.min.y < topSide.y
            && currentBounds.managerBox.bounds.max.y > bottomSide.y
            && !currentBounds.alreadyClampedThisBounds
        ) {
            currentBounds.alreadyClampedThisBounds = true;
            isAbleToClamp = true;
        }
    }

    void FollowPlayerPos() {
        GameObject boundary = GameObject.Find("Boundary");

        if (boundary != null) {
            playerClampX = Mathf.Clamp(player.position.x, boundary.GetComponent<BoxCollider2D>().bounds.min.x + cameraBox.size.x / 2, boundary.GetComponent<BoxCollider2D>().bounds.max.x - cameraBox.size.x / 2);
            playerClampY = Mathf.Clamp(player.position.y, boundary.GetComponent<BoxCollider2D>().bounds.min.y + cameraBox.size.y / 2, boundary.GetComponent<BoxCollider2D>().bounds.max.y - cameraBox.size.y / 2);
        }

        // Seguindo o player com o Clamp no limite atual
        // Following the player with the Clamp at the current limit
        if (GameObject.Find("Boundary") && isAbleToClamp) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(playerClampX, playerClampY, transform.position.z), cameraDelay);
        }
    }

    // Debug
    void OnDrawGizmos() {
        // Limites da camera
        // Camera limits
        Gizmos.color = Color.green;

        if (cameraBox != null) {
            Vector3 rightSide = new Vector3(transform.position.x + cameraBox.size.x / 2, transform.position.y, 0f);
            Vector3 leftSide = new Vector3(transform.position.x - cameraBox.size.x / 2, transform.position.y, 0f);
            Vector3 topSide = new Vector3(transform.position.x, transform.position.y + cameraBox.size.y / 2, 0f);
            Vector3 bottomSide = new Vector3(transform.position.x, transform.position.y - cameraBox.size.y / 2, 0f);

            Gizmos.DrawWireSphere(rightSide, 0.5f);
            Gizmos.DrawWireSphere(leftSide, 0.5f);
            Gizmos.DrawWireSphere(topSide, 0.5f);
            Gizmos.DrawWireSphere(bottomSide, 0.5f);
        }

        // Limites do BoundaryManager
        // BoundaryManager limits
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(boundsMax, 0.5f);
        Gizmos.DrawWireSphere(boundsMin, 0.5f);
        Gizmos.DrawWireSphere(new Vector3(boundsMin.x, boundsMax.y, 0f), 0.5f);
        Gizmos.DrawWireSphere(new Vector3(boundsMax.x, boundsMin.y, 0f), 0.5f);
    }

}
