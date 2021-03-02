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
        // Sätter så vår spelar-karaktär är på marken. OnTriggerStay(denna metoden) anropas EFTER FixedUpdate
        p.SetOnGround(true);
    }

    private void FixedUpdate()
    {
        // Sätter så att vi inte är på marken vid början av fysikens uppdatering.
        // Kollisioner sker efter FixedUpdate, så detta "nollställer" vår "på mark" status.
        p.SetOnGround(false);
    }

}
