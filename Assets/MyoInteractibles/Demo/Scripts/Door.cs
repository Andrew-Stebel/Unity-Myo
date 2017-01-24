using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	public MyoDoorknob doorknob;
	public MyoDoorknob backdoornob;

	void OnEnable() {
		if (doorknob != null)
			doorknob.OnActivate += OnDoorknobActivate;
		if (backdoornob != null)
			backdoornob.OnActivate +=OnDoorknobActivate;
	}

	void OnDisable() {
		if (doorknob != null)
			doorknob.OnActivate -= OnDoorknobActivate;
		if (backdoornob != null)
			backdoornob.OnActivate -=OnDoorknobActivate;
	}

	public void OnDoorknobActivate(MyoActor actor, MyoDoorknob doorknob)
	{
		StartCoroutine(OpenRoutine());
	}

	IEnumerator OpenRoutine()
	{
		Open ();

		yield return new WaitForSeconds(3.0f);

		Close();
	}
	
	public void Open()
	{
		this.transform.localRotation = Quaternion.Euler(0,90, 0);
	}
	
	public void Close()
	{
		this.transform.localRotation = Quaternion.Euler(0,0, 0);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
