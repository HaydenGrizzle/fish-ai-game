using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public Reciever reciever;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    private Vector3 direction = Vector3.zero;

    void Update()
    {
        if (reciever == null || string.IsNullOrEmpty(reciever.data))
        {
            return;
        }

        string[] parts = reciever.data.Split(',');
        if (parts.Length >= 3)
        {
            if(float.TryParse(parts[0], out float x) &&
            float.TryParse(parts[1], out float y) &&
            float.TryParse(parts[2], out float z))
            {
                Vector3 targetDir = new Vector3(x, y, z);
                direction = Vector3.Lerp(direction, targetDir, Time.deltaTime * 5f);
                transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
                
                if ( direction.magnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
    }
}
