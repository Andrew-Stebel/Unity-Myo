using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MyoThirdPersonCharacter : MonoBehaviour
{
	public Transform rightShoulderResting;
	public Transform rightForearmRestingTransform;
	public Transform rightHandCalculated;


	public Transform leftShoulderResting;
	public Transform leftForearmRestingTransform;
	public Transform leftHandCalculated;

	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_JumpPower = 30f;


	public float speed = 10.0f;

	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;

	CharacterController m_CharacterController;
	bool m_Crouching;
	Animator m_Animator;

	float m_Height = 1.0f;

	void Start()
	{
	
		m_Animator = GetComponent<Animator>();
		m_CharacterController = GetComponent<CharacterController>();
	}
	


	Vector3 moveVector = Vector3.zero;

	public void Move(Vector3 move, bool crouch, bool jump)
	{
	
	
		m_ForwardAmount = move.z;
		m_TurnAmount = move.x;

		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);


		if (m_CharacterController.isGrounded) {
			moveVector = transform.forward * m_ForwardAmount * speed;
	

			if (jump)
				moveVector.y = m_JumpPower;
		}

		moveVector.y -= 20.0f * Time.deltaTime;
		m_CharacterController.Move(moveVector * Time.deltaTime);
	
		UpdateAnimator();
	}

	float groundedTime = 0.1f;


	void Update(){

		if (m_CharacterController.isGrounded)
		{
			groundedTime=0.1f;
		}
		else
		{
			groundedTime-=Time.deltaTime;
		}

	}


	void UpdateAnimator()
	{
		m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
		m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
		//m_Animator.SetBool("Crouch", m_Crouching);


		m_Animator.SetBool("OnGround", m_CharacterController.isGrounded || groundedTime > 0.0f);
		if (!m_CharacterController.isGrounded)
		{
			m_Animator.SetFloat("Jump", m_CharacterController.velocity.y);
		}


	}

	void ScaleHeightForCrouching(bool crouch)
	{
		if (m_CharacterController.isGrounded && crouch)
		{
			if (m_Crouching) return;
			m_CharacterController.height = m_Height / 2;
		
			m_Crouching = true;
		}
		else
		{
			m_Crouching = false;
		}
	}
		

	public float armLength = 0.4f;

	public float forearmLength = 0.2f;

	public Vector3 armVector;

	//This is the correction that we have on the extending of the arm on the x axis
	public float xCorrection = 3.0f;
	public float armZOffset = 180.0f;
	void OnAnimatorIK()
	{
		if (MyoInteractibles.RightMyo != null) {
		
			m_Animator.SetIKPositionWeight (AvatarIKGoal.RightHand, 0.8f);
			//m_Animator.SetIKRotationWeight (AvatarIKGoal.RightHand, 1.0f);  

			Quaternion forearmOrientation = MyoInteractibles.RightMyo.Orientation;
		
			//set the forearm rotation to be equal to the right orientation
			//rightForearmRestingTransform.localRotation = Quaternion.Euler (forearmOrientation.eulerAngles.x, forearmOrientation.eulerAngles.y, rightForearmRestingTransform.localRotation.z);
			rightForearmRestingTransform.localRotation = forearmOrientation;

			Vector3 handOffset = new Vector3 (0, Mathf.Cos (Mathf.Abs (forearmOrientation.x)) * armLength * 0.5f, 0);
		
			//the estimated hand position by taking into account the size of teh forearm + the current rotation we are at...
			Vector3 estimatedForearmHandPosition = handOffset + rightForearmRestingTransform.position + rightForearmRestingTransform.TransformVector (armVector) * forearmLength;

			m_Animator.SetIKPosition (AvatarIKGoal.RightHand, estimatedForearmHandPosition);
		} else if (MyoInteractibles.LeftMyo != null) {

			m_Animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, 0.8f);
			//m_Animator.SetIKRotationWeight (AvatarIKGoal.RightHand, 1.0f);  
			
			Quaternion forearmOrientation = MyoInteractibles.LeftMyo.Orientation;

			//set the forearm rotation to be equal to the right orientation
			//rightForearmRestingTransform.localRotation = Quaternion.Euler (forearmOrientation.eulerAngles.x, forearmOrientation.eulerAngles.y, rightForearmRestingTransform.localRotation.z);
			leftForearmRestingTransform.localRotation = forearmOrientation;
			
			Vector3 handOffset = new Vector3 (0, Mathf.Cos (Mathf.Abs (forearmOrientation.x)) * armLength * 0.5f, 0);
		
			//the estimated hand position by taking into account the size of teh forearm + the current rotation we are at...
			Vector3 estimatedForearmHandPosition = handOffset + leftForearmRestingTransform.position + leftForearmRestingTransform.TransformVector (armVector) * forearmLength;

			m_Animator.SetIKPosition (AvatarIKGoal.LeftHand, estimatedForearmHandPosition);
		}
	
	}
}