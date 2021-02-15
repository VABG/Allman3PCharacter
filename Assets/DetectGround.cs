using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectGround : MonoBehaviour
{
    //Lokal variabel där vi sparar vårt spelarskript för åtkomst i metoder
    private PlayerController p;

    void Start()
    {
        //Hitta vårt spelarskript i vår "Parent", dvs förälder.
        p = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerStay(Collider other)
    {
        //sätt så vår spelar-karaktär är på marken. OnTriggerStay anropas EFTER FixedUpdate
        p.SetOnGround(true);
    }
}
