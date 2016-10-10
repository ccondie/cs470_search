using UnityEngine;
using System.Collections;

public class ObjectFollow : MonoBehaviour {

	public GameObject toFollow;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.transform.position = toFollow.transform.position;
	}
}
