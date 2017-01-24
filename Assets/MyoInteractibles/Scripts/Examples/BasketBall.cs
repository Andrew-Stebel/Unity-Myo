using UnityEngine;
using System.Collections;

public class BasketBall : MonoBehaviour {
	
	MyoTriggeredObject triggeredObject;
	
	public string triggeredEventTag = "PUNCH_BALL_START";

	//coutner to count whether or not we should still be the child of our dribbler
	public float bounceCounter = 0.0f;

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
				Vector3 force = new Vector3(0, -100.0f,0);

				this.GetComponent<Rigidbody>().AddForce(force);

				this.transform.parent = triggeredObject.Target.transform;
				if (triggeredObject.Target.rightHand != null)
				{
					this.transform.position = triggeredObject.Target.rightHand.position + new Vector3(0,-0.2f,0);
				}
				bounceCounter= 1.0f;
			}
		}
	}

	void Update(){

		if (bounceCounter > 0) {
			bounceCounter -= Time.deltaTime;


		} else {
			this.transform.parent = null;
		}
	}
}