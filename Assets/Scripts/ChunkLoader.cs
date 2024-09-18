using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public GameObject chunkPrefab;

    public int totalChunks = 7;
    public int chunkSize = 200;

    public Transform player;
    public static float playerPositionZ;

    Queue<GameObject> chunkPool = new Queue<GameObject>();
    ListNode activeChunksHead;
    ListNode activeChunksTail;
    int activeCount = 0;

    float nextForwardSpawnPosition;
    float nextBackwardSpawnPosition;

    void Start()
    { 
        FloatingOriginController.OnFloatingOrigin += UpdateChunkPosition;
        InitializeChunkPool();
    }

    void Update()
    {
        playerPositionZ = player.position.z;

        UpdateNextChunk();
    }

    void OnDestroy()
    {
        FloatingOriginController.OnFloatingOrigin -= UpdateChunkPosition;
    }

    void InitializeChunkPool()
    {
        for(int i = 0; i < totalChunks; i++)
        { 
            GameObject newChunk = SpawnChunk(nextForwardSpawnPosition);
            AddToFront(new ListNode(ref newChunk));
            nextForwardSpawnPosition += chunkSize;
        }

        nextBackwardSpawnPosition = activeChunksTail.chunk.transform.position.z - chunkSize;
    }

    void UpdateNextChunk()
    {
        //Add new chunks
        if(nextForwardSpawnPosition - playerPositionZ < chunkSize * 3)
        {
            GameObject newChunk = SpawnChunk(nextForwardSpawnPosition);
            AddToFront(new ListNode(ref newChunk));
            nextForwardSpawnPosition += chunkSize;
        }
        if(playerPositionZ - nextBackwardSpawnPosition < chunkSize * 3)
        {
            GameObject newChunk = SpawnChunk(nextBackwardSpawnPosition);
            AddToBack(new ListNode(ref newChunk));
            nextBackwardSpawnPosition -= chunkSize;
        }
                
        //Remove old chunks
        if(activeCount > totalChunks)
        {
            if (activeChunksHead.chunk.transform.position.z - playerPositionZ > chunkSize * 4) 
            {
                RemoveFromFront();
                nextForwardSpawnPosition -= chunkSize;
            }
            else if (playerPositionZ - activeChunksTail.chunk.transform.position.z > chunkSize * 4)
            {
                RemoveFromBack();
                nextBackwardSpawnPosition += chunkSize;
            }
        }
    }

    GameObject SpawnChunk(float newPosition)
    {
        GameObject chunk;
        if(chunkPool.Count > 0)
        {
            chunk = chunkPool.Dequeue();
            chunk.SetActive(true);
        }
        else
        {
            chunk = Instantiate(chunkPrefab, transform);
        }

        chunk.transform.position = new Vector3(0, 0, newPosition);
        return chunk;
    }

    void UpdateChunkPosition(float reference)
    {
        ListNode ptr = activeChunksTail;
        while (ptr != null)
        {
            ptr.chunk.transform.position = new Vector3(0, 0, ptr.chunk.transform.position.z - reference);
            ptr = ptr.next;
        }
        nextForwardSpawnPosition -= reference;
        nextBackwardSpawnPosition -= reference;
    }

    void AddToFront(ListNode newNode)
    {
        if (activeChunksHead == null)
        {
            activeChunksHead = newNode;
            activeChunksTail = newNode;
        }
        else
        {
            activeChunksHead.next = newNode;
            newNode.prev = activeChunksHead;

            activeChunksHead = newNode;
        }
        activeCount++;
    }

    void AddToBack(ListNode newNode)
    {
        if (activeChunksTail == null)
        {
            activeChunksHead = newNode;
            activeChunksTail = newNode;
        }
        else
        {
            activeChunksTail.prev = newNode;
            newNode.next = activeChunksTail;

            activeChunksTail = newNode;
        }
        activeCount++;
    }

    void RemoveFromFront()
    {
        if (activeChunksHead != null)
        {
            ListNode newHead = activeChunksHead.prev;

            activeChunksHead.chunk.SetActive(false);
            chunkPool.Enqueue(activeChunksHead.chunk);

            activeChunksHead = newHead;
        }
        activeCount--;
    }

    void RemoveFromBack()
    {
        if (activeChunksHead != null)
        {
            ListNode newTail = activeChunksTail.next;

            activeChunksTail.chunk.SetActive(false);
            chunkPool.Enqueue(activeChunksTail.chunk);

            activeChunksTail = newTail;
        }
        activeCount--;
    }

    public float GetFarthestForwardZ()
    {
        return activeChunksHead.prev.chunk.transform.position.z;
    }
    public float GetFarthestBackwardZ()
    {
        return activeChunksTail.next.chunk.transform.position.z;
    }

    public class ListNode
    {
        public GameObject chunk;
        public ListNode next;
        public ListNode prev;

        public ListNode(ref GameObject newChunk)
        {
            chunk = newChunk;
        }
    }
}