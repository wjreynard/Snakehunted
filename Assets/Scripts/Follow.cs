using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;
    public float smoothSpeed;

    private void Awake()
    {
        // offset borrowed from: https://learn.unity.com/tutorial/movement-basics?projectId=5c514956edbc2a002069467c#5c7f8528edbc2a002053b711
        offset = transform.position - target.position;
    }

    // research on LateUpdate() or FixedUpdate():
    //  1. https://forum.unity.com/threads/solved-camera-jitter-as-soon-as-using-lerp.762116/
    //  2. https://starmanta.gitbooks.io/unitytipsredux/content/all-my-updates.html
    // LateUpdate(), or FixedUpdate(), if moving player via Rigidbody physics
    private void FixedUpdate()
    {
        MoveFollower();
    }

    private void MoveFollower()
    {
        Player player = target.gameObject.GetComponent<Player>();
        Vector3 projectedDirection = player.direction * player.maxSpeed;
        Vector3 desiredPosition = target.position + offset + projectedDirection;
        // Vector3.Lerp() from borrowed from: https://www.youtube.com/watch?v=MFQhpwc6cKE
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.time);
        transform.position = smoothedPosition;
    }
}
