﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryManager : MonoBehaviour {

    private BoxCollider2D managerBox; // BoxCollider of the BoundaryManager
    private Transform player; // Position of Player
    public GameObject boundary; // The real camera boundary which will be activated and deactivated

    private bool alreadyEntered = false;

    void Start() {
        managerBox = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update() {
        ManageBoundary();
    }

    void ManageBoundary() {
        if (managerBox.bounds.min.x < player.position.x && player.position.x < managerBox.bounds.max.x
        && managerBox.bounds.min.y < player.position.y && player.position.y < managerBox.bounds.max.y) {
            boundary.SetActive(true);
        } else {
            boundary.SetActive(false);
        }

        // Identifying if player entered a new boundary
        if (boundary.activeSelf && !alreadyEntered) {
            alreadyEntered = true;
        } else if (!boundary.activeSelf) {
            alreadyEntered = false;
        }
    }

}
