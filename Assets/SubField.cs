using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SubField {

	private Vector3 location;
	private Vector3 vector;

	public SubField(Vector3 _location, Vector3 _vector){
		location = _location;
		vector = _vector;
	}

	public Vector3 getLocation(){
		return location;
	}

	public Vector3 getVector(){
		return vector;
	}

}

