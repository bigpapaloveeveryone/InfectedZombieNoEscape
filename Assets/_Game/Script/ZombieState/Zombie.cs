using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Zombie : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    [SerializeField] private int takeDamage;
    [SerializeField] private float detectedRange;
    [SerializeField] private float attackRange;
    [SerializeField] private ZombieHand[] hand;
    [SerializeField] private GameObject BloodEff;

    public IState<Zombie> currentState;
    public IdleState idleState;
    public ChaseState chaseState;
    public AttackState attackState;

    private Animator anim;
    private bool flag;

    [HideInInspector] public bool isDed;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public AudioSource audioSource;

    private void Start()
    {
        if (flag)
            return;

        audioSource = GetComponent<AudioSource>(); // Lấy AudioSource từ zombie
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Nếu chưa có thì thêm vào
        }
        anim = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        flag = true;

        // Initialize states
        idleState = new IdleState();
        chaseState = new ChaseState();
        attackState = new AttackState();

        // Start in Idle state
        TransitionToState(idleState);
    }

    private void Update()
    {
        if (isDed)
        {
            foreach (ZombieHand h in hand)
            {
                h.gameObject.SetActive(false);
            }
            return;
        }

        DetectedPlayer();
        AbleToAttack();

        currentState?.OnExecute(this);
    }

    public void TransitionToState(IState<Zombie> newState)
    {
        currentState?.OnExit(this);
        currentState = newState;
        currentState?.OnEnter(this);
    }

    public void TurnOffAudioSource()
    {
        audioSource.enabled = false;
    }

    public void PlayAnim(string name)
    {
        anim.Play(name);
    }

    public void ChasePlayer()
    {
        transform.GetChild(0).LookAt(GameManager.Ins.player.transform.position);

        MoveToPos(GameManager.Ins.player.transform.position);
    }

    public void MoveToPos(Vector3 pos)
    {
        navMeshAgent.SetDestination(pos);
    }

    public void Attack()
    {
        //Debug.Log("Attacking");
    }

    public IEnumerator TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            audioSource.enabled = false;
            isDed = true;
            navMeshAgent.isStopped = true;  // Dừng di chuyển
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.enabled = false;
            PlayAnim(CacheString.TAG_ZDIE);

            AudioManager.Ins.zombieSource.Stop(); // Dừng tất cả âm thanh trước khi phát âm thanh chết
            AudioManager.Ins.PlayZombieClip(AudioManager.Ins.zombieDeath);

            yield return new WaitForSeconds(3.5f);
            Destroy(this.gameObject);
            //SimplePool.Despawn(this);
        }
        else
        {
            AudioManager.Ins.PlayZombieClip(AudioManager.Ins.zombieHurt);
        }

        yield return null;
    }


    public bool DetectedPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.Ins.player.transform.position);

        if (distanceToPlayer <= detectedRange)
        {
            //Debug.Log("In Range");
            return true;
        }
        else
        {
            //Debug.Log("Out Range");
            return false;
        }
    }

    public bool AbleToAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.Ins.player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            //Debug.Log("In Range");
            return true;
        }
        else
        {
            //Debug.Log("Out Range");
            return false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Bullet b = Cache.GetBullet(collision.gameObject);
        if (b != null && !isDed)
        {
            SimplePool.Despawn(b);
            ContactPoint contact = collision.contacts[0];
            //Debug.Log("B");
            GameObject blood = Instantiate(BloodEff, contact.point, Quaternion.LookRotation(contact.normal));
            ParticleSystem ps = blood.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
            
            StartCoroutine(TakeDamage(takeDamage));
        }
    }


    /*    private void OnTriggerEnter(Collider other)
        {
            Bullet b = Cache.GetBullet(other);
            if (b != null)
            {
                SimplePool.Despawn(b);
                StartCoroutine(TakeDamage(takeDamage));
            }
        }*/

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectedRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
