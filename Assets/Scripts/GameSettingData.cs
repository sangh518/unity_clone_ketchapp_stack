using UnityEngine;
using System;

[CreateAssetMenu(fileName ="Block Spawn Data", menuName = "Data/Block Spawn Data")]
public class GameSettingData : ScriptableObject
{
    //초기 게임 세팅값

    public Vector3 initialBlockScale;
    public float blockMoveTime;
    public float blockSpawnDistance;
    public float minDistance;
    public float camMoveTime;
    public Color[] colorPalette;
    public float deltaColor;

    public float blockOffsetHeight = 3.95f;
    public float endingSpeed = 20f;

    public int perfectCondition = 8;
    public float perfectScale = 0.5f;

}
