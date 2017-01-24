using UnityEngine;
using System.Collections;


/**
 * This class represents an object that can be triggered by the presence of the myo actor
 * */
public class MyoObjectListener : MonoBehaviour {
	
	
	public static int layer = 14;
	
	public float triggerRadius = 1.0f;
	
	[HideInInspector]
	private MyoActor targetActor;
	protected SphereCollider trigger;
	
	void Awake(){
		
		//create the trigger object automatically
		trigger = this.gameObject.AddComponent<SphereCollider>();
		trigger.isTrigger = true;
		trigger.radius = triggerRadius;
	}

	//player (actor) is in range
	void OnTriggerEnter(Collider c)
	{
		MyoActor actor = c.GetComponent<MyoActor> ();
		if (actor != null)
		{
			print ("Actor is in range");
			SetTargetActor(actor);
		}
	}

	//player (actor) has left range
	void OnTriggerExit(Collider c)
	{
		MyoActor actor = c.GetComponent<MyoActor> ();
		if (actor != null)
		{
			//if the actor that is leaving our trigger is our current target then remove the target actor 
			if (actor == targetActor)
			{
				SetTargetActor(null);
			}
		}
	}
	
	void SetTargetActor(MyoActor actor)
	{
		targetActor = actor;
	}
	
	public MyoActor Target
	{
		get {return this.targetActor;}
	}
	
}
