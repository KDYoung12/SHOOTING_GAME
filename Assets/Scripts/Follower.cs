using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;

    public ObjectManager objectManager;

    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;

    void Awake()
    {
        parentPos = new Queue<Vector3>();    
    }
    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    void Watch()
    {
        // Queue = ���� �Էµ� �����Ͱ� ���� ������ �ڷᱸ�� FIFO (First Input First Out)
        // Queue(FIFO)�� �ݴ� Stack(LIFO)
        // Enqueue() : ť�� ������ �����ϴ� �Լ�
        // #.Input Pos
        if(!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);

        // Dequeue() : ť�� ù �����͸� ���鼭 ��ȯ�ϴ� �Լ�
        // #.Output Pos
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay)
            followPos = parent.position;
    }

    void Follow()
    {
        transform.position = followPos;
    }
    void Fire()
    {
        // �Ѿ� �߻� �Լ�
        if (!Input.GetKey(KeyCode.Space))
            return; // Fire1 �� ������ ������ �Ѿ��� �ȳ���.

        if (curShotDelay < maxShotDelay)
        {
            // ���� ������ �����ʾҴ����
            return;
        }

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        curShotDelay = 0; // �� ���� ���ٸ� �ٽ� 0���� ���� ��Ÿ���� �帣�� �Ѵ�.
    }
    void Reload()
    {
        // ������ �Լ�
        curShotDelay += Time.deltaTime;

    }
}
