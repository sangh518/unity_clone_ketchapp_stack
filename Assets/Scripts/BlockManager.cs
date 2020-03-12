using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    
    enum Dircetion { x, z }
    enum BlockState { perfect, good, fail}

    GameSettingData gameData;
    public BackgroundColorManager background;

    GameObject currentBlock, preBlock;
    Dircetion currentDirection = Dircetion.x;
    Vector3 currentBlockScale;


    int currentBlockCount;
    float randomColorOffset;
    float randomBgColorOffset;
    Vector3 camOffset;

    GameObject firstBlock;

    bool isTouched = false;
    bool enableTouch = true;

    int perfectCount;


    private void Start()
    {
        InitGame();
        StartCoroutine(OnUpdate());
    }



    void OnTouch()
    {
        if(enableTouch) isTouched = true;
    }


    void InitGame()
    {

        gameData = GameManager.instance.gameData;
        GameManager.instance.OnTouch += OnTouch;
        
        currentBlockScale = gameData.initialBlockScale;
        currentBlockCount = 0;
        randomColorOffset = Random.Range(0f, gameData.colorPalette.Length - 1f);
        randomBgColorOffset = Random.Range(0f, gameData.colorPalette.Length - 1f);

        background.InitColor(GetCurrentColor(randomBgColorOffset), GetCurrentColor(randomBgColorOffset + 1));
        currentBlock= preBlock = SpawnBlock(currentBlockScale, new Vector3(0f, -1f, 0f), GetCurrentColor(randomColorOffset));
        firstBlock = currentBlock;

        camOffset = Camera.main.gameObject.transform.position;
  
    }

    IEnumerator OnUpdate()
    {

        while (GameManager.instance.isGameOver == false)
        {

            var spawnPos = new Vector3();
            var movePos = new Vector3();

            if (currentDirection == Dircetion.x)
            {
                spawnPos.x = -gameData.blockSpawnDistance + preBlock.transform.position.x;
                spawnPos.y = currentBlockCount;
                spawnPos.z = preBlock.transform.position.z;

                movePos = spawnPos;
                movePos.x = gameData.blockSpawnDistance + preBlock.transform.position.x;
            }
            else
            {
                spawnPos.x = preBlock.transform.position.x;
                spawnPos.y = currentBlockCount;
                spawnPos.z = -gameData.blockSpawnDistance + preBlock.transform.position.z;

                movePos = spawnPos;
                movePos.z = gameData.blockSpawnDistance + preBlock.transform.position.z;
            }

            background.SetColor(GetCurrentColor(randomBgColorOffset), GetCurrentColor(randomBgColorOffset + 1));
            currentBlock = SpawnBlock(currentBlockScale, spawnPos, GetCurrentColor(randomColorOffset));
            GameManager.instance.scoreText.text = (currentBlockCount).ToString();
            currentBlockCount++;
            int loopTweenId = LeanTween.move(currentBlock, movePos, gameData.blockMoveTime).setLoopPingPong().uniqueId;

            do { yield return null; }
            while (!isTouched);

            isTouched = false;
            LeanTween.cancel(loopTweenId);

            var tempCurrentPos = currentBlock.transform.position;
            tempCurrentPos.y = preBlock.transform.position.y;


            switch(CheckState(currentBlock, preBlock))
            {
                case BlockState.perfect:
                    OnBlockPerfect();
                    break;
                case BlockState.good:
                    OnBlockGood();
                    break;
                case BlockState.fail:
                    OnGameOver();
                    yield break;
            }

            MoveCam();
            currentDirection = (currentDirection == Dircetion.x)? Dircetion.z : Dircetion.x;

            yield return null;
        }
    }

    void OnBlockGood()
    {
        perfectCount = 0;
        currentBlock = CutBlock(currentBlock, preBlock.transform.position);
        currentBlockScale = currentBlock.transform.localScale;
        preBlock = currentBlock;
    }
    void OnBlockPerfect()
    {
        perfectCount++;
        currentBlock.transform.position = new Vector3(preBlock.transform.position.x, currentBlock.transform.position.y, preBlock.transform.position.z);

        WaveEffectManager.instance.PlayEffect(currentBlock, perfectCount);


        if (perfectCount >= gameData.perfectCondition)
        {
            Vector3 targetBlockScale = currentBlock.transform.localScale;
            bool blockScaleDirection;

            if (currentDirection == Dircetion.x)
            {
                if (currentBlock.transform.localScale.x + gameData.perfectScale > gameData.initialBlockScale.x)
                {
                    targetBlockScale.x = gameData.initialBlockScale.x;
                }
                else targetBlockScale.x = currentBlock.transform.localScale.x + gameData.perfectScale;

                blockScaleDirection = currentBlock.transform.position.x > 0;
             
            }
            else
            {
                if (currentBlock.transform.localScale.z + gameData.perfectScale > gameData.initialBlockScale.z)
                {
                    targetBlockScale.z = gameData.initialBlockScale.z;
                }
                else targetBlockScale.z = currentBlock.transform.localScale.z + gameData.perfectScale;

                blockScaleDirection = currentBlock.transform.position.z > 0;
            }
           
           

            var preBlockScale = currentBlockScale;
            var preBlockPos = currentBlock.transform.position;
            var targetBlockPos = preBlockPos + (currentBlockScale - targetBlockScale) / 2f *( blockScaleDirection? 1 : -1 );


            var temObj = new GameObject();
            temObj.transform.position = targetBlockPos;
            temObj.transform.localScale = targetBlockScale;
            preBlock = temObj;

            var animationBlock = currentBlock;

            currentBlockScale = targetBlockScale;
            

            LeanTween.value(0f, 1f, gameData.camMoveTime).setOnUpdate((float value) =>
            {
                animationBlock.transform.localScale = Vector3.Lerp(preBlockScale, targetBlockScale, value);
                animationBlock.transform.position = Vector3.Lerp(preBlockPos, targetBlockPos, value);
            }).setOnComplete(()=> {
                Destroy(temObj);
                preBlock = animationBlock;
                    });
        }
    }
    
    void OnGameOver()
    {
        currentBlock.AddComponent<Rigidbody>();
        GameManager.instance.isGameOver = true;

        StartCoroutine(ShowEntireBlock());

    }

    void MoveCam()
    {
        
        var pos = currentBlock.transform.position;
        var posValue = (pos.x + pos.z) / gameData.initialBlockScale.x;


        enableTouch = false;
        LeanTween.moveY(Camera.main.gameObject, camOffset.y + currentBlockCount - posValue * gameData.blockOffsetHeight, gameData.camMoveTime).setEaseOutSine().setOnComplete(()=> { enableTouch = true; });
    }

    IEnumerator ShowEntireBlock()
    {
        var mesh = firstBlock.GetComponent<Renderer>();

        float speed = gameData.endingSpeed;
        while (!mesh.isVisible)
        {
            Camera.main.orthographicSize += speed * Time.deltaTime;
            speed += speed * Time.deltaTime;
            yield return null;
        }

    }

    GameObject SpawnBlock(Vector3 scale, Vector3 position, Color color)
    {
        var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.transform.localScale = scale;
        block.transform.position = position;
        block.transform.parent = transform;
        block.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", color);

        return block;
    }
    
    GameObject CutBlock(GameObject block, Vector3 targetPos)
    {

        GameObject cutBlock = Instantiate(block, transform);
        
        

        Vector3 blockPos = block.transform.position;
        Vector3 originalScale = block.transform.localScale;

        float distance = Vector2.Distance(new Vector2(blockPos.x, blockPos.z), new Vector2(targetPos.x, targetPos.z));
        
        if(currentDirection == Dircetion.x)
        {

            block.transform.position = new Vector3( (targetPos.x + blockPos.x) / 2f, blockPos.y, blockPos.z);
            block.transform.localScale = new Vector3(originalScale.x - distance, originalScale.y, originalScale.z);

            float factor = blockPos.x - targetPos.x > 0 ? 1 : -1;
            cutBlock.transform.position = new Vector3((targetPos.x + blockPos.x) / 2f + originalScale.x * factor / 2f, blockPos.y, blockPos.z);
            cutBlock.transform.localScale = new Vector3(distance, originalScale.y, originalScale.z);
        }
        else
        {
            block.transform.position = new Vector3(blockPos.x, blockPos.y, (targetPos.z + blockPos.z) / 2f);
            block.transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z - distance);

            float factor = blockPos.z - targetPos.z > 0 ? 1 : -1;
            cutBlock.transform.position = new Vector3(blockPos.x, blockPos.y, (targetPos.z + blockPos.z) / 2f + originalScale.z * factor / 2f);
            cutBlock.transform.localScale = new Vector3(originalScale.x, originalScale.y, distance);
        }

        cutBlock.AddComponent<Rigidbody>().mass = 100f;
        StartCoroutine(CheckBlockFall(cutBlock));

        return block;

    }
    
    BlockState CheckState(GameObject block, GameObject target)
    {


        float dis = currentDirection == Dircetion.x ? 
            Mathf.Abs(block.transform.position.x - target.transform.position.x) 
            : Mathf.Abs(block.transform.position.z - target.transform.position.z);

        if (dis < gameData.minDistance) return BlockState.perfect;
        else
        {
            float scale = currentDirection == Dircetion.x ? block.transform.localScale.x : block.transform.localScale.z;

            if (scale > dis)
            {
                return BlockState.good;
            }
            else return BlockState.fail;

        }
    }

    IEnumerator CheckBlockFall(GameObject block)
    {
        yield return null;

        var renderer = block.GetComponent<Renderer>();

        while (renderer.isVisible) yield return null;
        Destroy(block);
    }


    Color GetCurrentColor(float offset)
    {

        float colorID = ((currentBlockCount+ gameData.colorPalette.Length) * gameData.deltaColor + offset) % gameData.colorPalette.Length;
        int colorI1 = (int)colorID;
        int colorI2 = (colorI1 + 1) % gameData.colorPalette.Length;
        float middleValue = colorID - colorI1;
        return Color.Lerp(gameData.colorPalette[colorI1], gameData.colorPalette[colorI2], middleValue);
    }
}
