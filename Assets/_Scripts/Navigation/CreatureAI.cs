using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CreatureAI : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] State CurrentState = State.Wandering;
    [SerializeField] MazeGenerations MazeGenerator;

    [SerializeField] float SampleRadius = 20f, VisualThreshold = 30f, DegreesOfView = 90f;
    [SerializeField] float DefaultAgentSpeed, Timer, TimeUntilChange = 5f;
    
    NavMeshAgent Agent;

    enum State { Wandering, Chasing }

    private void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        DefaultAgentSpeed = Agent.speed;
    }

    private void Update()
    {
        //STATES

        if(CurrentState == State.Wandering)
        {
            Agent.speed = DefaultAgentSpeed;

            if (!Agent.hasPath && !Agent.pathPending)
            {
                MazeTile tile = MazeGenerator.GetRandomTileWithinRange(transform.position, SampleRadius / 2f, SampleRadius);
                if (tile == null) { return; }
                else
                {
                    Vector3 point = tile.transform.position;

                    if (NavMesh.SamplePosition(point, out NavMeshHit hit, SampleRadius, 1))
                    {
                        Agent.SetDestination(hit.position);
                    }
                    else
                        Debug.Log("Found no position!");
                }
            }
            else
                Debug.Log("Agent has path!");
        }

        if(CurrentState == State.Chasing)
        {
            Agent.speed = DefaultAgentSpeed * 2f;
            Agent.SetDestination(Player.position);
        }

        //STATE CHANGES

        if(CurrentState == State.Wandering)
        {
            Vector3 direction = (Player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, Player.position);

            Physics.Raycast(transform.position, direction, out RaycastHit hit);

            if(hit.collider.transform == Player && angle < DegreesOfView / 2f)
            {
                float distance = Vector3.Distance(transform.position, Player.position);
                if(distance < VisualThreshold)
                {
                    CurrentState = State.Chasing;
                }
            }
        }

        if(CurrentState == State.Chasing)
        {
            float distance = Vector3.Distance(Player.position, transform.position);
            float angle = Vector3.Angle(transform.forward, Player.position);

            if (distance > VisualThreshold || angle > DegreesOfView / 2f)
            {
                Timer += Time.deltaTime;
                if (Timer >= TimeUntilChange)
                {
                    CurrentState = State.Wandering;
                    Timer = 0f;
                }
            }
            else
                Timer = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, SampleRadius);
        Gizmos.DrawWireSphere(transform.position, VisualThreshold);
    }
}
