﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Transform begin;
    public Transform end;
    public Transform circleCenter;
    public int roadRotation;
    public AnimationCurve chanceFromDistance;
    private KnifeController _knifeController;
    private ChunkPlacer _chunkPlacer;

    void Start()
    {
        _chunkPlacer = GameObject.Find("ChunkPlacer").GetComponent<ChunkPlacer>();
        _knifeController = GameObject.Find("Knife").GetComponent<KnifeController>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player"&& roadRotation!=0)
        {
            BoxCollider boxCollider= this.GetComponent<BoxCollider>();
            boxCollider.enabled = false;
            _knifeController.changeDirection(roadRotation, begin,end);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            _chunkPlacer.setTraversedChunks();
        }
    }
}
