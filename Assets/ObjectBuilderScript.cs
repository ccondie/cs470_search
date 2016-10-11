using UnityEngine;

using System.Collections;

public class ObjectBuilderScript : MonoBehaviour 
{

	bool beenBuilt;
	//Your grid stuff
	public static NodeSquare[,] nodes;

	//this gets called when you press the button in the unity inspector
	public void BuildObject()	{
		beenBuilt = true;
		//this is where you initialize all of your nodes/grid
		// find the ground space to gen the nodes on
		Ground ground = GameObject.FindObjectOfType<Ground>();
		Vector3 ground_pos = ground.transform.position;
		Vector3 ground_scale = ground.transform.localScale;

		Debug.Log (ground_scale.z);

		nodes = new NodeSquare[(int)(ground_scale.x), (int)(ground_scale.z)];
		float start_x = -(ground_scale.x / 2) + (float)(.5);
		float start_z = -(ground_scale.z / 2) + (float)(.5);

		for (int z = 0; z < (int)ground_scale.z; z += 1) {
			for(int x = 0; x < (int)ground_scale.x; x += 1){
				nodes [x, z] = new NodeSquare (new Vector3 (start_x + x, 0.7F, start_z + z), new Vector3(x, 0, z));
			}
		}

		Debug.Log ("Completed BuildObject");
	}


	void OnDrawGizmos()
	{
		if (beenBuilt) {

			// Do a double for loop and draw all of your nodes in your grid

			//Gizmos.DrawWireSphere (nodes[i, j].position, .5f);
			for (int z = 0; z < nodes.GetLength(1); z++) {
				for (int x = 0; x < nodes.GetLength (0); x++) {
					if (!nodes [x, z].isPassable) {
						Gizmos.color = new Color (0.0F, 0.0F, 0.0F, 0.7F);
					}
					else if(nodes [x, z].isPath){
						Gizmos.color = new Color (0.0F, 0.0F, 1.0F, 0.3F);
					}
					else if (nodes [x, z].isRobot) {
						Gizmos.color = new Color (1.0F, 0.0F, 0.0F, 0.3F);
					} 
					else {
						Gizmos.color = new Color (1.0F, 0.0F, 0.0F, 0.0F);
					}
					Gizmos.DrawCube (nodes [x, z].getRenderLoc (), new Vector3 (1, 0.3F, 1));

					Gizmos.color = new Color (0.0F, 0.0F, 0.0F, 0.05F);
					Gizmos.DrawWireCube (nodes [x, z].getRenderLoc (), new Vector3 (1, 0.3F, 1));
				}
			}

		}
	}
}