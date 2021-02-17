using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Rörelse-variabler, [SerializeField] gör dessa synliga i "Inspector" i Unity.
    // Acceleration av karaktär
    [SerializeField] private float accelerationGround = 20.0f;
    [SerializeField] private float accelerationAir = 20.0f;

    [SerializeField] private float dragGround = 3.0f;
    [SerializeField] private float dragAir = 0.0f;

    // Hur hög hastighet karaktären hoppar med
    [SerializeField] private float jumpVelocity = 5.0f;

    // Det värdet som visar om vi är på mark eller inte.
    private bool onGround;

    // Vår rigidbody som vi förflyttar karaktären med
    private Rigidbody rb;
    
    //Senaste input för riktning, används i FixedUpdate för att förflytta karaktären.
    private Vector3 lastInput;

    //Nuvarande accelerations-multiplicerare
    private float currentAcceleration = 0;

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

        // Kollar om vi är på marken, och har tryckt space
        if (onGround && Input.GetKeyDown(KeyCode.Space)) 
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);

        if (onGround)
        {
            rb.drag = dragGround;
            currentAcceleration = accelerationGround;
        }
        else
        {
            rb.drag = dragAir;
            currentAcceleration = accelerationAir;
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

        rb.angularVelocity = new Vector3(0, rotation, 0);
        rotation = 0;
        // Sätter så att vi inte är på marken vid början av fysikens uppdatering.
        // Kollisioner sker efter FixedUpdate, så detta "nollställer" vår "på mark" status.
        SetOnGround(false);
    }
}
