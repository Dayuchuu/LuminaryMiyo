using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIParticleTrigger : MonoBehaviour
{
    [Header("Particle")]
    public GameObject triggerParticle;

    // Display particle on Button click
    public void PlayParticleOnButtonClick()
    {
        // enable particle
        triggerParticle.SetActive(true);

        // play particle
        triggerParticle.GetComponent<UIParticleSystem>().Play();

    }
}
