using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;

public class FihState : MonoBehaviour
{
    private float mass = 0.1f;
    public TMP_Text massText;
    public ParticleSystem munch;

    void Update()
    {
        if(massText != null)
        {
            massText.text = "MASS: " + mass.ToString("F2") + "lb";
        }
    }

    public void Grow(float amount)
    {
        Vector3 growth = new Vector3(amount, amount, amount);
        transform.localScale += growth;
        munch.Play();
        mass += amount;
    }

    public float GetMass()
    {
        return mass;
    }
}
