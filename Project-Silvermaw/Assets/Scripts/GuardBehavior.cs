using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBehavior : MonoBehaviour
{
    public NavMeshPatroller patrolScript;
    private Vector3 lastKnownPosition;
    public sightBox sight;
    public Transform interactPoint;
    public GameController game;

    [System.Serializable]
    public class Stats
    {
        public float interactDistance = 3;
        public float susThreshold = 20;
        public float alertThreshold = 100;
        public float caution = 10;
        public float ADHD = 10;
        public float perception = 0.4f;
        public float courage = 10;
    }

    public enum State
    {
        Patrolling,
        Suspicious,
        Alerted,
        Combat
    }

    public State GuardState = State.Patrolling;

    public Stats stats = new Stats();

    public float suspicion = 1;

    public bool PlayerInSight = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerInSight)
        {
            //if its below 0, make it 0
            suspicion = Mathf.Max(suspicion - (stats.ADHD * Time.deltaTime), 0);
        }
        if(suspicion < stats.susThreshold)
        {
            sight.GetComponent<Renderer>().material = sight.Mat;
        }
        else if( suspicion >= stats.susThreshold && suspicion < stats.alertThreshold)
        {
            GuardState = State.Suspicious;
            patrolScript.Patrolling = false;
            patrolScript.agent.SetDestination(lastKnownPosition);
            sight.GetComponent<Renderer>().material = sight.SusMat;
            Debug.Log(suspicion);
        }
        else if (suspicion >= stats.alertThreshold)
        {
            GuardState = State.Alerted;
            patrolScript.Patrolling = false;
            patrolScript.agent.SetDestination(lastKnownPosition);

            game.Lose();
            sight.GetComponent<Renderer>().material = sight.AlertMat;
        }
        Debug.DrawLine(transform.position, lastKnownPosition, Color.yellow);
        //Debug.Log("Guard's Suspicion: " + suspicion);
    }

    void LateUpdate()
    {
        const int ignoreRaycast = 2;
        const int ignoreLightLayer = 10;
        const int layerMask = ~((1 << ignoreLightLayer) | (1 << ignoreRaycast)) ;

        if (Physics.Raycast(interactPoint.position, interactPoint.forward, out RaycastHit hitInfo, stats.interactDistance, layerMask))
        {
			Door door = hitInfo.collider.GetComponent<Door>();
            if (door != null && !door.opened)
            {
                Debug.Log("interactable found--Guard");
				door.Toggle(gameObject);
            }
        }
        Debug.DrawRay(interactPoint.position, interactPoint.forward * stats.interactDistance, Color.magenta);
    }

    public void determineSight(PlayerController player)
    {
        //perception--lower is better
        if (player.luminance >= stats.perception)
        {
            PlayerInSight = true;
            suspicion += Time.deltaTime * (stats.caution * (1 + player.luminance));
            lastKnownPosition = player.transform.position;
        }
        else
        {
            PlayerInSight = false;
        }
    }
}
