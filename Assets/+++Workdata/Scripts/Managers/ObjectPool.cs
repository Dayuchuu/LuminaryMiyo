using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	#region Variables
	
	public static ObjectPool sharedInstance = null;
	public List<GameObject> pooledObjects = new List<GameObject>();
	public GameObject objectToPool = null;
	public Transform pooledObjectParent = null;
	public int amountToPool = 0;
	
	#endregion

	#region Methods
	
	private void Awake()
	{
		if (sharedInstance == null)
		{ 
			sharedInstance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		pooledObjects = new List<GameObject>();
		GameObject temp = new GameObject();
		for (int poolIndex = 0; poolIndex < amountToPool; poolIndex++)
		{
			temp = Instantiate(objectToPool, pooledObjectParent);
			temp.SetActive(false);
			pooledObjects.Add(temp);
		}
	}

	public GameObject GetPooledObject()
	{
		for (int pooledIndex = 0; pooledIndex < amountToPool; pooledIndex++)
		{
			if (!pooledObjects[pooledIndex].activeInHierarchy)
			{
				return pooledObjects[pooledIndex];
			}
		}
		return null;
	}
	#endregion
}
