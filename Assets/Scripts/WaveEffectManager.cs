using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WaveEffectManager : MonoBehaviour
{

    public static WaveEffectManager instance;

    public Material waveEffectMaterial;

    readonly int preCreatedEffects = 5;

    readonly float effectWidth = 0.3f;


    readonly float effectDuration = 0.8f;

    readonly float multipleEffectWidth = 2f;
    readonly int multipleEffectCondition = 3;

    Stack<GameObject> pool;

    private void Awake()
    {
        instance = this;
        pool = new Stack<GameObject>();
        for(int i = 0; i < preCreatedEffects; i++)
        {
            PushToPool(CreateEffect());
        }
    }


    public void PlayEffect(GameObject target, int effectCount)
    {


        StartCoroutine(OnEffect(target, effectCount));


    }
    IEnumerator OnEffect(GameObject target, int effectCount)
    {
        

        
        var defaultEffect = PopFromPool();
        defaultEffect.transform.position = target.transform.position - new Vector3(0, 0.5f, 0f);
        var targetScale = target.transform.localScale;
        defaultEffect.transform.localScale = new Vector3(effectWidth*2 + targetScale.x, effectWidth*2 + targetScale.z, 1f);

        var mat = defaultEffect.GetComponent<Renderer>().material;
        //_Opacity, _Margin, _LineWidth, _Color, _QaudScale

        mat.SetColor("_Color", Color.white);
        mat.SetVector("_QuadScale", new Vector4(defaultEffect.transform.localScale.x, defaultEffect.transform.localScale.y, 0, 0));
        mat.SetFloat("_Margin", 0f);
        mat.SetFloat("_LineWidth", effectWidth*2);

        LeanTween.value(1f, 0f, effectDuration).setOnUpdate((float value) => { mat.SetFloat("_Opacity", Mathf.Clamp(value * 2, 0f, 1f)); }).setOnComplete(() => { PushToPool(defaultEffect); }).setEaseOutQuad();

        if (effectCount < multipleEffectCondition) yield break;

        
        if (effectCount <= GameManager.instance.gameData.perfectCondition)
        {
            var multiEffectCount = effectCount - multipleEffectCondition + 1;

            for (int i = 1; i <= multiEffectCount; i++)
            {
                var multiEffect = PopFromPool();
                multiEffect.name = i.ToString();
                multiEffect.transform.position = defaultEffect.transform.position;
                multiEffect.transform.localScale = new Vector3(multipleEffectWidth * 2 + targetScale.x, multipleEffectWidth * 2 + targetScale.z, 1f);

                var multiMat = multiEffect.GetComponent<Renderer>().material;
                multiMat.SetColor("_Color", Color.white);
                multiMat.SetVector("_QuadScale", new Vector4( multiEffect.transform.localScale.x, multiEffect.transform.localScale.y, 0,0));
                multiMat.SetFloat("_LineWidth", (multiEffectCount - i + 1f) / multiEffectCount * effectWidth);

                var leftTime = (multiEffectCount - i + 1f) / multiEffectCount * effectDuration;

                LeanTween.value(1f, 0f, leftTime).setOnUpdate((float value) =>
                {
                    multiMat.SetFloat("_Opacity", Mathf.Clamp(value * 2, 0f, 1f));
                    multiMat.SetFloat("_Margin", multipleEffectWidth * value);
                    

                })
                .setOnComplete(() => { PushToPool(multiEffect); }).setEaseOutQuad();

                yield return new WaitForSeconds(1f / multiEffectCount * effectDuration);
            }
        }
        else
        {
            ParticleEffectManager.instance.PlayEffect(target);
        }



        yield return null;
    }

    

    void PushToPool(GameObject obj)
    {
        pool.Push(obj);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }

    GameObject PopFromPool()
    {
        GameObject obj;

        if(pool.Count == 0)
        {
            obj = CreateEffect();
        }
        else obj = pool.Pop();

        obj.SetActive(true);
        return obj;
    }

    GameObject CreateEffect()
    {

        var effect = GameObject.CreatePrimitive(PrimitiveType.Quad);
        var renderer = effect.GetComponent<Renderer>();
        renderer.material = waveEffectMaterial;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        effect.transform.rotation = Quaternion.Euler(90, 0f, 0f);

        return effect;
    }
}
