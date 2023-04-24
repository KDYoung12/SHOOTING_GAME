using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
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

    // �� ������ ���õ� ����
    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;
    
    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL" , "EnemyB"};
        ReadSpawnFile();
    }

    void ReadSpawnFile()
    {
        // #1. ���� �ʱ�ȭ
        // Clear : ����Ʈ�� ���� �Լ�
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        // #2. ������ ���� �б�
        // TextAsset : �ؽ�Ʈ ���� ���� Ŭ����
        TextAsset textFile = Resources.Load("Stage 0") as TextAsset;
        // StringReader : ���� ���� ���ڿ� ������ �б� Ŭ����
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);

            if (line == null)
                break;
            // ������ ������ �ۼ� 
            Spawn spawnData = new Spawn();
            // Parse : �տ� �ٴ� �ڷ������� �ٲپ��ִ� �Լ�
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        // #. �ؽ�Ʈ ���� �ݱ�
        stringReader.Close();

        // #. ù ��° ���� ������ ����
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
        // string.format() : ������ ������� ���ڿ��� ��ȯ���ִ� �Լ�
        // {0:n0} : �� �ڸ��� ���ڸ� ��� ǥ�����ִ� ���˹��
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
        // Enemy�� ���� 3����
        int enemyPoint = spawnList[spawnIndex].point;
        // Enemy ����
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;
        
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        enemyLogic.objectManager = objectManager;
        if(enemyPoint == 5 || enemyPoint == 6)
        {
            // #. Right Spawn
            enemy.transform.Rotate(Vector3.back * 90); // Z���� Back(-), forward(+)�� ����
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

        // #. ������ �ε��� ����
        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        // #. ���� ������ ������ ����
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

        // �� ���� �Ѿ��� �� �� ���� �ʰ� 
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
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
