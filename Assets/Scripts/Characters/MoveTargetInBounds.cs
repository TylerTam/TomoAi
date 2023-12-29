using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetInBounds : MonoBehaviour
{
    public Transform Target;
    public Vector2 RandomStillTime;

    public Vector3 Bounds;
    public Vector3 BoundsOffset;

    private float timerTarget;
    private float currentTimer;

    [Header("Debug")]
    [SerializeField] private bool debug;
    [SerializeField] private Color BoundsColor;

    private void Awake()
    {
        NewPosition();
    }
    private void Update()
    {
        currentTimer += Time.deltaTime;
        if(currentTimer >= timerTarget)
        {
            NewPosition();
        }
    }
    private void NewPosition()
    {
        
        
        Target.transform.position = GetRandomPos();
        timerTarget = Random.Range(RandomStillTime.x, RandomStillTime.y);
        currentTimer = 0;
    }

    public Vector3 GetRandomPos()
    {
        Vector3 newPos = new Vector3(Random.Range(-Bounds.x / 2, Bounds.x / 2), Random.Range(-Bounds.y / 2, Bounds.y / 2), Random.Range(-Bounds.z / 2, Bounds.z / 2));
        newPos += transform.position + BoundsOffset;
        return newPos;
    }

    private void OnDrawGizmos()
    {
       if(debug)
        {
            Gizmos.color = BoundsColor;
            Gizmos.DrawCube(transform.position + BoundsOffset, Bounds);
        }
    }
}
