using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    UnitController Unit;
    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    private EventManager _eventManager;

    public Animator Animation;
    public float moveSpeed = 2;

    void Awake()
    {
        Unit = GetComponent<UnitController>();
        _eventManager = new EventManager();
        Animation = GetComponent<Animator>();
    }

    void Update()
    {
        if(Unit.UnitProperty.IsMoving)
        {
            Unit.ReadyToStep = false;
            Debug.DrawRay(transform.position, transform.forward);

            Vector3 Target = CoordToPosition(Unit.UnitProperty.X, Unit.UnitProperty.Y, Unit.FieldSize);
            Move(Target);
        }
        if(Unit.UnitProperty.IsMoving)
        {
            Animation.SetBool("IsWalking",true);
        }
        else
        {
            Animation.SetBool("IsWalking", false);
        }
    }

    public void Move(Vector3 target)
    {
        if (Vector3.Distance(transform.position, target) >= 0.05f)
        {            
            CalculateHeading(target);
            SetHorizotalVelocity();
            
            //Locomotion
            transform.forward = heading;
            transform.position += velocity * Time.deltaTime;
        }
        else
        {
            //Tile center reached
            transform.position = target;
            Unit.ReadyToStep = true;
            _eventManager.OnStep();
        }
    }

    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    void SetHorizotalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    Vector3 CoordToPosition(int x, int y, int mapSize)
    {
        return new Vector3(-mapSize / 2 + 0.5f + x, Unit.transform.position.y, -mapSize / 2 + 0.5f + y);
    }
}
