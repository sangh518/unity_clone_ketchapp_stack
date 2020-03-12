using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectManager : MonoBehaviour
{
    public static ParticleEffectManager instance;

    public GameObject prefab;

    Stack<GameObject> pool;

    private void Awake()
    {
        instance = this;
        pool = new Stack<GameObject>();
    }

    public void PlayEffect(GameObject target)
    {
        StartCoroutine(OnEffect(target));
    }

    IEnumerator OnEffect(GameObject target)
    {
        var particle = PopFromPool().GetComponent<ParticleSystem>();
        particle.Stop();
        particle.transform.position = target.transform.position - new Vector3(0, 0.5f, 0f);
        particle.Play();

        while (particle.isPlaying) yield return null;


        PushToPool(particle.gameObject);
    }

    GameObject CreateEffect()
    {
        return Instantiate(prefab);
    }

    void PushToPool(GameObject obj)
    {
        pool.Push(obj);
        obj.SetActive(false);
    }

    GameObject PopFromPool()
    {
        GameObject obj;

        if(pool.Count == 0)
        {
            obj = CreateEffect();
        }
        else
        {
            obj = pool.Pop();
        }

        obj.SetActive(true);
        return obj;
    }
}
