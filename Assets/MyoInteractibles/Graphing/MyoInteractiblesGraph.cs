using UnityEngine;
using System.Collections;

public class MyoInteractiblesGraph : MonoBehaviour {


	private int minX = 0;//number of units 
	private int maxX = 500;

	private int minY = -100;
	private int maxY = 100;
	
	public GameObject[,] gridPoints;
	
	// Use this for initialization
	void Start () {

		//create a grid with as many points in it as the min - the max
	
		gridPoints = new GameObject[maxX-minX,maxY-minY];

		for(int x = minX; x < maxX;x++)
		{
			for (int y = minY; y < maxY;y++)
			{
				GameObject gp = new GameObject("" + x + "," + y);
				gp.AddComponent<LineRenderer>();
				gp.transform.parent = this.transform;
				gridPoints[x-minX,y-minY] = gp;
			}
		}
	
		
	
	}




	// Update is called once per frame
	void Update () {
	
	}
}
