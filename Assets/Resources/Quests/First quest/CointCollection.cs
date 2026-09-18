using System;
using UnityEngine;

public class CointCollection : MonoBehaviour
{
    private int Coins = 0;

    public event Action CoinCollect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Coins++;
            CoinCollect?.Invoke();
            Destroy(gameObject);
        }
    }
}
