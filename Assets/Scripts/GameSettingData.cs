using UnityEngine;
using System;

[CreateAssetMenu(fileName ="Block Spawn Data", menuName = "Data/Block Spawn Data")]
public class GameSettingData : ScriptableObject, ISerializationCallbackReceiver
{
    //초기 게임 세팅값

    public Vector3 initialBlockScale;
    public float blockMoveTime;
    public float blockSpawnDistance;
    public float minDistance;
    public float camMoveTime;
    public Color[] colorPalette;
    public float deltaColor;


    //변수


    



    public void OnAfterDeserialize()
    {

    }

    public void OnBeforeSerialize(){}
}
