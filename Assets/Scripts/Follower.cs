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
        // Queue = 먼저 입력된 데이터가 먼저 나가는 자료구조 FIFO (First Input First Out)
        // Queue(FIFO)의 반대 Stack(LIFO)
        // Enqueue() : 큐에 데이터 저장하는 함수
        // #.Input Pos
        if(!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);

        // Dequeue() : 큐의 첫 데이터를 빼면서 반환하는 함수
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
        // 총알 발사 함수
        if (!Input.GetKey(KeyCode.Space))
            return; // Fire1 을 누르지 않으면 총알이 안나감.

        if (curShotDelay < maxShotDelay)
        {
            // 아직 장전이 되지않았더라면
            return;
        }

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        curShotDelay = 0; // 한 발을 쐈다면 다시 0으로 만들어서 쿨타임이 흐르게 한다.
    }
    void Reload()
    {
        // 재장전 함수
        curShotDelay += Time.deltaTime;

    }
}
