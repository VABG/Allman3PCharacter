using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // V�r spelare
    PlayerController p;

    // Det vi vill att kameran ska kolla p�/mot
    [SerializeField] Transform lookAt;
    // Det vi roterar ist�llet f�r kameran
    [SerializeField] Transform parentTransform;
    //Begr�nsar vad v�r "raycast" kan tr�ffa
    [SerializeField] LayerMask layerMask;

    // Sparar v�r rotations-input s� vi kan anv�nda den i LateUpdate
    float rotation = 0;

    // V�rt standard-avst�nd fr�n spelaren (v�r look-at)
    float defaultDistance;
    // Nuvarande avst�nd (om det beh�vs)
    float currentDistance;

    // Hur l�ngt borta vi kan vara som mest (position)
    Vector3 cameraPositionFarthest;

    // Hur n�ra vi kan vara (position)
    Vector3 cameraPositionNearest;

    // Start is called before the first frame update
    void Start()
    {
        // Hitta v�r spelare i scenen
        p = FindObjectOfType<PlayerController>();
        // Kolla p� v�rt "lookAt"-m�l med kameran
        transform.LookAt(lookAt, Vector3.up);


        // Om man tar en vektor minus en annan vektor s� v�r man deras relativa position till varandra
        // Sedan kan man anv�nda det f�r att hitta avst�nd.
        // Hur l�ngt bort vi vanligtvis �r (.magnitude hittar "storlek" p� avst�ndet)
        defaultDistance = (transform.position - lookAt.transform.position).magnitude;
        // Vilken position som �r l�ngst bort s�tts h�r
        cameraPositionFarthest = transform.localPosition;
        // N�rmsta position s�tts h�r
        cameraPositionNearest = lookAt.position;
        //G�m v�r muspekare och l�s den till v�rt spel-f�nster
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Kolla p� v�rt "m�l" som heter "lookAt"
        transform.LookAt(lookAt);

        // H�mtar rotation fr�n v�r input fr�n musen (Input Manager i menyn Edit->Project Settings->Input Manager)
        rotation = -Input.GetAxis("Mouse Y");

        // Skapa v�r "str�le" fr�n v�r "Look at" mot kameran
        Ray r = new Ray(lookAt.position, -transform.forward);
        // Denna variabeln h�ller information om det vi tr�ffar
        RaycastHit rayHit;

        // Physics.Raycast returnerar en boolean, s� vi kan anv�nda den i en if-sats.
        // Avst�ndet begr�nsar med f�rber�knat max-avst�nd(defaultDistance)
        // Vi anv�nder �ven en "layer mask" s� vi begr�nsar vad str�len kan tr�ffa, i detta fallet inte spelaren
        if (Physics.Raycast(r, out rayHit, defaultDistance, layerMask))
        {
            //Hitta hur l�ngt bort vi tr�ffade
            currentDistance = rayHit.distance;
            //Normalisera (begr�nsa avst�ndet till mellan 0 och 1)
            float distance = currentDistance / defaultDistance;
            // Interpolera (g� mellan) avst�nden med hj�lp av v�rt normaliserade avst�nd
            // Detta anv�nder ett tidigare definerat max-avst�nd position och minimum-avst�nd position.
            transform.localPosition = Vector3.Lerp(cameraPositionNearest, cameraPositionFarthest, distance);
        }
        else
        {
            //Om vi inte tr�ffar n�got med str�len, s�tt kamera-positionen p� max-avst�ndet.
            currentDistance = defaultDistance;
            transform.localPosition = cameraPositionFarthest;
        }
    }

    //Late Update anropas sist av alla uppdateringar, hj�lper att stabilisera r�relse f�r kameran d� fysiken har redan ber�knats.
    private void LateUpdate()
    {
        //Anropas h�r f�r att f�rhindra att kameran "skakar" eller laggar.
        // Linj�r interpolering mellan nuvarande position p� v�r "parent", dvs f�r�lder och spelar-positionen.
        // * 6 g�r att kameran r�r sig snabbare mot spelar-positionen.
        parentTransform.position = Vector3.Lerp(parentTransform.position, p.transform.position, 6 * Time.deltaTime);
        // H�mta nuvarande rotation i l�ttare att modifiera "vinklar" f�r b�de spelaren och v�r f�r�ldern.
        Vector3 pRot = parentTransform.rotation.eulerAngles;
        Vector3 playerRot = p.transform.rotation.eulerAngles;
        // S�tt rotaionen till spelar-rotationen i endast y-led, dvs horisontell rotation.
        // Anv�nd x-rotationen (vertikal) fr�n v�r f�r�lder f�r att beh�lla den.
        // Vi anv�nder inte z s� den s�tts till 0 (f�r nuvarande).
        parentTransform.rotation = Quaternion.Euler(pRot.x + rotation, playerRot.y, 0);
    }
}
