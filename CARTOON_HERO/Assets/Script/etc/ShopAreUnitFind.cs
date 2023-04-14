using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopAreUnitFind : MonoBehaviour
{
    [SerializeField]
    GameObject Shop_stick;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Shop_stick.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Shop_stick.SetActive(false);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
