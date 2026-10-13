using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBall : MonoBehaviour {

    public float speed = 20f;
    public int damage = 10;
    public float time = 0.5f;
    public Rigidbody2D rb;
    
    private GameObject player;
    private GameObject boss;
    private bool hasCustomDirection;
    private Vector2 customDirection;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        boss = GameObject.FindGameObjectWithTag("Boss");

        // Direction to which the ball will be thrown
        Vector2 direction = hasCustomDirection
            ? customDirection
            : (Vector2)(player.transform.position - boss.transform.position);

        // Throw the ball
        Debug.Log("Ball thrown");
        rb.linearVelocity = direction * speed;
    }

    void Update() {
        // Delete the object after a certain amount of time has passed
        Object.Destroy(gameObject, 2f);
    }

    void OnTriggerEnter2D(Collider2D collision) {

        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponentInParent<Stats>().takeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void setTransform(GameObject boss) {
        this.boss = boss;
    }

    public void SetDirection(Vector2 direction)
    {
        customDirection = direction.normalized;
        hasCustomDirection = true;
    }
}
