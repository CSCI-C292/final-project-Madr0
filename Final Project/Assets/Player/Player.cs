using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // constants
    [SerializeField] float _mouseSensitivity = 4f;
    [SerializeField] float _moveSpeed = 3f;
    [SerializeField] float _sprintSpeed = 3f;
    [SerializeField] float _crouchSpeed = 3f;
    [SerializeField] float _wallJumpSpeed = 3f;
    [SerializeField] float _jumpForce = 2f;
    [SerializeField] float _wallJumpForce = 2f;
    [SerializeField] float _gravity = -9.81f;
    [SerializeField] float _airControlMult = .2f;
    [SerializeField] float _slideSpeed = 3f;
    [SerializeField] float _slideSpeedDecay = 3f;
    [SerializeField] float _slideResetCooldown = 3f;
    
    // obj references
    Camera _cam;
    Vector3 camDefault;
    Vector3 camSlide;
    GameObject _pistol;
    GameObject _assaultRifle;
    CharacterController _controller;

    // "live" variables
    private float currentTilt = 0f;
    private Vector3 movementVector = Vector3.zero;
    private Vector3 speedVector = Vector3.zero;
    private Vector3 camSmoothVector;
    private float colliderSmoothFloat;
    private float moveSpeedBeforeJump;
    private float currentSlideSpeed;
    private float timeSinceLastSlide;
    private bool currentlyCrouching;
    
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        _controller = gameObject.GetComponent<CharacterController>();
        _cam = gameObject.GetComponentInChildren<Camera>();
        camDefault = _cam.transform.localPosition;
        camSlide = new Vector3(camDefault.x,camDefault.y-.15f,camDefault.z);
        _pistol = _cam.transform.Find("Pistol").gameObject;
        _assaultRifle = _cam.transform.Find("AssaultRifle").gameObject;
        currentSlideSpeed = _slideSpeed;
    }

    void Update() {
        Aim();
        if(!currentlyCrouching) {
            if(!Input.GetKey(KeyCode.LeftShift)) {
                moveSpeedBeforeJump = _controller.isGrounded ? _moveSpeed : moveSpeedBeforeJump;
                MomentumMovement(_moveSpeed);
            } else {
                moveSpeedBeforeJump = _controller.isGrounded ? _sprintSpeed : moveSpeedBeforeJump;
                MomentumMovement(_sprintSpeed);
            }
        } else {
            if (!Input.GetKey(KeyCode.LeftShift)) {
                moveSpeedBeforeJump = _controller.isGrounded ? _crouchSpeed : moveSpeedBeforeJump;
                MomentumMovement(_crouchSpeed);
            } else {
                SlideMovement();
            }
        }
        _controller.Move(speedVector * Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.E)) {
            SwapWeapons();
        }
    }

    void Aim() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        transform.Rotate(Vector3.up, mouseX * _mouseSensitivity);

        currentTilt -= (mouseY * _mouseSensitivity);
        currentTilt = Mathf.Clamp(currentTilt, -90,90);
        _cam.transform.localEulerAngles = new Vector3(currentTilt,0,0);
        RaycastHit hit;
        if(!Input.GetKey(KeyCode.LeftControl) && !Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, 1.5f)) {
            currentlyCrouching = false;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up)*hit.distance,Color.yellow);
            _controller.height = Mathf.SmoothDamp(_controller.height, 2f, ref colliderSmoothFloat, .05f);
            _cam.transform.localPosition = Vector3.SmoothDamp(_cam.transform.localPosition, camDefault, ref camSmoothVector, .05f);
        } else {
            currentlyCrouching = true;
            _controller.height = Mathf.SmoothDamp(_controller.height, 1f, ref colliderSmoothFloat, .05f);
            _cam.transform.localPosition = Vector3.SmoothDamp(_cam.transform.localPosition, camSlide, ref camSmoothVector, .05f);
        }
    }

    void MomentumMovement(float speed) {
        float speedMult = _controller.isGrounded ? 1f : _airControlMult;
        Vector3 sidewaysMovementVector = transform.right * Input.GetAxis("Horizontal") *speedMult;
        Vector3 forwardBackwardMovementVector = transform.forward * Input.GetAxis("Vertical") *speedMult;
        if(_controller.isGrounded) {
            movementVector = sidewaysMovementVector + forwardBackwardMovementVector;
            movementVector*=speed;
        } else {
            movementVector += sidewaysMovementVector + forwardBackwardMovementVector;
        }
        movementVector = _controller.isGrounded ? Vector3.ClampMagnitude(movementVector, speed) : Vector3.ClampMagnitude(movementVector, moveSpeedBeforeJump);
        float preserveY = speedVector.y;
        speedVector = movementVector;
        speedVector.y = preserveY;

        if(_controller.isGrounded) {
            if(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0) {
                movementVector = Vector3.zero;
            }
            if(Input.GetKey(KeyCode.Space)) {
                speedVector.y = _jumpForce;
            }
        } else if (!_controller.isGrounded) {
            speedVector.y += _gravity * Time.deltaTime;
        }

        timeSinceLastSlide += Time.deltaTime;
    }

    // walljump function
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if(!_controller.isGrounded && hit.normal.y < .1f){
            if(Input.GetKey(KeyCode.Space)) {
                Debug.DrawRay(hit.point,hit.normal,Color.red,1.25f);
                moveSpeedBeforeJump = _wallJumpSpeed;
                speedVector.y = _wallJumpForce;
                movementVector = Vector3.ClampMagnitude(Vector3.Reflect(movementVector,hit.normal)+hit.normal*moveSpeedBeforeJump/2,moveSpeedBeforeJump);
            }
        }
    }

    void SlideMovement() {
        movementVector = Vector3.Normalize(movementVector);
        float preserveY = speedVector.y;
        speedVector = movementVector * currentSlideSpeed;
        speedVector.y = preserveY;

        currentSlideSpeed -= _slideSpeedDecay * Time.deltaTime;
        currentSlideSpeed = Mathf.Clamp(currentSlideSpeed,0,_slideSpeed);
        if(timeSinceLastSlide >= _slideResetCooldown) {
            currentSlideSpeed = _slideSpeed;
        }
        timeSinceLastSlide = 0;
    }

    void SwapWeapons() {
        if(_assaultRifle.activeSelf == true) {
            _assaultRifle.SetActive(false);
            _pistol.SetActive(true);
        } else {
            _pistol.SetActive(false);
            _assaultRifle.SetActive(true);
        }
    }
}
