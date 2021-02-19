using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // Vår spelare
    PlayerController p;

    // Det vi vill att kameran ska kolla på/mot
    [SerializeField] Transform lookAt;
    // Det vi roterar istället för kameran
    [SerializeField] Transform parentTransform;
    //Begränsar vad vår "raycast" kan träffa
    [SerializeField] LayerMask layerMask;

    // Sparar vår rotations-input så vi kan använda den i LateUpdate
    float rotation = 0;

    // Vårt standard-avstånd från spelaren (vår look-at)
    float defaultDistance;
    // Nuvarande avstånd (om det behövs)
    float currentDistance;

    // Hur långt borta vi kan vara som mest (position)
    Vector3 cameraPositionFarthest;

    // Hur nära vi kan vara (position)
    Vector3 cameraPositionNearest;

    // Start is called before the first frame update
    void Start()
    {
        // Hitta vår spelare i scenen
        p = FindObjectOfType<PlayerController>();
        // Kolla på vårt "lookAt"-mål med kameran
        transform.LookAt(lookAt, Vector3.up);


        // Om man tar en vektor minus en annan vektor så vår man deras relativa position till varandra
        // Sedan kan man använda det för att hitta avstånd.
        // Hur långt bort vi vanligtvis är (.magnitude hittar "storlek" på avståndet)
        defaultDistance = (transform.position - lookAt.transform.position).magnitude;
        // Vilken position som är längst bort sätts här
        cameraPositionFarthest = transform.localPosition;
        // Närmsta position sätts här
        cameraPositionNearest = lookAt.position;
        //Göm vår muspekare och lås den till vårt spel-fönster
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Kolla på vårt "mål" som heter "lookAt"
        transform.LookAt(lookAt);

        // Hämtar rotation från vår input från musen (Input Manager i menyn Edit->Project Settings->Input Manager)
        rotation = -Input.GetAxis("Mouse Y");

        // Skapa vår "stråle" från vår "Look at" mot kameran
        Ray r = new Ray(lookAt.position, -transform.forward);
        // Denna variabeln håller information om det vi träffar
        RaycastHit rayHit;

        // Physics.Raycast returnerar en boolean, så vi kan använda den i en if-sats.
        // Avståndet begränsar med förberäknat max-avstånd(defaultDistance)
        // Vi använder även en "layer mask" så vi begränsar vad strålen kan träffa, i detta fallet inte spelaren
        if (Physics.Raycast(r, out rayHit, defaultDistance, layerMask))
        {
            //Hitta hur långt bort vi träffade
            currentDistance = rayHit.distance;
            //Normalisera (begränsa avståndet till mellan 0 och 1)
            float distance = currentDistance / defaultDistance;
            // Interpolera (gå mellan) avstånden med hjälp av vårt normaliserade avstånd
            // Detta använder ett tidigare definerat max-avstånd position och minimum-avstånd position.
            transform.localPosition = Vector3.Lerp(cameraPositionNearest, cameraPositionFarthest, distance);
        }
        else
        {
            //Om vi inte träffar något med strålen, sätt kamera-positionen på max-avståndet.
            currentDistance = defaultDistance;
            transform.localPosition = cameraPositionFarthest;
        }
    }

    //Late Update anropas sist av alla uppdateringar, hjälper att stabilisera rörelse för kameran då fysiken har redan beräknats.
    private void LateUpdate()
    {
        //Anropas här för att förhindra att kameran "skakar" eller laggar.
        // Linjär interpolering mellan nuvarande position på vår "parent", dvs förälder och spelar-positionen.
        // * 6 gör att kameran rör sig snabbare mot spelar-positionen.
        parentTransform.position = Vector3.Lerp(parentTransform.position, p.transform.position, 6 * Time.deltaTime);
        // Hämta nuvarande rotation i lättare att modifiera "vinklar" för både spelaren och vår föräldern.
        Vector3 pRot = parentTransform.rotation.eulerAngles;
        Vector3 playerRot = p.transform.rotation.eulerAngles;
        // Sätt rotaionen till spelar-rotationen i endast y-led, dvs horisontell rotation.
        // Använd x-rotationen (vertikal) från vår förälder för att behålla den.
        // Vi använder inte z så den sätts till 0 (för nuvarande).
        parentTransform.rotation = Quaternion.Euler(pRot.x + rotation, playerRot.y, 0);
    }
}
