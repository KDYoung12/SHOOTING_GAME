using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public int stage;
    public Animator stageAnim;
    public Animator clearAnim;
    public Animator fadeAnim;
    public Transform playerPos;

    public string[] enemyObjs;
    public Transform[] spawnPoints;

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;

    public ObjectManager objectManager;

    // 적 출현에 관련된 변수
    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;
    
    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL" , "EnemyB"};
        StageStart();
    }

    public void StageStart()
    {
        // #. Stage UI Load
        stageAnim.SetTrigger("On");
        stageAnim.GetComponent<Text>().text = "Stage " + stage + "\nStart";
        clearAnim.GetComponent<Text>().text = "Stage " + stage + "\nClear !";

        // #. Enemy Spawn File Read
        ReadSpawnFile();

        // #. Fade In
        fadeAnim.SetTrigger("In");
    }

    public void StageEnd()
    {
        // #. Clear UI Load
        clearAnim.SetTrigger("On");

        // #. Fade Out
        fadeAnim.SetTrigger("Out");

        // #. Player Reposition
        player.transform.position = playerPos.position;

        // #. Stage Increament
        stage++;

        Debug.Log(stage);

        if (stage > 2)
        {
            Debug.Log("게임 종료");
            Invoke("gameOver", 6f);
        }
        else
            Invoke("StageStart", 5f);
    }

    void ReadSpawnFile()
    {
        // #1. 변수 초기화
        // Clear : 리스트를 비우는 함수
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        // #2. 리스폰 파일 읽기
        // TextAsset : 텍스트 파일 에셋 클래스
        TextAsset textFile = Resources.Load("Stage " + stage.ToString()) as TextAsset;
        // StringReader : 파일 내의 문자열 데이터 읽기 클래스
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);

            if (line == null)
                break;
            // 리스폰 데이터 작성 
            Spawn spawnData = new Spawn();
            // Parse : 앞에 붙는 자료형으로 바꾸어주는 함수
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        // #. 텍스트 파일 닫기
        stringReader.Close();

        // #. 첫 번째 스폰 딜레이 적용
        nextSpawnDelay = spawnList[0].delay;
    }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;
        }

        // #. UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        // string.format() : 지정된 양식으로 문자열을 변환해주는 함수
        // {0:n0} : 세 자리씩 숫자를 끊어서 표현해주는 포맷방식
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }

    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }

        // Enemy의 개수 3가지
        int enemyPoint = spawnList[spawnIndex].point;

        // Enemy 생성
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;
        
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        enemyLogic.gameManager = this;
        enemyLogic.objectManager = objectManager;
        if(enemyPoint == 5 || enemyPoint == 6)
        {
            // #. Right Spawn
            enemy.transform.Rotate(Vector3.back * 90); // Z축은 Back(-), forward(+)가 있음
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -0.5f);
        }
        else if (enemyPoint == 7 || enemyPoint == 8)
        {
            // #. Left Spawn
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed, -0.5f);
        }
        else
        {
            // #. Front Spawn
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }

        // #. 리스폰 인덱스 증가
        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        // #. 다음 리스폰 딜레이 갱신
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }
    public void UpdateLifeIcon(int life)
    {
        // #. UI Life Init Disable
        for(int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0); 
        }

        // #. UI Life Active
        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        // #. UI Boom Init Disable
        for (int index = 0; index < 3; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 0);
        }

        // #. UI Boom Active
        for (int index = 0; index < boom; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f; // down = -y
        player.SetActive(true);

        // 한 번에 총알을 두 번 맞지 않게 
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void CallExplosion(Vector3 position, string type)
    {
        GameObject explosion = objectManager.MakeObj("Explosion");
        Explosion explosionLogic = explosion.GetComponent<Explosion>();

        explosion.transform.position = position;
        explosionLogic.StartExplosion(type);
    }

    public void gameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void gameRetry()
    {
        SceneManager.LoadScene(0);
    }
}

