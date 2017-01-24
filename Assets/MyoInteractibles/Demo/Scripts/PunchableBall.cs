using UnityEngine;
using System.Collections;

public class PunchableBall : MonoBehaviour {

	MyoTriggeredObject triggeredObject;

	public string triggeredEventTag = "PUNCH_BALL_START";

	void Awake(){

		triggeredObject = this.gameObject.AddComponent<MyoTriggeredObject>();
		triggeredObject.triggerRadius = 3.0f;
	}

	void OnEnable(){
		MyoInteractibles.OnGestureRecipeEvent += OnGestureRecipeEvent;
	}

	void OnDisable(){

		MyoInteractibles.OnGestureRecipeEvent -= OnGestureRecipeEvent;
	}

	void OnGestureRecipeEvent(GestureRecipeInstance instance, string tag){
	
		if (triggeredObject.Target != null)
		{
			if (tag.Equals(triggeredEventTag))
			{
				Vector3 direction = (transform.position - triggeredObject.Target.transform.position);

				direction.y = 4.0f;
				direction = direction.normalized;

				float force=  500.0f;
				direction.y = 1.0f;

				this.GetComponent<Rigidbody>().AddForce(direction*force);
			}

		}
	}

}
