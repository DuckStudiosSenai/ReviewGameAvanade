using UnityEngine;
using Photon.Pun;

public class NPCSpawner : MonoBehaviourPun
{
    public GameObject npcPrefab;
    public float spawnDistance = 10f;

    private Transform player;
    private Camera mainCam;
    private GameObject activeNpc;

    void Start()
    {
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        player = transform;
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
        if (activeNpc == null) return;
        activeNpc.GetComponent<NPCController>().SendAway();
    }

    public GameObject GetActiveNPC()
    {
        return activeNpc;
    }
}
