using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	public float speed;
	public float def_speed = 50;

	//Used to keep track of "forward direction" in manual control
	float rotationAngle = 0;
	//Indicates which way is "forward" in manual control
	GameObject followArrow;
	public RunType runtype; 
	public enum RunType{one, two, three};

	List<Field> rand_fields = new List<Field>();
	List<Field> att_fields = new List<Field>();
	List<Field> rep_fields = new List<Field> ();

	// Use this for initialization
	void Start () {
		followArrow = GameObject.FindObjectOfType<ObjectFollow> ().gameObject;
		att_fields = getFieldofType (1);
		rep_fields = getFieldofType (2);
		rand_fields = getFieldofType (3);

		foreach (Field rand_field in rand_fields) {
			float field_radius = rand_field.getRadius ();
			float dist_from_center = Mathf.Sqrt (field_radius * field_radius / 2);
			for (float x = -dist_from_center; x < dist_from_center; x += 7) {
				for (float y = -dist_from_center; y < dist_from_center; y += 7) {

					// seed the random field with random vectors
					Vector3 location = new Vector3(rand_field.getLocation().x + x, 0, rand_field.getLocation().y + y);
					float vec_x = Random.Range (-1, 1);
					float vec_y = Random.Range (-1, 1);
					Vector3 vector = new Vector3 (vec_x, 0, vec_y);

					rand_field.sub_fields.Add (new SubField (location, vector));
				}
			}
		}

	}


	float vec_length(Vector3 v) {
		return Mathf.Sqrt (Mathf.Pow (v [0], 2) + Mathf.Pow (v [1], 2) + Mathf.Pow (v [2], 2));
	}
	// Update is called once per frame
	void Update () {

		Vector3 toMove = getManualInput ();
	
		//Alter the toMove variable here based on the varius fields in the scene.
		//You can set the runtype in Unity in the Inspector Window
		switch (runtype) {
		case RunType.one:
			//For Part 1 of the lab
			Vector3 my_pos = myLocation ();

			Field goal = att_fields [0];
			Vector3 goal_pos = goal.getLocation ();
			float goal_dist = getDistance (goal);

			float exp_shift = (float)1.3;
			float goal_scalar = (float)1 + (1 / (goal_dist - exp_shift));
			goal_scalar = 1;

			Vector3 move_force = goal_scalar * ((goal_pos - my_pos) / vec_length (goal_pos - my_pos));

			// counter the robot's velocity based on how close to the goal it is
			float counter_speed_radius = goal.getRadius () * (float)0.1;
			if (goal_dist < goal.getRadius ()) {
				move_force = move_force - (counter_speed_radius / goal_dist) * myVelocity ();
			}


			// apply the influence of the repulsive fields, by unit vector and scalar
			for (int i = 0; i < rep_fields.Count; i++) {
				Field cur_rep = rep_fields [i];
				Vector3 cur_pos = cur_rep.getLocation ();
				float cur_dist = getDistance (cur_rep);
				float cur_scalar = 0;

				if (cur_dist < cur_rep.getRadius ()) {
					cur_scalar = Mathf.Abs ((1 / (float)Mathf.Pow ((cur_dist - exp_shift), (float)1.2)));
				} else {
					speed = def_speed;
					cur_scalar = 0;
				}

				if (cur_dist < cur_rep.getRadius () * (float)0.45) {
					speed = (cur_scalar * 25);
				}

				move_force += cur_scalar * ((my_pos - cur_pos) / vec_length (my_pos - cur_pos));
			}

			// handle the influence of a random fields
			int rand_vector_sample = 2;
			foreach (Field rand_field in rand_fields) {
				if (getDistance (rand_field) < rand_field.getRadius ()) {
					List<Vector3> influences = closest_sub_fields (rand_field.sub_fields, rand_vector_sample);
					foreach (Vector3 push in influences) {
						move_force += move_force + (float)0.5 * push;
					}
				}
			}
				
			move (move_force);


			break;
		case RunType.two:
			//For Part 2 of the lab
			//	readSonars ();
			break;

		case RunType.three:
			//For whatever else
			//feel free to add more 

			break;

		}

	
		move (toMove);


	}

	public List<Vector3> closest_sub_fields(List<SubField> subs, int sampleSize){
		SortedDictionary<float, SubField> distances = new SortedDictionary<float, SubField> ();
		List<Vector3> return_me = new List<Vector3> ();

		foreach (SubField sub in subs) {
			distances.Add(Vector3.Distance(sub.getLocation(), myLocation()), sub);
		}

		int count = 0;
		foreach (KeyValuePair<float,SubField> entry in distances) {
			if (count < sampleSize) {
				return_me.Add(entry.Value.getVector());
			}
		}
					/*
		for (int i = 0; i < sampleSize; i++) {
			float key = distances.Keys [i];
			return_me.Add(distances[key].getVector());
		}*/

		return return_me;
	}


	// add a force to me in the given direction, this input force is normalized (made to have a magnitude of 1) for your convenience
	// you can control its total acceleration by altering the speed if you like.
	public void move(Vector3 direction)
	{
		this.GetComponent<Rigidbody>().AddForce( direction.normalized * Time.deltaTime * speed);

	}



	// Returns a List of 16 floats of the distance to a given object, the first float is point in the north direction,
	//and then they increment clockwise going around the circle. if nothing is detected, it will be a number of size 25
	// Example, an object to the north would be in the 0 index, a object to the east (90 degrees) would be in the 4th index and so on
	public List<float> readSonars() {

		List<float> myContacts = new List<float> ();
		//Vector3 loc = myLocation ();
		Vector3 fwd = new Vector3(1,0,0);

		for (int i = 0; i < 16; i++) {

			RaycastHit hitobj ;
			float distance = 20;
			if (Physics.Raycast ( transform.position, fwd, out hitobj, 25)) {
				Vector3 hitlocation = hitobj.point;
				distance = Vector3.Distance (hitlocation, transform.position);

				Debug.Log (" hit at "+ (360 / 16) *(i));

			}
			myContacts.Add (distance);
			//Rotate the raycast by 22ish degrees.
			fwd = Quaternion.AngleAxis ((360 / 16), Vector3.up) * fwd;
		

		}
		return myContacts;



	}

	//Distance between me and a given field
	public float getDistance(Field f)
	{
		return Vector3.Distance(f.getLocation(), myLocation());
	}


	public Vector3 myLocation()
	{
		return this.transform.position;
	}

	public Vector3 myVelocity(){
		return this.GetComponent<Rigidbody> ().velocity;
	}




	//Get all Fields.
	public List<Field> getPowerFields()
	{List<Field> myList = new List<Field> ();
		foreach (Field f in GameObject.FindObjectsOfType<Field> ()) {
			myList.Add (f);

		}
		return myList;
	}

	//Only get fields that match the n fieldType in range of the robot .
	public List<Field> getFieldsInRange(int n)
	{
		List<Field> myList = new List<Field> ();
		foreach (Field f in GameObject.FindObjectsOfType<Field> ()) {
			if (f.getFieldType() == n  && Vector3.Distance(myLocation(), f.getLocation()) <= f.getRadius()) {
				myList.Add (f);			
			}
		}

		return myList;

	}
	//get all fields in range of the robot.
	public List<Field> getFieldsInRange()
	{
		List<Field> myList = new List<Field> ();
		foreach (Field f in GameObject.FindObjectsOfType<Field> ()) {
			if (Vector3.Distance(myLocation(), f.getLocation()) <= f.getRadius()) {
				myList.Add (f);			
			}
		}

		return myList;

	}
	//get all fields that match this type.
	public List<Field> getFieldofType(int n)
	{
		List<Field> myList = new List<Field> ();
		foreach (Field f in GameObject.FindObjectsOfType<Field> ()) {
			if (f.getFieldType() == n) {
				myList.Add (f);			
			}
		}

		return myList;

	}


	// get input from the AWSD keys.
	public Vector3 getManualInput()
	{Vector3 MoveDirection = Vector3.zero;
		if (Input.GetKey (KeyCode.W)) {
			MoveDirection += (Quaternion.Euler(0,rotationAngle,0)* Vector3.forward * Time.deltaTime * speed/2);
		}

		else if (Input.GetKey (KeyCode.S)) {
			MoveDirection += (Quaternion.Euler(0,rotationAngle,0)* Vector3.forward * Time.deltaTime * -speed/2);
		}

		if (Input.GetKey (KeyCode.D)) {
			rotationAngle += 180 * Time.deltaTime;
		}
		else if (Input.GetKey (KeyCode.A)) {
			rotationAngle -= 180 * Time.deltaTime;
		}
		followArrow.transform.rotation = Quaternion.Euler (0, rotationAngle -90, 0);

		return MoveDirection;
	}


}
