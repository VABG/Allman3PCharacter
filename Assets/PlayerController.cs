using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Rörelse-variabler, [SerializeField] gör dessa synliga i "Inspector" i Unity.
    // Acceleration av karaktär på mark eller i luft.
    [SerializeField] private float accelerationGround = 20.0f;
    [SerializeField] private float accelerationAir = 20.0f;

    //Olika luftmostånd om vi är på mark eller i luft.
    [SerializeField] private float dragGround = 3.0f;
    [SerializeField] private float dragAir = 0.0f;

    // Hur hög hastighet karaktären hoppar med
    [SerializeField] private float jumpVelocity = 5.0f;

    // Det värdet som visar om vi är på mark eller inte.
    private bool onGround;

    [SerializeField] private float canJumpTime = 0.1f;
    private float offGroundTimer = 0;
    private bool canJump = false;

    private float stuckDetectionTimer = 0;

    // Vår rigidbody som vi förflyttar karaktären med
    private Rigidbody rb;
    
    //Senaste input för riktning, används i FixedUpdate för att förflytta karaktären.
    private Vector3 lastInput;

    //Nuvarande accelerations-multiplicerare
    private float currentAcceleration = 0;

    // Håller i rotations-information så vi kan använda den i FixedUpdate för att påverka fysiken.
    private float rotation = 0;

    void Start()
    {
        // Hämta vår Rigidbody komponent så vi kan använda den
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 360;
    }

    // Update is called once per frame
    void Update()
    {
        // Nollställer input
        lastInput = Vector3.zero;

        // Kollar om någon av WASD är nertryckt och lägger till riktningsvektor
        if (Input.GetKey(KeyCode.W)) lastInput += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) lastInput += Vector3.back;
        if (Input.GetKey(KeyCode.D)) lastInput += Vector3.right;
        if (Input.GetKey(KeyCode.A)) lastInput += Vector3.left;

        // Ser till att storleken (längden) på vektorn är 1.
        lastInput.Normalize();

        rotation += Input.GetAxis("Mouse X");

        //Om vi är på marken, ändrar vi luftmoståndet och hur snabbt vi accelererar
        if (onGround)
        {
            offGroundTimer = 0;
            canJump = true;

            rb.useGravity = false;
            // Sätt luftmostånd
            rb.drag = dragGround;
            // Sätt accelerations-hastighet
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

    // Metod som ändrar det som behövs när spelaren är på marken.
    public void SetOnGround(bool onGround)
    {
        // this är denna klassen, och används för att skilja på den lokala variabeln och klass-variabeln.
        this.onGround = onGround;
    }
    private void FixedUpdate()
    {
        // Lägg till kraft på vår kraktär med acceleration baserat på vår input (lastInput)
        // Då lastInput är en riktning så använder vi variabeln "acceleration" för att ändra hur snabbt karaktären rör sig.
        rb.AddRelativeForce(lastInput * currentAcceleration, ForceMode.Acceleration);
        //Sätter vår rotations-hastighet för att rotera spelaren.
        rb.angularVelocity = new Vector3(0, rotation, 0);
        // Ser till att nollställa/återställa rotationen till 0 så att den inte bara ökar konstant.
        rotation = 0;
    }
}
