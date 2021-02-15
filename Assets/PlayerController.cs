using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // R�relse-variabler, [SerializeField] g�r dessa synliga i "Inspector" i Unity.
    // Acceleration av karakt�r
    [SerializeField] private float acceleration = 20.0f;
    // Hur h�g hastighet karakt�ren hoppar med
    [SerializeField] private float jumpVelocity = 5.0f;

    // Det v�rdet som visar om vi �r p� mark eller inte.
    private bool onGround;

    // V�r rigidbody som vi f�rflyttar karakt�ren med
    private Rigidbody rb;
    
    //Senaste input f�r riktning, anv�nds i FixedUpdate f�r att f�rflytta karakt�ren.
    private Vector3 lastInput;
    
    void Start()
    {
        // H�mta v�r Rigidbody komponent s� vi kan anv�nda den
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Nollst�ller input
        lastInput = Vector3.zero;

        //Kollar om n�gon av WASD �r nertryckt och l�gger till riktningsvektor
        if (Input.GetKey(KeyCode.W)) lastInput += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) lastInput += Vector3.back;
        if (Input.GetKey(KeyCode.D)) lastInput += Vector3.right;
        if (Input.GetKey(KeyCode.A)) lastInput += Vector3.left;
        // Ser till att storleken (l�ngden) p� vektorn �r 1.
        lastInput.Normalize();
        
        //Kollar om vi �r p� marken, och har tryckt space
        if (onGround && Input.GetKeyDown(KeyCode.Space)) 
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
    }

    //Metod som �ndrar det som beh�vs n�r spelaren �r p� marken.
    public void SetOnGround(bool onGround)
    {
        // this �r denna klassen, och anv�nds f�r att skilja p� den lokala variabeln och klass-variabeln.
        this.onGround = onGround;
    }
    private void FixedUpdate()
    {
        // L�gg till kraft p� v�r krakt�r med acceleration baserat p� v�r input (lastInput)
        // D� lastInput �r en riktning s� anv�nder vi variabeln "acceleration" f�r att �ndra hur snabbt karakt�ren r�r sig.
        rb.AddForce(lastInput * acceleration, ForceMode.Acceleration);
        // S�tter s� att vi inte �r p� marken vid b�rjan av fysikens uppdatering.
        // Kollisioner sker efter FixedUpdate, s� detta "nollst�ller" v�r "p� mark" status.
        SetOnGround(false);
    }
}
