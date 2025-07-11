using UnityEngine;
using UnityEngine.Tilemaps;


public class EnemyController : MonoBehaviour
{
    public float max_speed = 1.0f;
    public float acceleration = 2.0f;
    public float friction = 0.5f;
    public float knockback_from_enemy = 10f;
    public float knockback_from_player = 10f;
    public float enemy_collision_distance = 0.3f;
    public float player_collision_distance = 1f;
    public Tile trail_tile;

    private Vector3 velocity;
    public Vector3 ext_velocity;
    private PlayerController player;
    private MapManager map_manager;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3(0f, 0f);
        ext_velocity = new Vector3(0f, 0f);

        map_manager = GameObject.FindGameObjectWithTag("Map").GetComponent<MapManager>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (player != null)
        {
            Debug.Log("Player found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        velocity += acceleration * Time.fixedDeltaTime * direction;
        velocity = Vector3.ClampMagnitude(velocity, max_speed);

        ext_velocity -= friction * Time.fixedDeltaTime * ext_velocity;
        transform.Translate(Time.fixedDeltaTime * (velocity + ext_velocity));
    }

    private void on_death()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject target = collision.gameObject;
        if (target.CompareTag("Player"))
        {
            Debug.Log("Player takes damage! " + collision.gameObject.name);
            PlayerController player = target.GetComponent<PlayerController>();
            player.deal_damage(1, transform.position);
        }
        else if (target.CompareTag("Attack"))
        {
            on_death();
        }
        else
        {
            Debug.Log("Enemy " + name + " colliding with " + target.name);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject target = collision.gameObject;
        Vector3 target_to_enemy = transform.position - target.transform.position;
        Vector3 direction = target_to_enemy.normalized;

        if (target.CompareTag("Enemy") && target_to_enemy.magnitude < enemy_collision_distance)
        {
            Vector2 rand_dir = Random.insideUnitCircle;
            direction += new Vector3(rand_dir.x, rand_dir.y);
            velocity += knockback_from_enemy * Time.fixedDeltaTime * direction;
        }
        else if (target.CompareTag("Player") && target_to_enemy.magnitude < player_collision_distance)
        {
            velocity += knockback_from_player * Time.fixedDeltaTime * direction;
        }
        else if (target.CompareTag("Wellspring"))
        {
            Vector3 closest_point = collision.ClosestPoint(transform.position);
            Vector3 normal = (transform.position - closest_point).normalized;
            Vector3 tangent = new Vector3(-normal.y, normal.x);
            velocity = Vector3.Dot(velocity, tangent) * tangent + knockback_from_enemy * Time.fixedDeltaTime * direction;
        }
    }
}
