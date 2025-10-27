using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform player;
    public float spawnDistance = 10f;

    private Camera mainCam;
    private GameObject activeNpc;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {

        if (activeNpc == null)
            activeNpc = null;
    }

    public void SpawnAndChase()
    {
        Vector3 spawnPos = GetSpawnPositionOutsideCamera();
        activeNpc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);
        activeNpc.GetComponent<NPCController>().player = player;
    }

    Vector3 GetSpawnPositionOutsideCamera()
    {
        Vector3 camPos = mainCam.transform.position;
        Vector3 spawnPos = camPos;

        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: spawnPos += Vector3.up * spawnDistance; break;
            case 1: spawnPos += Vector3.down * spawnDistance; break;
            case 2: spawnPos += Vector3.left * spawnDistance; break;
            case 3: spawnPos += Vector3.right * spawnDistance; break;
        }

        spawnPos.z = 0;
        return spawnPos;
    }

    public void SendNPCAway()
    {
        NPCController npc = GameObject.FindAnyObjectByType<NPCController>();
        
        if (npc == null) return;
        
        npc.SendAway();
    }

    public GameObject GetActiveNPC()
    {
        return activeNpc;
    }
}
