using UnityEngine;
using System.Collections;
using Unity.Jobs;
using Unity.Collections;
using System.Collections.Generic;

/// <summary>
/// comportamiento de un enemigo que ataca en mele
/// </summary>
public class MeleEnemy : MonoBehaviour
{

    public float m_InitialVelocity = 0.5f;
    public float m_MaxVelocity = 20f;
    //public float m_gravity = 20f;
    public float m_accel = 20f;
    public float m_yOffsetToRay = 0.25f;
    public string m_tagTarget = "Player";
    public bool HasWalAnim = true;
    public bool HasAttackAnim = true;
    public bool HasDeathAnim = true;

    public float YPlane = 0.02681851f;

    public float m_stopDistance = 6f;
    public float m_runDistance = 10f;
    public float m_timeBetweenAttack = 2f;

    public bool m_pathfinding = false;

    private float m_groundSpeed = 0f;
    private Quaternion m_deathRotationBlock;
    //private float m_airSpeed = 0f;
    private Vector3 m_direction = Vector3.zero;
    private GameObject m_target;
    //private enum TState{ MOVE, ATTACK, DEATH};
    private enum TState { MOVE, ATTACK, DEATH, INIT_SEARCH, FOLLOW_PATH }; //new
    private TState m_state = TState.MOVE;
    private float m_attackTime;
    private PlayerAnimation m_animationcomponent;
    private bool m_started = false;
    private List<Vector3> m_waypointList;
    //new
    private int m_currentWaypoint;
    private float m_distanceToChangeWaypoint = 2.0f;



    void OnEnable()
    {
        if (m_started)
        {
            ReStart();
        }
    }

    protected void Start()
    {
        ReStart();
        m_started = true;
    }

    protected void ReStart()
    {
        m_waypointList = new List<Vector3>();
        m_target = GameObject.FindGameObjectWithTag(m_tagTarget) as GameObject;
        m_groundSpeed = m_InitialVelocity;
        m_attackTime = 0f;
        m_animationcomponent = GetComponent<PlayerAnimation>();
        //m_state = TState.MOVE;
        m_state = TState.INIT_SEARCH;//new
        if (m_animationcomponent != null)
            m_animationcomponent.RegisterOnAnimationEvent = OnAnimationFinish;
    }


    // Tick is called once per frame
    protected void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 5f, Color.red);

        if (m_state != TState.DEATH)
            Common();

        if (m_state == TState.INIT_SEARCH)
            InitSearchAStart();

        if (m_state == TState.FOLLOW_PATH)
            FollowPath();
        else if (m_state == TState.MOVE)
        {
            Look(m_target.transform.position);
            Move();
        }
        else if (m_state == TState.ATTACK)
            Attack();
        else
        {
            //Muerto...
            //Debug.Log(name + " DEATH");
            transform.rotation = m_deathRotationBlock;
        }
    }


    protected void Attack()
    {
        m_groundSpeed = m_InitialVelocity;
        m_attackTime -= Time.deltaTime;
        bool attackAnim = true;
        if (m_attackTime <= 0f)
        {
            m_attackTime = m_timeBetweenAttack;
            attackAnim = false;
        }

        if (m_direction.sqrMagnitude > m_runDistance * m_runDistance)
        {
            m_state = TState.MOVE;
            attackAnim = false;
        }

        if (HasAttackAnim)
            m_animationcomponent.IsAttacking = attackAnim;
    }
    //new
    /// <summary>
    /// Implementa rutinas válidos para todos lso estados escepto el de muerte.
    /// </summary>
    protected void Common()
    {
        RaycastHit hit;
        Vector3 direction = m_target.transform.position - this.transform.position;

        Vector3 center = new Vector3(transform.position.x, transform.position.y + m_yOffsetToRay, transform.position.z);
        Debug.DrawRay(center, direction * 5f, Color.green);
        bool targetVisible = false;
        if (Physics.Raycast(center, direction, out hit))
        {
            //Si te veo
            if (hit.collider.gameObject == m_target)
            {
                targetVisible = true;
                m_direction = direction;
                //Si no te tengo a tiro
                if (m_direction.sqrMagnitude > m_stopDistance * m_stopDistance)
                {
                    m_state = TState.MOVE;
                }
                else
                {
                    //Si te tengo a tiro...
                    m_state = TState.ATTACK;
                    m_attackTime = m_timeBetweenAttack;
                    if (HasAttackAnim)
                        m_animationcomponent.IsAttacking = true;
                }
            }
        }

        if (!targetVisible)
        {
            //Si estaba buscando directamente y le pierdo de vista, usa A*
            if (m_state == TState.MOVE || m_state == TState.ATTACK)
                m_state = TState.INIT_SEARCH;
        }
    }


    /// <summary>
    /// Inicializa A*
    /// </summary>
    protected void InitSearchAStart()
    {
        
        /*NavigationMap navigationMap = GameMgr.GetInstance().GetCustomMgrs().GetServer<NavigationMap>();
        WayPoint wpInit = navigationMap.FindWayPointNear(this.transform.position);
        WayPoint wpEnd = navigationMap.FindWayPointNear(m_target.transform.position);*/
        // TODO: En vez de devolver solo el último punto llamar al PathfindingJob
        if (m_pathfinding)
        {

        }
        else
        {
            m_waypointList.Clear();
            //m_waypointList.Add(wpInit.transform.position);
            //m_waypointList.Add(wpEnd.transform.position);
        }
    }

    /// <summary>
    /// Sigue la ruta proporcionada por A*
    /// </summary>
    protected void FollowPath()
    {
        Vector3 vDirection = m_waypointList[m_currentWaypoint] - transform.position;

        if (vDirection.sqrMagnitude < m_distanceToChangeWaypoint * m_distanceToChangeWaypoint)
        {

            NextWaypoint();
        }
        else
        {
            Look(m_waypointList[m_currentWaypoint]);
            Move();
        }
    }

    //new 
    protected void Move()
    {
        if (m_direction.sqrMagnitude > m_stopDistance * m_stopDistance)
        {
            //Debug.Log("Move");
            if (HasWalAnim)
                m_animationcomponent.IsWalking = true;


            //Move
            float increment = Trajectories.UniformAccelerateMovement(m_groundSpeed, m_accel, Time.deltaTime);
            //m_direction.y = 0f;
            if (m_groundSpeed < m_MaxVelocity)
            {
                m_groundSpeed = Trajectories.UniformAccelerateVelocity(m_groundSpeed, m_accel, Time.deltaTime);
            }
            Vector3 fordward = transform.forward;
            fordward.y = 0f;
            this.GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + fordward * increment);
            //transform.position = new Vector3(transform.position.x, YPlane, transform.position.z);
            //transform.position += transform.forward*increment;
            //m_state = TState.MOVE;
        }

    }

    protected void Look(Vector3 newPosition)
    {
        m_direction = newPosition;
        m_direction.y = transform.position.y;
        this.transform.LookAt(m_direction);
    }

    //New
    protected void NextWaypoint()
    {
        m_currentWaypoint++;
        if (m_currentWaypoint >= m_waypointList.Count)
        {
            //He llegado al destino. Recalculo.
            m_currentWaypoint = -1;
            m_state = TState.INIT_SEARCH;
        }
        else
        {
            m_state = TState.FOLLOW_PATH;
        }
    }

    void Death()
    {
        if (m_state != TState.DEATH)
        {
            m_state = TState.DEATH;
            m_target.SendMessage("EnemyDeath");
            m_deathRotationBlock = transform.rotation;
            if (HasDeathAnim)
                m_animationcomponent.EventDeath();
            else
                OnAnimationFinish("death");
        }
    }


    void OnAnimationFinish(string name)
    {
        if (name == "death")
        {
            SendMessage("DestroyGameObject", SendMessageOptions.DontRequireReceiver);
        }
    }


    protected void OnDestroy()
    {
        if (m_animationcomponent != null)
            m_animationcomponent.UnRegisterOnAnimationEvent = OnAnimationFinish;
    }

}
