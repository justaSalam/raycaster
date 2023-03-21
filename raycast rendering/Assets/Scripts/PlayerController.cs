////Sals afwul code////
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    private ObjectPooler obejctPooler;
    [Header("Level")]
    public GameObject levelSprite;
    public GameObject spritePrefab;
    public Transform map;
    [Space, Header("Movement")]
    [Range(0, 100)]
    public int health = 100;
    public new Camera camera;
    private Rigidbody2D rb;
    public float movementSpeed;
    public float sensitivity;
    private float inputX;
    private float inputY;
    private Vector2 desiredMovement;
    private float desiredRotation;
    [Space, Header("Rendering")]
    [Tooltip("The amount of fog. Bigger value => less fog (dont question it)")]
    public float fogMultiplier;
    public float shading;
    public int FieldOfView = 100;
    public int RaycastCount = 50;
    [Space, Header("Dont change the next 4 values")]
    public float firstRayOffset;
    public float currentRayRotation;
    public float RayOffset;
    private float screenPosOffset;
    public List<RayInfoObject> raysList = new List<RayInfoObject>();
    public LayerMask WhatIsWall;

    [Space]
    public GameObject emptyObject;
    public GameObject wallPrefab;
    [Space, Header("Scaling")]
    [Tooltip("I dont even remember what this does, anyways dont change it in runtime because it might break some stuff")]
    public float renderDistanceMultiplier;
    public float xMultiplier;
    [Space, Header("Shooting")]
    public int damage = 17;
    public float fireRate;
    public Animator animator;
    private float nextTimeToFire;


    
    //Events and audio
    public delegate void OnEvent();
    public static OnEvent onDamage;
    private AudioSourceController asc;

    private void Awake()
    {
        onDamage += OnDamage;
       

        Screen.SetResolution(1920, 1080, true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        instance = this;
        Level.DrawMap2D(levelSprite);

        rb = GetComponent<Rigidbody2D>();

        firstRayOffset = FieldOfView / 2;
        RayOffset = FieldOfView / RaycastCount;
       
        for (int i = 1; i <= RaycastCount; i++)
        {
            RayInfoObject raycastObject = new RayInfoObject();

            raycastObject.gameObject = Instantiate(emptyObject, transform.position, transform.rotation, transform);

            screenPosOffset += /*5.33333*/30f; //if you want to make the screen wider, take the width of ur screen and divide it by 60 and replace the value
            raycastObject.screenPosition = screenPosOffset;



            raysList.Add(raycastObject);

            if (i == 1)
            {
                currentRayRotation -= RayOffset - firstRayOffset;
            }
            else
            {
                currentRayRotation -= RayOffset;
            }

           
            raycastObject.gameObject.transform.rotation = Quaternion.Euler(0, 0, currentRayRotation);
        }
        obejctPooler = ObjectPooler.instance;
    }

    private void Start()
    {
        asc = AudioSourceController.Instance;
    }
    private void Update()
    {
        inputY = Input.GetAxisRaw("Vertical") * movementSpeed; //basic 2D movement
        inputX = -Input.GetAxisRaw("Horizontal") * movementSpeed;
        float mouseX = -Input.GetAxisRaw("Mouse X") * sensitivity;

        desiredMovement = transform.up * inputY + inputX * transform.right;

        desiredRotation -= mouseX;
        transform.rotation = Quaternion.Euler(0, 0, desiredRotation);

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        if(Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            asc.PlayAudio("shootSound");
            animator.Play("shoot");
            nextTimeToFire = Time.time + 1f/fireRate;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 9999,WhatIsWall);
            Enemy enemy = hit.collider.gameObject?.GetComponent<Enemy>();

            if(enemy != null)
            {
                enemy.RecieveDamage(20);
            }
        }

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(desiredMovement.x, desiredMovement.y);
        DrawRays3D();

    }
    private void DrawLine3D(RaycastHit2D hit, int index, Color color)
    {
        GameObject wall = obejctPooler.SpawnFromQueue("quad");
        MeshRenderer renderer = wall.GetComponent<MeshRenderer>();
        //Position of the walls (i want to kms)
        Vector2 desiredPosition = new Vector2(camera.WorldToViewportPoint(Vector2.zero).x - (raysList[index].screenPosition - 960/*Width of the screen divided by 2*/), camera.WorldToViewportPoint(Vector2.zero).y);
        wall.transform.position = new Vector2(camera.ScreenToViewportPoint(desiredPosition).x * xMultiplier, desiredPosition.y);

        //Sets the height of the object
        float height = Level.size / Vector2.Distance(transform.position, hit.point) * renderDistanceMultiplier;
        height = Mathf.Clamp(height , .25f, 18);
        wall.transform.localScale = new Vector2(wallPrefab.transform.localScale.x, height);


        renderer.material.color = color * ((1/hit.distance) * fogMultiplier);
        renderer.sortingOrder = hit.collider.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        //Destroy(wall, Time.fixedUnscaledDeltaTime);
    }

    public void DrawRays3D()
    {
        for (int i = 0; i < RaycastCount; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, raysList[i].gameObject.transform.up, 9999, WhatIsWall);
            if (hit.collider != null)
            {
                if(hit.normal.x == 1 || hit.normal.x == -1)//pretty pointless but ill keep this is just in case
                {
                    Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, Vector2.Angle(transform.position, hit.point)) * Vector2.right);
                    RaycastHit2D _hit = Physics2D.Raycast(hit.point, dir, 9999, WhatIsWall);
                    DrawLine3D(hit, i, hit.collider.gameObject.GetComponent<SpriteRenderer>().color * shading); //left and right sides of the walls will be be a bit darker - smaller shading value => darker
                }
                else
                {

                    DrawLine3D(hit, i, hit.collider.gameObject.GetComponent<SpriteRenderer>().color);
                }
                Debug.DrawLine(transform.position, hit.point);

            }
        }
    }

    private void OnDamage()
    {
        Debug.Log("recieved damage");
        health -= 15;

        if (health <= 0) Die();
        else
        {
            asc.PlayAudio("hitSound");
            
        }


    }

    private void Die()
    {
        asc.PlayAudio("gameOver");
        Execute.ExecuteCommand("msg %username% ig its over for you");
    }

}

public class Level : MonoBehaviour
{
    public static int mapX = 16;
    public static int mapY = 16;
    public static int size = 256;
    static int[] map =
    {
        1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,//yes, this is how I made the map..
        1,3,0,0,0,0,0,0,0,0,0,0,0,0,0,1,//value "0" means that there will be nothing, dont make holes into the walls that are surrounding the map (breakes the game)
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,1,1,0,1,0,0,0,0,0,0,1,1,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,0,2,0,0,1,
        1,0,0,0,0,0,0,0,0,0,0,2,0,0,0,1,
        1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1,
        1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1,
        1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1,
        1,1,1,1,2,2,1,1,1,1,2,2,1,1,1,1
    };

    public static void DrawMap2D(GameObject sprite)
    {
        for (int y = 0; y < mapX; y++)
        {
            for (int x = 0; x < mapX; x++)
            {
                GameObject _sprite = Instantiate(sprite, PlayerController.instance.map);
                SpriteRenderer renderer = _sprite.GetComponent<SpriteRenderer>();
                _sprite.transform.position = new Vector2(x - mapX / 2, y - mapY / 2);

                switch(map[y * mapX + x])
                {
                    case 0:
                        //_sprite.gameObject.SetActive(false); 
                        Destroy(_sprite.gameObject);
                        break;
                    case 1:
                        renderer.color = Color.grey;
                        renderer.sortingOrder = 1;
                        break;

                    case 2:
                        renderer.color = Color.red;
                        renderer.sortingOrder = 2;
                        break;

                    case 3:
                        _sprite.AddComponent<Collidable>();
                        renderer.color = Color.cyan;
                        renderer.sortingOrder = 3;
                        break;
                }
            }
        }
    }
}
public class RayInfoObject //Each ray hodls the position of the line, dont remove 
{
    public GameObject gameObject;
    public float screenPosition;
}
