using UnityEngine;

public class WireSpawner : MonoBehaviour
{
    public GameObject wirePrefab;
    public float baseSpawnRate = 2f;
    public float baseWireSpeed = 6f;
    public float heightVar = 3f;

    private float nextSpawnTime;

    void Start() { nextSpawnTime = Time.unscaledTime + 4f; }

    void Update()
    {
        if (MiniGame3Manager.Instance != null && Time.unscaledTime >= nextSpawnTime)
        {
            if (MiniGame3Manager.Instance.gameStarted && Time.timeScale > 0) 
            {
                SpawnLogic();
                float diff = MiniGame3Manager.Instance.GetCurrentDifficultyMultiplier();
                nextSpawnTime = Time.unscaledTime + (baseSpawnRate / (diff > 0 ? diff : 1f));
            }
        }
    }

    void SpawnLogic()
    {
        float diff = MiniGame3Manager.Instance.GetCurrentDifficultyMultiplier();
        if (diff <= 0) diff = 1f;

        SpawnWire(new Vector3(12, Random.Range(-heightVar, heightVar), 0), Vector3.left, baseWireSpeed * diff);

        if (MiniGame3Manager.Instance.IsDualSideActive())
            SpawnWire(new Vector3(-12, Random.Range(-heightVar, heightVar), 0), Vector3.right, baseWireSpeed * diff);
    }

    void SpawnWire(Vector3 pos, Vector3 dir, float speed)
    {
        Debug.Log("⚡ SPAWNING WIRE AT: " + pos);
        GameObject wire = Instantiate(wirePrefab, pos, Quaternion.identity);
        if (dir == Vector3.right) wire.transform.localScale = new Vector3(-1, 1, 1);

        MoveLeft moveScript = wire.AddComponent<MoveLeft>();
        moveScript.direction = dir;
        moveScript.speed = speed;
        Destroy(wire, 6f);
    }
}

public class MoveLeft : MonoBehaviour 
{
    public Vector3 direction = Vector3.left;
    public float speed;
    void Update() => transform.Translate(direction * speed * Time.deltaTime);
}