using UnityEngine;

using System.Collections;

public class ObjectBuilderScript : MonoBehaviour 
{


	bool beenBuilt;
	//Your grid stuff
	NodeSquare[,] nodes;




	//this gets called when you press the button in the unity inspector
	public void BuildObject()	{
		beenBuilt = true;
		//this is where you initialize all of your nodes/grid
		// find the ground space to gen the nodes on
		Ground ground = GameObject.FindObjectOfType<Ground>();
		Vector3 ground_pos = ground.transform.position;
		Vector3 ground_scale = ground.transform.localScale;

		nodes = new NodeSquare[(int)ground_scale.x, (int)ground_scale.y];
		for (int y = 0; y < (int)ground_scale.y; y++) {
			for(int x = 0; x < (int)ground_scale.x; x++){
				nodes [x, y] = new NodeSquare (new Vector3 (x, 0, y));
			}
		}
	}


	void OnDrawGizmos()
	{
		if (beenBuilt) {

			// Do a double for loop and draw all of your nodes in your grid

			//Gizmos.DrawWireSphere (nodes[i, j].position, .5f);
			for (int y = 0; y < nodes.GetLength(1); y++) {
				for (int x = 0; x < nodes.GetLength (0); x++) {
					Gizmos.DrawWireCube (nodes [x, y].getLoc (), new Vector3 (1, 1, 1));
				}
			}

		}
	}
}