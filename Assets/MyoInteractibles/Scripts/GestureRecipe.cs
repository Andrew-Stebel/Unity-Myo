using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MyoRole = MyoInteractibles.MyoRole;

using Gesture = MyoInteractibles.Gesture;
using GestureState = MyoInteractibles.GestureState;

using System;

[Serializable]
public class GestureRecipe : ScriptableObject
{
	public Item Root
	{
		get {
			
			if (itemList.Count > 0)return itemList[0];
			return null;
		}
	}
	
	public List<Item> itemList = new List<Item>();
	public string tag;//this is the unique identifier for this gesture recipe so that items can be called when this is triggered

	[Serializable]
	public class Item{
		
		public MyoRole myoRole = MyoRole.PRIMARY;//the myo role that this gesture must be part of
		public Gesture gesture = Gesture.UNKNOWN;
		public GestureState gestureState;//the state of the gesture...e.g. must be starting or finishing or just happening...
		//how long does this gesture have to last in seconds
		public float minDuration;
		public float maxDuration;
		//the last gesture in this recipe must have happened between this defined range
		public float delayFromLastMin = 0.1f;
        public float delayFromLastMax = 0.8f;
		//the event tag that gets triggered at the end of this step
		public string eventTag;

		//the range of the value of the event...e.g. if a velocity threshold, gets triggered if velocity within this range
		//this threshold is not applicable to all interactible gestures since pose gestures are binary (on or off)
		public float minValue;
		public float maxValue;

	}
}


