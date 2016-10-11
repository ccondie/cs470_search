using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	public float speed;
	public float def_speed = 50;
	public NodeSquare[,] nodes;

	//Used to keep track of "forward direction" in manual control
	float rotationAngle = 0;
	//Indicates which way is "forward" in manual control
	GameObject followArrow;
	public RunType runtype; 
	public enum RunType{one, two, three};

	List<Field> rand_fields = new List<Field>();
	List<Field> att_fields = new List<Field>();
	List<Field> rep_fields = new List<Field> ();

	List<NodeSquare> path = new List<NodeSquare> ();

	// Use this for initialization
	void Start () {

		ObjectBuilderScript script = GameObject.FindObjectOfType<ObjectBuilderScript>();
		script.BuildObject ();

		followArrow = GameObject.FindObjectOfType<ObjectFollow> ().gameObject;
		nodes = ObjectBuilderScript.nodes;

		Vector3 roboLoc = myLocation ();
		Vector3 goalLoc = GameObject.FindGameObjectWithTag("Goal").transform.position;
		float startDist = float.MaxValue;
		float goalDist = float.MaxValue;
		NodeSquare start = new NodeSquare(new Vector3(0,0,0), new Vector3(0,0,0));
		NodeSquare goal = new NodeSquare(new Vector3(0,0,0), new Vector3(0,0,0));

		Debug.Log ("Pre-Calculations");
		Debug.Log (roboLoc);
		Debug.Log (goalLoc);

		foreach (NodeSquare node in nodes) {
			if (Vector3.Distance (node.getRenderLoc(), roboLoc) < startDist) {
				Debug.Log ("Reassign A");
				start = node;
				startDist = Vector3.Distance (node.getRenderLoc (), roboLoc);
			}

			if (Vector3.Distance (node.getRenderLoc (), goalLoc) < goalDist) {
				Debug.Log ("Reassign B");
				goal = node;
				goalDist = Vector3.Distance (node.getRenderLoc (), goalLoc);
			}
		}

		// find the nodes that are impassable
		List<Field> obstacles = getFieldofType (3);
		foreach (Field block in obstacles) {
			foreach (NodeSquare node in nodes) {
				if (PointInOABB (node.getRenderLoc (), block.gameObject.GetComponent<BoxCollider> ())) {
					node.makeImpassable ();
				}
			}
		}

		// calculate the path
		path = a_star (start, goal);
		foreach (NodeSquare node in path) {
			node.makePath ();
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


		//calculate path


	
		move (toMove);


	}

	bool PointInOABB (Vector3 point, BoxCollider box )
	{
		point = box.transform.InverseTransformPoint( point ) - box.center;

		float halfX = (box.size.x * 0.5f);
		float halfY = (box.size.y * 0.5f);
		float halfZ = (box.size.z * 0.5f);
		if( point.x < halfX && point.x > -halfX && 
			point.y < halfY && point.y > -halfY && 
			point.z < halfZ && point.z > -halfZ )
			return true;
		else
			return false;
	}


	public List<NodeSquare> reconstruct_path(Dictionary<NodeSquare, NodeSquare> cameFrom, NodeSquare current){
		List<NodeSquare> total_path = new List<NodeSquare>();
		total_path.Add(current);

		while(cameFrom.ContainsKey(current)){
			current = cameFrom [current];
			total_path.Add (current);
		}

		return total_path;
	}


	public List<NodeSquare> adjacent(NodeSquare current){
		List<NodeSquare> adj = new List<NodeSquare>();

		int x = (int)current.getLoc().x;
		int z = (int)current.getLoc().z;

		int max_x = nodes.GetLength(0);
		int max_z = nodes.GetLength(1);

		if (x - 1 >= 0 && z - 1 >= 0) {
			if(nodes[x-1, z-1].isPassable)
				adj.Add (nodes [x - 1, z - 1]);
		}

		if (z - 1 >= 0) {
			if(nodes[x, z-1].isPassable)
				adj.Add (nodes [x, z - 1]);
		}

		if (x + 1 < max_x && z - 1 >= 0) {
			if(nodes[x+1, z-1].isPassable)
				adj.Add (nodes [x + 1, z - 1]);
		}

		if (x - 1 >= 0) {
			if(nodes[x-1, z].isPassable)
				adj.Add (nodes [x - 1, z]);
		}

		if (x + 1 < max_x) {
			if(nodes[x+1, z].isPassable)
				adj.Add (nodes [x + 1, z]);
		}

		if (x - 1 >= 0 && z + 1 < max_z) {
			if(nodes[x-1, z+1].isPassable)
				adj.Add (nodes [x - 1, z + 1]);
		}

		if (z + 1 < max_z) {
			if(nodes[x, z+1].isPassable)
				adj.Add (nodes [x, z + 1]);
		}

		if (x + 1 < max_x && z + 1 < max_z) {
			if(nodes[x+1, z+1].isPassable)
				adj.Add (nodes [x + 1, z + 1]);
		}
		
		return adj;
	}

	public float heuristic_cost_estimate(NodeSquare start, NodeSquare goal){
		float distance = Mathf.Sqrt (Mathf.Pow (goal.getLoc ().x - start.getLoc ().x, 2) + Mathf.Pow (goal.getLoc ().z - start.getLoc ().z, 2));
		return distance;
	}

	public NodeSquare getLowest(Dictionary<NodeSquare, float> fScore, List<NodeSquare> openSet){

		NodeSquare lowest = openSet [0];

		foreach (NodeSquare key in openSet) {
			if (fScore [key] < fScore [lowest]) {
				lowest = key;
			}
		}
		return lowest;
	}

	public List<NodeSquare> a_star(NodeSquare start, NodeSquare goal){
		
		List<NodeSquare> closedSet = new List<NodeSquare> ();
		List<NodeSquare> openSet = new List<NodeSquare> ();
		openSet.Add (start);
		Dictionary<NodeSquare, NodeSquare> cameFrom = new Dictionary<NodeSquare, NodeSquare> ();

		Dictionary<NodeSquare, float> gScore = new Dictionary<NodeSquare, float> ();
		Dictionary<NodeSquare, float> fScore = new Dictionary<NodeSquare, float> ();

		foreach (NodeSquare node in nodes) {

			gScore.Add (node, float.MaxValue);
			fScore.Add (node, float.MaxValue);
		}

		gScore [start] = 0;
		fScore [start] = heuristic_cost_estimate (start, goal);

		while (openSet.Count > 0) {
			NodeSquare current = getLowest (fScore, openSet);
			current.makeRobot();

			if (current.Equals(goal)) {
				return reconstruct_path (cameFrom, current);
			}
				
			openSet.Remove (current);
			closedSet.Add (current);

			foreach (NodeSquare neighbor in adjacent(current)) {
				if (closedSet.Contains (neighbor)) {
					continue;
				}

				float tentative_gScore = gScore [current] + 1;
				if (!openSet.Contains (neighbor)) {
					openSet.Add (neighbor);
				} else if (tentative_gScore >= gScore [neighbor]) {
					continue;
				}
				cameFrom.Add (neighbor, current);
				gScore [neighbor] = tentative_gScore;
				fScore [neighbor] = gScore [neighbor] + heuristic_cost_estimate (neighbor, goal);
			}
		}
		return null;
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
