using UnityEngine;
using System.Collections;

public class NodeSquare {

	public Vector3 location;

	public NodeSquare(Vector3 _location){
		location = _location;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3 getLoc(){
		return location;
	}
}
