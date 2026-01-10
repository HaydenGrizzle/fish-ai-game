using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public float angleChangeInterval = 15f;
    public float transitionDuration = 2f; // how long it takes to move to new angle

    public Vector3[] localOffsets; // offsets relative to fish
    private Vector3 currentOffset;
    private Vector3 nextOffset;
    private int currentIndex = 0;
    private float timer = 0f;
    private float transitionTimer = 0f;
    private bool isTransitioning = false;

    void Start()
    {
        if (localOffsets.Length == 0)
        {
            localOffsets = new Vector3[]
            {
                new Vector3(0, 5, -10),  // Behind
                new Vector3(0, 3, 8),    // Front
                new Vector3(0, 1, -6)    // Low angle
            };
        }

        currentOffset = localOffsets[0];
        nextOffset = currentOffset;
    }

    void LateUpdate()
    {
        timer += Time.deltaTime;

        // Time to pick next offset
        if (timer >= angleChangeInterval && !isTransitioning)
        {
            timer = 0f;
            currentIndex = (currentIndex + 1) % localOffsets.Length;
            nextOffset = localOffsets[currentIndex];
            transitionTimer = 0f;
            isTransitioning = true;
        }

        Vector3 desiredPosition;

        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);

            // Smoothly interpolate between current and next offset
            Vector3 currentRotated = target.rotation * currentOffset;
            Vector3 nextRotated = target.rotation * nextOffset;
            desiredPosition = Vector3.Lerp(transform.position, target.position + nextRotated, t);

            // Rotate camera smoothly to look at the fish
            Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, t);

            if (t >= 1f)
            {
                // Finished transition
                currentOffset = nextOffset;
                isTransitioning = false;
            }
        }
        else
        {
            desiredPosition = target.position + (target.rotation * currentOffset);
            transform.rotation = Quaternion.LookRotation(target.position - transform.position);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}
