//------------------------------------------------------
//  Script responsable for controlling player
//  movement.
//------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    private Vector3 velocity = Vector3.zero;    // Velocity of rigidbody
    private Vector3 rotation = Vector3.zero;     // Roation of rb
    private Vector3 thrusterForce = Vector3.zero;   // Thruster force

    private Rigidbody theRb;    // The RB

    [Header("Camera rotation")]
    [SerializeField] private Camera theCamera;  // Player camera
    private float cameraXRotation = 0f;  // Rotation of camera
    [SerializeField] private float cameraRotationLimit = 85f;
    private float currentCameraRotationX = 0f;




    private void Start()
    {
        theRb = GetComponent<Rigidbody>();  // We get  rigidbody
    }

    private void FixedUpdate()  // Runs every physics iteration
    {
        PerformMovement();   // Performs movement     
        PerformRotation();  // Performs rotation
        PerformThruster();  // Apply Thruster 
    }

    private void PerformMovement() { // Performs movement
        if (velocity != Vector3.zero) { // If our velocity input is not zero
            theRb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);    // Move rb with physics to position + velocity
        }
    }

    private void PerformRotation() {
        theRb.MoveRotation(theRb.rotation * Quaternion.Euler(rotation));    // Rotate rb with rotation + euler of rotation input

        if(theCamera != null) { // If we have a camera
            currentCameraRotationX -= cameraXRotation;  // Set our rotation of camera X
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // Clamp rotation X
            theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f ,0f); // Apply camera rotation X
        }
    }

    private void PerformThruster() {
        if (thrusterForce != Vector3.zero) {
            theRb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }


    public void applyThruster(Vector3 _thrusterForce) {
        thrusterForce = _thrusterForce;
    }
    public void rotate(Vector3 _rotation) { // Gets rotation vector from input
        rotation = _rotation;
    }
    public void move(Vector3 _velocity) {   // Gets movement vector from input
        velocity = _velocity;
    }
    public void rotateCamera(float _cameraXRotation) {// Gets movement vector for camera from input
        cameraXRotation = _cameraXRotation;
    }

}
