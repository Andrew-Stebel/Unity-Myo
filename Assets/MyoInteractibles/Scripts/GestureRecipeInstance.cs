using UnityEngine;
using System.Collections;

/**
 * Gesture recipe instance is an instance of gesture recipe that is currently in an active state somewhere
 * in it's recipe state machine logic.  E.g. the initial static machine parameters have pasesd and 
 * */
public class GestureRecipeInstance
{
	public int stepIndex;
	public float timestamp;//when the recipe started
	public float lastEventTimestamp;//when the last event occurred
	
	public GestureRecipe recipe;
	
	public GestureRecipeInstance(GestureRecipe recipe, float timestamp)
	{
		this.recipe = recipe;
		this.timestamp = timestamp;
	}
	
	public bool Completed
	{
		get { return stepIndex >= recipe.itemList.Count;}
	}
	
	public GestureRecipe.Item CurrentItem{
		get {
			if (stepIndex >= 0 && stepIndex < recipe.itemList.Count){
				
				return recipe.itemList[stepIndex];
			}
			return null;
		}
	}
	
	public void GoNext(float timestamp)
	{
		stepIndex++;
		this.lastEventTimestamp = timestamp;
	}
	
	public bool Satisfied(GestureEvent e)
	{
		GestureRecipe.Item item = CurrentItem;
		
		if (item != null)
		{
			bool s = ItemSatisfied(item, e, lastEventTimestamp);
			
			return s;
		}
		return false;
	}
	
	public bool Rejected(GestureEvent e)
	{
		GestureRecipe.Item item = CurrentItem;
		
		if (item != null)
		{
			bool s = ItemRejected(item, e, lastEventTimestamp);
			
			return s;
		}
		return false;
	}
	
	/**
	 * Check to see if this item is satisfied in the current state
	 * */
	public static bool ItemSatisfied(GestureRecipe.Item item, GestureEvent e, float timeSinceLast, bool isRoot = false){
		bool result = false;
		
		if (e is SingleGestureEvent)
		{
			result = true;

			if (!isRoot)
			{
				if (timeSinceLast < item.delayFromLastMin && item.delayFromLastMin > 0)
				{
					result = false;
				}
			}
			
			if (e.mGesture == item.gesture)
			{
				MyoInteractibles.GestureState state = e.gestureState;
				
				MyoInteractibles.GestureState otherState = item.gestureState;

				if (state != otherState)
				{
					result =false;
				}
			}
			else
			{
				result = false;
			}
		}
		
		return result;
	}
	
	/**
	 * Check to see if this recipe item has been rejected.
	 * This could be caused due to the item not being performed in time, or for long enough, or it has been made in valid by another gesture
	 * */
	public bool ItemRejected(GestureRecipe.Item item, GestureEvent e, float timeSinceLast){
		
		bool result = false;
		
		if (e is SingleGestureEvent){
			result = false;
			if ((Time.time - timeSinceLast) > item.delayFromLastMax && item.delayFromLastMax > 0)
			{
				result = true;
			}
		}
		return result;
	}
	
}