using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIParticleSpawn : MonoBehaviour
{

    [Header("Particle")]

    public GameObject spawnParticlePrefab;


    [Header("Location")]

    public Transform spawnParticlePrefabLocation;
    GameObject newParticle;


    // sporn particle prefab on location
    public void SpornparticleOnLocationOnButtonclick()
    {
        // spawn Particles under parent game object in canvas
        newParticle = Instantiate(spawnParticlePrefab, new Vector3(0, 0, 0), Quaternion.identity, spawnParticlePrefabLocation);


        // put them on (0,0,0) under parent
        newParticle.transform.localPosition = Vector3.zero;

        // set active
        newParticle.SetActive(true);


        // start play function on Partical prefab in script
        newParticle.GetComponent<UIParticleSystem>().Play();
    }




    // destroy particle prefab
    public void DestroyParticle()
    {
        Destroy(newParticle);
    }



}

