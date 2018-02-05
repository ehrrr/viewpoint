﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Header("Control sensitivity")]
	public float movementSpeed;
	public float jumpForce; // upward force applied to jump
	public float fallForce; // downward force applied to falling
	public float lowJumpForce; // jump adjustment force
	//public float speed;

	[Header("Other player settings")]
	public float fallThreshold; // how far the player falls before respawning

	private Rigidbody rb; // reference to rigidbody of this player
	private bool grounded = true; // true iff player is not in the air
	private Vector3 startPosition; // start position of player

	WorldManager worldManager; // reference to WorldManager

	void Start() {
		worldManager = WorldManager.instance; // set the reference to WorldManager instance
		rb = GetComponent<Rigidbody> (); // get the rigidbody of this player object
		startPosition = transform.position; // store the start position of the player
	}

	void Update () {
		//speed = rb.velocity.y;
		// player movement
		if (Input.GetAxis ("Horizontal") > 0) {
			transform.position += transform.right * Time.deltaTime * movementSpeed;
		}
		if (Input.GetAxis ("Horizontal") < 0) {
			transform.position += -transform.right * Time.deltaTime * movementSpeed;
		}
		if (Input.GetAxis ("Vertical") > 0 && !worldManager.mode2d) {
			transform.position += transform.forward * Time.deltaTime * movementSpeed;
		}
		if (Input.GetAxis ("Vertical") < 0 && !worldManager.mode2d) {
			transform.position += -transform.forward * Time.deltaTime * movementSpeed;
		}

		// jump
		if (grounded && Input.GetKeyDown(KeyCode.Space)) {
			rb.velocity = Vector3.up * jumpForce;
			grounded = false;
		}
		// fall faster after jumping up
		if (rb.velocity.y < 0) {
			rb.velocity += Vector3.up * Physics.gravity.y * fallForce * Time.deltaTime;
		} else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)) {
			rb.velocity += Vector3.up * Physics.gravity.y * lowJumpForce * Time.deltaTime;
		}

		// respawn in start position if player falls off plane
		if (transform.position.y < fallThreshold){
			transform.position = startPosition;
		}
	}

	// return true if player is allowed to jump again
	// (helps prevent infinite jumping)
	void OnCollisionStay(Collision col){
		if (col.transform.tag == "MeshDiff" || col.transform.tag == "ColliderDiff" || col.transform.tag == "Ground") {
			grounded = true;
			//Debug.Log ("Grounded again");
		} 
	}

	// collecting the coin objects
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Coin")){
			other.gameObject.SetActive(false);
		}
	}

}
