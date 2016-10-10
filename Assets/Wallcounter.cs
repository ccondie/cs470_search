using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Wallcounter : MonoBehaviour {


	public Text myText;
	int counter;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void incrementCounter()
	{
		counter++;
		myText.text = "Walls Hit: " + counter;

	}


}
