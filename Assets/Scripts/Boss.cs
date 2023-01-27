using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortL;
    public Transform missilePortR;
    public bool isLook;

    Vector3 lookVec;
    Vector3 tauntVec;
    

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        navMeshAgent.isStopped = true;
        StartCoroutine(Think());
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else
        {
            navMeshAgent.SetDestination(tauntVec);
        }
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");

        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileL = Instantiate(missile, missilePortL.position, missilePortL.rotation);
        BossMissile bossMissileL = instantMissileL.GetComponent<BossMissile>();
        bossMissileL.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileR = Instantiate(missile, missilePortR.position, missilePortR.rotation);
        BossMissile bossMissileR = instantMissileR.GetComponent<BossMissile>();
        bossMissileR.target = target;


        GameObject instantBullet = Instantiate(missile, missilePortR.position, missilePortR.rotation);
        Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
        rigidBullet.velocity = transform.forward * 20;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);

        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        navMeshAgent.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        navMeshAgent.isStopped = true;
        boxCollider.enabled = true;

        StartCoroutine(Think());
    }
}
