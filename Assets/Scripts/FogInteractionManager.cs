using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogInteractionManager : MonoBehaviour
{
    public ParticleSystem particles;
    public List<ParticleCollisionEvent> collisionEvents;

    public GameObject UIObject;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();

        UIObject.SetActive(false);
    }

    void OnParticleCollision(GameObject other)
    {
        UIObject.SetActive(true);
        StartCoroutine("FadeMessage");
    }

    IEnumerator FadeMessage()
    {
        yield return new WaitForSeconds(3);
        UIObject.SetActive(false);
    }
}