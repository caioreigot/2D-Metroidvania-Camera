using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryManager : MonoBehaviour {

    public static BoundaryManager instance;

    public BoxCollider2D managerBox; // BoxCollider of the BoundaryManager
    private Transform player; // Position of Player
    public GameObject boundary; // The real camera boundary which will be activated and deactivated

    public bool enteredBounds = false;
    public bool alreadyClampedThisBounds = false;

    void Start() {
        managerBox = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        instance = this;
    }

    void Update() {
        ManageBoundary();
    }

    void ManageBoundary() {
        // Verificando se o player está dentro do limite da area
        if (managerBox.bounds.min.x < player.position.x && player.position.x < managerBox.bounds.max.x
        && managerBox.bounds.min.y < player.position.y && player.position.y < managerBox.bounds.max.y) {
            boundary.SetActive(true);
        } else {
            boundary.SetActive(false);
        }

        // Identificando se o player esta dentro de um novo limite
        if (boundary.activeSelf && !enteredBounds) {
            alreadyClampedThisBounds = false;
            enteredBounds = true;
        } else if (!boundary.activeSelf) {
            enteredBounds = false;
        }
    }

}
