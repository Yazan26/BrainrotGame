using System;
using Unity.Android.Gradle.Manifest;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   private CharacterController controller;
   [SerializeField] public float forwardspeed = 10f;
   [SerializeField] public float sideSpeed = 10f;
   [SerializeField] private float VerticalVelocity;
   [SerializeField] public float jumpPower = 5f;
   private int targetLane = 1; // 0 = left, 1 = middle, 2 = right
   [SerializeField] private float laneDistance = 3f; // Distance between lanes
   public Animator animator;
   private void Start()
   {
      controller = GetComponent<CharacterController>();
   }

   private void Update()
   {
      
   }

   private void HandleForwardMovement()
   {
      Vector3 forwardMove = transform.forward * forwardspeed * Time.deltaTime;
         Vector3 worldForwardMove = transform.TransformDirection(forwardMove);
         controller.Move(worldForwardMove);
   }
   
   private void handleLaneSwitching()
   {
      if (Input.GetKeyDown(KeyCode.LeftArrow))
      {
         moveLane(-1);
      }
      else if (Input.GetKeyDown(KeyCode.RightArrow))
      {
         moveLane(1);
      }
   }

   private void moveLane(int direction)
   {
      targetLane += direction;
      targetLane = Math.Clamp(targetLane, 0, 2);
      
   }
   
   private void MoveTowardsTargetLane()
   {
      Vector3 targetPosition = CalculateTargetPosition();
      if (transform.position != targetPosition)
      {
         Vector3 diff = targetPosition - transform.position;
         Vector3 movedirection = diff.normalized * sideSpeed * Time.deltaTime;

         if (movedirection.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(diff);
         else
            controller.Move(diff);
      }
   }


   private Vector3 CalculateTargetPosition()
   {
      Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
      if (targetLane == 0)
      {
         targetPosition += Vector3.left * laneDistance;  
      }
      else if (targetLane == 2)
      {
         targetPosition += Vector3.right * laneDistance;  
      }
      return targetPosition;
   }
   private void handlejumping()
   {
    VerticalVelocity = 0f;
    if (Input.GetKeyDown(KeyCode.UpArrow))
    {
       VerticalVelocity = jumpPower;
       animator.SetTrigger("Jump");
    }
    else VerticalVelocity += Physics.gravity * Time.deltaTime;
    
   }
   
   
   
}
