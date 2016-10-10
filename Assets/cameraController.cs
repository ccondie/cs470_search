using UnityEngine;
using System.Collections;

public class cameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.LeftArrow)) {
			this.gameObject.transform.Translate (new Vector3 (0, 0, 1) * Time.deltaTime * 4);
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			this.gameObject.transform.Translate (new Vector3 (-1, 0, 0) * Time.deltaTime * 4);
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			this.gameObject.transform.Translate (new Vector3 (0, 0, -1) * Time.deltaTime*4);
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			this.gameObject.transform.Translate (new Vector3 (1, 0, 0) * Time.deltaTime*4);
		}

	
	}
}
