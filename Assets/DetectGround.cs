using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectGround : MonoBehaviour
{
    //Lokal variabel d�r vi sparar v�rt spelarskript f�r �tkomst i metoder
    private PlayerController p;

    void Start()
    {
        //Hitta v�rt spelarskript i v�r "Parent", dvs f�r�lder.
        p = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerStay(Collider other)
    {
        //s�tt s� v�r spelar-karakt�r �r p� marken. OnTriggerStay anropas EFTER FixedUpdate
        p.SetOnGround(true);
    }
}
