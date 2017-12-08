//------------------------------------------------------
//  Script responsable for controlling and processing
//  all player inputs.
//------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{

    [Header("Movement parameters")]
    [SerializeField]
    private float movementSpeed = 10f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float thrusterForce = 1000f;
    public bool thrustersActive = false;

    private PlayerMotor motor;
    private Animator theAnim;

    [Header("Spring settings")]
    [SerializeField] private float jointSpring = 20;
    [SerializeField] private float jointMaxForce = 40;
    private ConfigurableJoint joint;


    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        theAnim = GetComponent<Animator>();
        joint = GetComponent<ConfigurableJoint>();

        SetJointSettings(jointSpring);
    }

    private void Update()
    {
        if(Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

         RaycastHit _hit;  
         if(Physics.Raycast(transform.position, Vector3.down, out _hit, 100f))   // Check for ground underneath us
         {
            joint.targetPosition = new Vector3(0f, - _hit.point.y, 0f); // If we're not grounded, set joint position to greound
         }
         else
         {
             joint.targetPosition = new Vector3(0f, 0f, 0f); // Set joint position to 0,0,0
         }

        // Calculate movement as 3d vector
        float _xMove = Input.GetAxis("Horizontal");  // Get raw horizontal input
        float _zMove = Input.GetAxis("Vertical");    // Get raw vertical input

        Vector3 moveHorizontal = transform.right * _xMove;  // Calculate horizontal direction
        Vector3 moveVertical = transform.forward * _zMove;  // Calculate vertical direction

        Vector3 _velocity = (moveHorizontal + moveVertical).normalized * movementSpeed; // Final movement vector

        theAnim.SetFloat("fowardVelocity", _zMove); // Apply thruster movememnt animation

        motor.move(_velocity);   // Apply movement to motor


        // Calculate rb rotation as 3D vector for X roation
        float _yRotation = Input.GetAxisRaw("Mouse X"); // Get X rotation input

        Vector3 _rotation = new Vector3(0f, _yRotation, 0f) * mouseSensitivity;  // Calculate X rotation vector

        motor.rotate(_rotation);    // Apply rotation to motor


        // Calculate camera rotation as float for Y roation
        float _xRotation = Input.GetAxisRaw("Mouse Y"); // Get Y rotation input

        float _cameraXRotation = _xRotation * mouseSensitivity;  // Calculate Y rotation value for camera

        motor.rotateCamera(_cameraXRotation);    // Apply camera rotation to motor 

        if (thrustersActive)
        {
            // Calculate thruster force
            Vector3 _thrusterForce = Vector3.zero;
            if (Input.GetButton("Jump"))
            {
                _thrusterForce = Vector3.up * thrusterForce; // Set thruster force
                SetJointSettings(0f);   // Deactivate joint while flying
            }
            else
            {
                SetJointSettings(jointSpring);      // Re activate joint
            }

            motor.applyThruster(_thrusterForce);    // Aplly thruster force   
        }
    }

    private void SetJointSettings(float _jointSpring) {   // Set joint default settings
        joint.yDrive = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce};
    }

}
