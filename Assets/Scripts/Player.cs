using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public int score;
    public int life;
    public int power;
    public int maxPower;
    public int boom;
    public int maxBoom;
    public float speed;
    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;


    public GameManager gameManager;
    public ObjectManager objectManager;
    public bool isHit;
    public bool isBoomTime;

    public GameObject[] followers;
    public bool isRespawnTime;

    Animator anim;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        Unbeatable();
        Invoke("Unbeatable", 3);
    }

    void Unbeatable()
    {
        isRespawnTime = !isRespawnTime;
        if (isRespawnTime) // ���� Ÿ�� ����Ʈ (����)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f); 

            for (int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        else // ���� Ÿ�� ���� (�������)
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            for (int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    void Move()
    {
        // �÷��̾ �����̴� �Լ�
        float h = Input.GetAxisRaw("Horizontal"); // ����
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
        {
            h = 0;
        }

        float v = Input.GetAxisRaw("Vertical"); // ����
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
        {
            v = 0;
        }
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos; // ���� ��ġ�� ���� ��ġ�� �����ֱ�

        // Animation Int ����
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
            // Get �������� ��
            // Set �����ϴ� ��
        }
    }

    void Fire()
    {
        // �Ѿ� �߻� �Լ�
        if (!Input.GetKey(KeyCode.Space))
            return; // Fire1 �� ������ ������ �Ѿ��� �ȳ���.

        if(curShotDelay < maxShotDelay)
        {
            // ���� ������ �����ʾҴ����
            return;
        }

        switch (power)
        {
            case 1: // Power One
                // Instantiate() : �Ű����� ������Ʈ�� �����ϴ� �Լ�
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;

                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2: // Power One
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            default:
                GameObject bulletCC1 = objectManager.MakeObj("BulletPlayerB");
                bulletCC1.transform.position = transform.position + Vector3.right * 0.2f;
                GameObject bulletCC2 = objectManager.MakeObj("BulletPlayerB");
                bulletCC2.transform.position = transform.position + Vector3.left * 0.2f;
                Rigidbody2D rigidCC1 = bulletCC1.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC2 = bulletCC2.GetComponent<Rigidbody2D>();
                rigidCC1.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC2.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }
        

        curShotDelay = 0; // �� ���� ���ٸ� �ٽ� 0���� ���� ��Ÿ���� �帣�� �Ѵ�.
    }

    void Reload()
    {
        // ������ �Լ�
        curShotDelay += Time.deltaTime;

    }

    void Boom()
    {
        if (!Input.GetButton("Fire2"))
        {
            return;
        }
        if (isBoomTime)
        {
            return;
        }

        if(boom == 0)
        {
            return;
        }

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);

        // #1. Effect visible
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 1.5f);

        // #2. Remove Enemy
        // FindGameObjectsWithTag : �±׷� ����� ��� ������Ʈ�� ����
        GameObject[] enemieL = objectManager.GetPool("EnemyL");
        GameObject[] enemieM = objectManager.GetPool("EnemyM");
        GameObject[] enemieS = objectManager.GetPool("EnemyS");
        for (int index = 0; index < enemieL.Length; index++)
        {
            if (enemieL[index].activeSelf)
            {
                Enemy enemyLogic = enemieL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemieM.Length; index++)
        {
            if (enemieM[index].activeSelf)
            {
                Enemy enemyLogic = enemieM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemieS.Length; index++)
        {
            if (enemieS[index].activeSelf)
            {
                Enemy enemyLogic = enemieS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        // #3. Remove Enemy Bullet
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
        for (int index = 0; index < bulletsA.Length; index++)
        {
            if (bulletsA[index].activeSelf)
            {
                bulletsA[index].SetActive(false);
            }
        }
        for (int index = 0; index < bulletsB.Length; index++)
        {
            if (bulletsB[index].activeSelf)
            {
                bulletsB[index].SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �� ����
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if (collision.gameObject.tag == "EnemyBullet" || collision.gameObject.tag == "Enemy")
        {
            if (isRespawnTime)
                return;

            if (isHit)
                return;

            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");

            if (life == 0)
            {
                gameManager.gameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }

            gameObject.SetActive(false);

        }

        else if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if(power == maxPower)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower();
                    }
                    break;
                case "Boom":
                    if (boom == maxBoom)
                        score += 500;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                        
                    break;
            }
            collision.gameObject.SetActive(false);
        }
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    void AddFollower()
    {
        if (power == 5)
            followers[0].SetActive(true);
        else if (power == 6)
            followers[1].SetActive(true);
        else if (power == 7)
            followers[2].SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // �� ����
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}
