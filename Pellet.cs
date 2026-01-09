using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    public float growthAmount = 1f;

    private void OnTriggerEnter(Collider other)
    {
        FihState fih = other.GetComponent<FihState>();
        if(fih != null)
        {
            fih.Grow(growthAmount);
            Destroy(gameObject);
        }
    }
}
