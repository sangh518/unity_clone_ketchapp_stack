using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundColorManager : MonoBehaviour
{

    RawImage backgroundImage;
    Texture2D texture;

    Color preColor1, preColor2;
   

    private void Awake()
    {
        backgroundImage = GetComponent<RawImage>();
        backgroundImage.color = Color.white;
        texture = new Texture2D(1, 2);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

    }

    public void InitColor(Color color1, Color color2)
    {
        preColor1 = color1;
        preColor2 = color2;

        texture.SetPixels(new Color[] { color1, color2 });
        texture.Apply();
        backgroundImage.texture = texture;
        Debug.Log("init");

    }

    public void SetColor(Color color1, Color color2)
    {
        if (backgroundImage == null) return;


        LeanTween.value(0f, 1f, GameManager.instance.gameData.camMoveTime).setOnUpdate(
            (float value) => 
            {
                var nColor1 = Color.Lerp(preColor1, color1, value);
                var nColor2 = Color.Lerp(preColor2, color2, value);

                texture.SetPixels(new Color[] { nColor1, nColor2 });
                texture.Apply();
                backgroundImage.texture = texture;
           
            }).setOnComplete(
            ()=>
            {
                preColor1 = color1;
                preColor2 = color2;
            }
            ).setEaseOutSine(); ;


    }

    
}
