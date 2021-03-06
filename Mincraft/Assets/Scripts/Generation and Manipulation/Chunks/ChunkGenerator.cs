﻿using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Perlin Noise")]
    [SerializeField] private float smoothness = 0.03f;
    [SerializeField] private float heightMult = 5f;
    
    [Header("Instantiation")]
    [SerializeField] private BlockUV surface = default;
    [SerializeField] private BlockUV bottom = default;
    [SerializeField] private Vector3Int size = default;
    
    private Biom[] bioms;
    private ChunkManager chunkManager;
    private BlockPool pool;

    
    private void Start()
    {
        pool = BlockPool.Instance;
        chunkManager = ChunkManager.Instance;

        List<Vector3Int> surfacePositions = GenerateHeightMap(size, (x, z) =>
        {
            float height = (Mathf.PerlinNoise(x * smoothness, z * smoothness * 2) * heightMult + 
                            Mathf.PerlinNoise(x * smoothness, z * smoothness * 2) * heightMult) / 2f;
            
            return Mathf.CeilToInt(height);
        });
        
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        foreach (Vector3Int surfacePosition in surfacePositions)
        {
            Block block = new Block(surfacePosition);
            block.UVSetter.SetBlockUV(surface);
            chunkManager.AddBlock(block);
        }

        // Optimierungsbedarf
        // Entweder erst Blöcke alle in den Chunk einfügen, dann einmal, aber viel combinen
        // und / oder Multithreading!!!! MÖGLICH DURCH BLOCK
        
        List<Vector3Int> rofl = GenerateBottomMap(surfacePositions);
        List<Block> temp2 = new List<Block>();
        foreach (Vector3Int bottomPosition in rofl)
        {
            Block block = new Block(bottomPosition);
            block.UVSetter.SetBlockUV(bottom);
            chunkManager.AddBlock(block);
        }
        watch.Stop();
        Debug.Log(watch.ElapsedMilliseconds);
    }

    private List<Vector3Int> GenerateBottomMap(List<Vector3Int> surfacePositions)
    {
        List<Vector3Int> list = new List<Vector3Int>();
        
        for (int i = 0; i < surfacePositions.Count; i++)
        {
            int heightDelta = surfacePositions[i].y - (-size.y);

            for (int j = 1; j < heightDelta; j++)
            {
                list.Add(new Vector3Int(surfacePositions[i].x, surfacePositions[i].y - j, surfacePositions[i].z));
            }
        }

        return list;
    }
    
    private List<Vector3Int> GenerateHeightMap(Vector3Int size, Func<int, int, int> heightFunc)
    {
        bool firstHeightSet = false;
        int firstHeight = int.MaxValue;
        
        List<Vector3Int> positions = new List<Vector3Int>();

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z <= size.z; z++)
            {
                int y = heightFunc(x, z);

                if (!firstHeightSet)
                {
                    firstHeightSet = true;
                    firstHeight = y;
                }
                
                positions.Add(new Vector3Int(x, y - firstHeight, z));
            }
        }
        
        
        return positions;
    }
}
