using System;
using UnityEngine;

public class AttackMovement : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
