using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // R�relse-variabler, [SerializeField] g�r dessa synliga i "Inspector" i Unity.
    // Acceleration av karakt�r p� mark eller i luft.
    [SerializeField] private float accelerationGround = 20.0f;
    [SerializeField] private float accelerationAir = 20.0f;

    //Olika luftmost�nd om vi �r p� mark eller i luft.
    [SerializeField] private float dragGround = 3.0f;
    [SerializeField] private float dragAir = 0.0f;

    // Hur h�g hastighet karakt�ren hoppar med
    [SerializeField] private float jumpVelocity = 5.0f;

    // Det v�rdet som visar om vi �r p� mark eller inte.
    private bool onGround;

    [SerializeField] private float canJumpTime = 0.1f;
    private float offGroundTimer = 0;
    private bool canJump = false;

    private float stuckDetectionTimer = 0;

    // V�r rigidbody som vi f�rflyttar karakt�ren med
    private Rigidbody rb;
    
    //Senaste input f�r riktning, anv�nds i FixedUpdate f�r att f�rflytta karakt�ren.
    private Vector3 lastInput;

    //Nuvarande accelerations-multiplicerare
    private float currentAcceleration = 0;

    // H�ller i rotations-information s� vi kan anv�nda den i FixedUpdate f�r att p�verka fysiken.
    private float rotation = 0;

    void Start()
    {
        // H�mta v�r Rigidbody komponent s� vi kan anv�nda den
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 360;
    }

    // Update is called once per frame
    void Update()
    {
        // Nollst�ller input
        lastInput = Vector3.zero;

        // Kollar om n�gon av WASD �r nertryckt och l�gger till riktningsvektor
        if (Input.GetKey(KeyCode.W)) lastInput += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) lastInput += Vector3.back;
        if (Input.GetKey(KeyCode.D)) lastInput += Vector3.right;
        if (Input.GetKey(KeyCode.A)) lastInput += Vector3.left;

        // Ser till att storleken (l�ngden) p� vektorn �r 1.
        lastInput.Normalize();

        rotation += Input.GetAxis("Mouse X");

        //Om vi �r p� marken, �ndrar vi luftmost�ndet och hur snabbt vi accelererar
        if (onGround)
        {
            offGroundTimer = 0;
            canJump = true;

            rb.useGravity = false;
            // S�tt luftmost�nd
            rb.drag = dragGround;
            // S�tt accelerations-hastighet
            currentAcceleration = accelerationGround;
        }
        else
        {
            offGroundTimer += Time.deltaTime;
            if (offGroundTimer <= canJumpTime) canJump = true;
            else canJump = false;

            rb.useGravity = true;
            rb.drag = dragAir;
            currentAcceleration = accelerationAir;
        }
        bool triedJumping = false;

        // Kollar om vi kan hoppa, och har tryckt space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            triedJumping = true;
            if (canJump)
            {
                rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
                offGroundTimer = canJumpTime;
            }
        }

        if (!onGround && !canJump && lastInput.magnitude > 0 && rb.velocity.magnitude < 0.05f)
        {
            stuckDetectionTimer += Time.deltaTime;
            if (stuckDetectionTimer > 0.2f)
            {
                if (triedJumping) rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
            }
        }
        else
        {
            stuckDetectionTimer = 0;
        }

    }

    // Metod som �ndrar det som beh�vs n�r spelaren �r p� marken.
    public void SetOnGround(bool onGround)
    {
        // this �r denna klassen, och anv�nds f�r att skilja p� den lokala variabeln och klass-variabeln.
        this.onGround = onGround;
    }
    private void FixedUpdate()
    {
        // L�gg till kraft p� v�r krakt�r med acceleration baserat p� v�r input (lastInput)
        // D� lastInput �r en riktning s� anv�nder vi variabeln "acceleration" f�r att �ndra hur snabbt karakt�ren r�r sig.
        rb.AddRelativeForce(lastInput * currentAcceleration, ForceMode.Acceleration);
        //S�tter v�r rotations-hastighet f�r att rotera spelaren.
        rb.angularVelocity = new Vector3(0, rotation, 0);
        // Ser till att nollst�lla/�terst�lla rotationen till 0 s� att den inte bara �kar konstant.
        rotation = 0;
    }
}
