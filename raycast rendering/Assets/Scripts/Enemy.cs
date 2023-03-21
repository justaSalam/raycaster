//Most optimized enemy script
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public float maxDistance;
    public float speed;
    public float resetTime = 10;
    private bool canDamage;
    public int health;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.yellow;
        canDamage = true;
    }

    void Update() //a* from wish
    {
        if(Vector2.Distance(transform.position, player.transform.position) < maxDistance)
        {
            transform.LookAt(player.transform);
            transform.Translate(transform.forward * (speed / 1000));
        }

    }

    private void OnCollisionStay2D(Collision2D collision)//best way to do damage
    {
        if(collision.collider.gameObject == player && canDamage)
        {
            canDamage = false;
            PlayerController.onDamage?.Invoke();
            Invoke(nameof(ResetDamage), resetTime);
        }
    }
     
    public void RecieveDamage(int damage)
    {
        health -= damage;
        StartCoroutine(nameof(IE_ChangeColors));

        if (health <= 0)
        {
            AudioSourceController.Instance.PlayAudio("gameOver");
            Destroy(gameObject);
        }
    }

    private IEnumerator IE_ChangeColors()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(.1f);
        spriteRenderer.color = Color.yellow;
    }

    private void ResetDamage()
    {
        canDamage = true;
    }

}
