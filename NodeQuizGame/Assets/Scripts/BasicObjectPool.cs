using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicObjectPool : MonoBehaviour {
	public GameObject prefab;
	private Stack<GameObject> inactiveInstances = new Stack<GameObject>();

	//takes object form pool
	public GameObject GetObject(){
		GameObject spawnedObject;
		if (inactiveInstances.Count > 0) {
			spawnedObject = inactiveInstances.Pop ();
		} 
		else {
			spawnedObject = Instantiate (prefab);

			PooledObject pooledObject = spawnedObject.AddComponent<PooledObject> ();
			pooledObject.pool = this;

		}

		spawnedObject.SetActive (true);
		return spawnedObject;
	}

	//Returns Object to pool
	public void ReturnObject(GameObject toReturn){
		PooledObject pooledObject = toReturn.GetComponent<PooledObject> ();

		if (pooledObject != null && pooledObject.pool == this) {
			toReturn.SetActive (false);
			inactiveInstances.Push (toReturn);
		} 
		else {
			Destroy (toReturn);
		}
	}
}

public class PooledObject:MonoBehaviour{
	public BasicObjectPool pool;
}
