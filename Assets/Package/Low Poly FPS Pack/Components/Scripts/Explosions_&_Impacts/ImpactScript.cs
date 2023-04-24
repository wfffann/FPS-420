using UnityEngine;
using System.Collections;

public class ImpactScript : MonoBehaviour {

	[Header("Impact Despawn Timer")]
	//How long before the impact is destroyed
	public float despawnTimer = 1f;

	

	private void Start () {
		// Start the despawn timer
		StartCoroutine (DespawnTimer ());
	}
	
	private IEnumerator DespawnTimer() {
		//Wait for set amount of time
		yield return new WaitForSeconds (despawnTimer);
		//Destroy the impact gameobject
		Destroy (gameObject);
	}
}