using UnityEngine;
using System.Collections;

public class NodeSquare {

	public Vector3 location;
	public Vector3 node_location;

	public bool isRobot = false;

	public NodeSquare(Vector3 _location, Vector3 _node_location){
		location = _location;
		node_location = _node_location;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3 getLoc(){
		return node_location;
	}

	public Vector3 getRenderLoc(){
		return location;
	}

	public void makeRobot(){
		isRobot = true;
	}

	public void demoteRobot(){
		isRobot = false;
	}
}
