using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public enum State{
        OUTER_FLYING,
        INNER_FLYING,
        WALKING
    };
    private const float LITTLE = .01f;
    private const float SMOOTH_COEF = 0.1f;
    [SerializeField] private GameObject head;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private float lookSpeed = 2.0f;
    private float lookXLimit = 50f;
    private Vector3 lastGravity;

    private const float WALK_SPEED = 2.0f;

    private Vector3 velocity = new Vector3(0, 0, 0);

    public PlayerStateMachine stateMachine;
    public OuterSpaceState outerFlying;
    public WalkingState walking;
    public InnerFlyState innerFly;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        stateMachine = new PlayerStateMachine();
        outerFlying = new OuterSpaceState(this, stateMachine);
        innerFly = new InnerFlyState(this, stateMachine);
        walking = new WalkingState(this, stateMachine);
        stateMachine.Initialize(outerFlying);
        lastGravity = WorldPhysics.GetLocalGravity(transform.position);        
    }


    void Update()
    {
        stateMachine.GetState().HandleInput();
        stateMachine.GetState().MoveUpdate();
    }

    public Transform GetTransform()
    {
        return transform;
    }
    public void MoveBody(Vector3 walkAccel)
    {
        //Ускоряем по внешним силам:
        velocity += WorldPhysics.GetTotalAccel(transform.position, velocity) * Time.deltaTime;
        velocity += walkAccel * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }

    public void RotateHead(float gravitySmoothing = SMOOTH_COEF)
    {
        Vector3 gravity = WorldPhysics.GetLocalGravity(transform.position);
        //Вектор гравитации должен быть сглажен на случай резкой смены направдения гравитации:
        bool isOpposite = true;
        while(isOpposite)
        {
            if ((gravity + lastGravity).sqrMagnitude < LITTLE) Debug.Log((gravity + lastGravity).sqrMagnitude);
            if ((gravity + lastGravity).sqrMagnitude > LITTLE) isOpposite = false;
            else
            {
                lastGravity += new Vector3(Random.value, Random.value, Random.value) * LITTLE;
                lastGravity.Normalize();   
            }        
        }

        gravity = Vector3.SlerpUnclamped(lastGravity, gravity, gravitySmoothing * Time.deltaTime);

        Vector3 targetDir = Vector3.Cross(-gravity, gravity - new Vector3(0.2f, 0.2f, 0.2f));
        Vector3 target = transform.position + targetDir;
        transform.LookAt(target, -gravity);
        lastGravity = gravity;

        //Head rotation:
        rotationY += Input.GetAxis("Mouse X") * lookSpeed;        
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        head.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, rotationY * lookSpeed, 0); 
    }

    public PlayerState GetState(State stateName)
    {
        switch(stateName)
        {
            case State.OUTER_FLYING:
            return outerFlying;
            case State.WALKING:
            return walking;
            case State.INNER_FLYING:
            return innerFly;
            default:
            return walking;
        }
    }

}
