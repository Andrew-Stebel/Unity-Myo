using UnityEngine;
using System.Collections;
using System;

public class MyoDoorknob : MyoTriggeredObject {

	public Action<MyoActor, MyoDoorknob> OnActivate;
	
	public float rotationThreshold = 0.9f;//the percent threshold needed to reach to activate the doorknob
	
	// Use this for initialization
	void Start () {

	}

	void OnEnable(){

		MyoInteractibles.OnGestureRecipeComplete += OnGestureRecipeComplete;
	}

	void OnDisable(){
		MyoInteractibles.OnGestureRecipeComplete -= OnGestureRecipeComplete;
	}

	void OnGestureRecipeComplete(GestureRecipeInstance gestureRecipe)
	{
		if (gestureRecipe.recipe.tag.Equals("DOORKNOB_LEFT"))
		{
			state = State.TURNING_LEFT;
		}
	}
	
	public enum State
	{
		DEFAULT,//able to receive gestures
		TURNING_RIGHT,//depending on if we are using the left handed myo or right.
		TURNING_LEFT,
		RETURNING,//just received a gesture...now returning back to baseline...
	}

	public State state = State.DEFAULT;
	
	void OnTriggerStay(Collider c)
	{

	}
		
	public float m_PercentRotated = 0.0f;
	private float m_RotationFactor = 1.0f;
	private float m_rotationVelocity = 0.0f;
	private float maxRotaion = 135.0f;

	//Called when the doorknob has been successfully activated
	void Activate()
	{
		if (Target != null)
		{
			Debug.Log(string.Format("{0} opened doorknob {1}", Target.name, this.name));
			if (OnActivate != null)
			{
				OnActivate(Target, this);
			}
		}
	}

	// Update is called once per frame
	void Update () {
	

		if (Target == null)return;

		switch(state)
		{
			case State.DEFAULT:

				
				break;

			case State.TURNING_RIGHT:

				m_PercentRotated-=Time.deltaTime*m_RotationFactor;
				this.transform.localRotation = Quaternion.Euler(0, m_PercentRotated*90.0f, 0);

				break;

			case State.TURNING_LEFT:
		
				//when the doorknob is in the turning state add to the percent rotated
				m_PercentRotated+=Time.deltaTime*m_RotationFactor;

				this.transform.localRotation = Quaternion.Euler(0, m_PercentRotated*90.0f, 0);
				break;


			case State.RETURNING:

				m_PercentRotated = Mathf.SmoothDamp(m_PercentRotated, 0.0f, ref m_rotationVelocity, Time.deltaTime); 
				this.transform.localRotation = Quaternion.Euler(0, m_PercentRotated*90.0f, 0);

				if (m_PercentRotated <= 0)
				{
					state = State.DEFAULT;
				}

				break;

			default:
		
				break;
		}

		m_PercentRotated = Mathf.Clamp (m_PercentRotated, 0.0f, 1.0f);

		if (m_PercentRotated >= rotationThreshold)
		{
			//return the doorknob to baseline and stop from registering gestures
			state = State.RETURNING;
			Activate();
		}

		this.transform.localRotation = Quaternion.Euler(0, 0, m_PercentRotated*maxRotaion);
	}
}
