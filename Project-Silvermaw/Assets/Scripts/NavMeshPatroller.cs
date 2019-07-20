using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshPatroller : MonoBehaviour
{
    private bool patrolling = false;
    private int targetPointIndex = -1;
    public NavMeshAgent agent = new NavMeshAgent();
    public float patrolPointDeadzone = 0;
    public Animator modelAnimator;

    public PatrolType patrolType = PatrolType.Loop;

    //increments list iteration--will be negative or positive
    private int pingDirection = 1;

    public enum PatrolType
    {
        Loop, PingPong
    }

    //this is called a property, value is a keyword for the prop's actual value set in game.
    public bool Patrolling
    {
        get { return patrolling; }
        set
        {
            if (patrolling != value)
            {
                //if value = true, start patrolling
                if (value)
                {
                    targetPointIndex = findNearestPPIndex();
                    agent.SetDestination(PatrolRoute[targetPointIndex].position);
                    Debug.DrawLine(transform.position, PatrolRoute[targetPointIndex].position, Color.cyan);
                }
                else
                {
                    targetPointIndex = -1;
                    agent.ResetPath();
                }
            }
        }
    }

    public List<Transform> PatrolRoute = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        Patrolling = true;
        //for attaching
        if (GetComponentInChildren<Animator>())
        {
            modelAnimator = GetComponentInChildren<Animator>();
        }
        else
        {
            Debug.LogError("The Character needs an Animation controller");
        }
    }

    // Update is called once per frame
    void Update()
    {
        modelAnimator.SetBool("GuardWalk", true);
        if (agent.remainingDistance <= patrolPointDeadzone)
        {
            switch (patrolType)
            {
                case PatrolType.Loop:
                    targetPointIndex++;
                    //modding the index by the array's count simulates looping 
                    targetPointIndex = targetPointIndex % PatrolRoute.Count;
                    break;
                case PatrolType.PingPong:
                    //if the beginning or end has been reached...
                    if (targetPointIndex == 0 || targetPointIndex == PatrolRoute.Count - 1)
                    {
                        pingDirection *= -1;
                    }
                    targetPointIndex += pingDirection;
                    break;
            }
            agent.SetDestination(PatrolRoute[targetPointIndex].position);
            
        }
        Debug.DrawLine(transform.position, PatrolRoute[targetPointIndex].position, Color.cyan);
    }

    //iterates through internal patrol route list
    private int findNearestPPIndex()
    {
        int foundIndex = 0;
        //init to first patrol route point's distance from current position
        float minDistance = (PatrolRoute[foundIndex].position - transform.position).magnitude;

        //you can access the type's properties after parentheses
        for(int i = 0; i < PatrolRoute.Count; i++)
        {
            if(minDistance > (PatrolRoute[i].position - transform.position).magnitude)
            {
                minDistance = (PatrolRoute[i].position - transform.position).magnitude;
                foundIndex = i;
            }
        }
        Debug.Log("Lost patrol. Resuming, now heading to: " + foundIndex);
        return foundIndex;
    }
}
