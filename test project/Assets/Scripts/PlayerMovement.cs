using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PlayerMovement : MonoBehaviour {

    private Rigidbody2D playerBody;

    float currentSpeed = 0f;
    //float weight = 10f;
    float maxSpeed = 3f;
    float acceleration = 0.002f;
    static float maxStamina = 50f;
    float stamina = maxStamina;
    float moveDirection = 0;
    float speedModifier = 0f;

    MovementType movementType = MovementType.Walking;

    public LayerMask wallMask;
    //public LayerMask playerMask;
    //public LayerMask itemMask;
    //public LayerMask vaultMask;

    void Start() {
        playerBody = GetComponent<Rigidbody2D>();
    }

    void Update() {
        ChangeMovementType(ref movementType, ref stamina, ref speedModifier, maxStamina);
        (bool, float) movement = GetRotation();
        Movement(speedModifier, movement.Item1, movement.Item2, ref currentSpeed, maxSpeed, playerBody, acceleration);
    }


    static (bool, float) GetRotation() {
        //gets rotation of player and if they are moving

        int horizontal = Convert.ToInt16(Input.GetAxisRaw("Horizontal"));
        int vertical = Convert.ToInt16(Input.GetAxisRaw("Vertical"));
        (int, int) hv = (horizontal, vertical);
        switch (hv)
        {
            case (0, 1):
                return (true, 0);
            case (1, 1):
                return (true, Convert.ToSingle(0.25 * Math.PI));
            case (1, 0):
                return (true, Convert.ToSingle(0.5 * Math.PI));
            case (1, -1):
                return (true, Convert.ToSingle(0.75 * Math.PI));
            case (0, -1):
                return (true, Convert.ToSingle(Math.PI));
            case (-1, -1):
                return (true, Convert.ToSingle(1.25 * Math.PI));
            case (-1, 0):
                return (true, Convert.ToSingle(1.5 * Math.PI));
            case (-1, 1):
                return (true, Convert.ToSingle(1.75 * Math.PI));
            default:
                return (false, Convert.ToSingle(Math.PI));
        }
    }

    static void Movement(float speedModifier, bool isMoving, float radiansFromNorth, ref float currentSpeed, float maxSpeed, Rigidbody2D playerBody, float acceleration) {
        //moves the player with wasd at the correct speeds

        Vector2 moveVector;
        moveVector.y = Mathf.Cos(radiansFromNorth);
        moveVector.x = Mathf.Sin(radiansFromNorth);
        Vector2 lastVector;
        lastVector.x = 0f;
        lastVector.y = 0f;

        if (isMoving) {
            currentSpeed = Mathf.Clamp((currentSpeed + acceleration), 0, ((maxSpeed - 1) * speedModifier));
            playerBody.velocity = moveVector * currentSpeed * Time.deltaTime * 1000;
            lastVector = moveVector;
            Debug.Log($"{playerBody.velocity.magnitude}");
        }
        else {
            currentSpeed = Mathf.Clamp((currentSpeed - acceleration), 0, ((maxSpeed - 1) * speedModifier));
            playerBody.velocity = lastVector * currentSpeed * Time.deltaTime * 1000;
        }
        //Debug.Log($"moveVector:{moveVector}, " +
        //    $"velocity:{playerBody.velocity}, " +
        //    $"currentSpeed:{currentSpeed}, " +
        //    $"velocity.magnitude:{playerBody.velocity.magnitude}, " +
        //    $"acceleration:{acceleration}");
    }

    static void ChangeMovementType(ref MovementType movementType, ref float stamina, ref float speedModifier, float maxStamina) {
        //changes movement speed and stamina depending on wether play is running, sneaking or walking

        if (Input.GetKeyDown("left shift") && stamina > 10) {
            movementType = MovementType.Running;
        }
        else if (Input.GetKeyDown("left ctrl")) {
            movementType = MovementType.Sneaking;
        }
        else if ((Input.GetKeyUp("left ctrl") && movementType == MovementType.Sneaking) || 
            (Input.GetKeyUp("left shift") && movementType == MovementType.Running) ||
            stamina == 0) {
            movementType = MovementType.Walking;
        }   

        if (movementType == MovementType.Running) {
            speedModifier = 1.5f;
            stamina = Mathf.Clamp(stamina - (10 * Time.deltaTime), 0, maxStamina);
            Debug.Log($"running {stamina}");
        }
        else if (movementType == MovementType.Sneaking) {
            speedModifier = 0.5f;
            stamina = Mathf.Clamp(stamina + (10 * Time.deltaTime), 0, maxStamina);
            Debug.Log($"sneaking {stamina}");
        }
        else {
            speedModifier = 1f;
            stamina = Mathf.Clamp(stamina + (5 * Time.deltaTime), 0, maxStamina);
            Debug.Log($"walking {stamina}");
        }
    }

    public enum MovementType {
        //enum for movement type
        Walking,
        Running,
        Sneaking
    }
}
