﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class OcclusionObject : MonoBehaviour
{
    [SerializeField] private float displayTime = 0f;
    
    private Renderer _renderer;

    private void OnEnable()
    {
        _renderer = gameObject.GetComponent<Renderer>();
        displayTime = -1f;
    }

    private void Update()
    {
        if (displayTime > 0)
        {
            displayTime -= Time.deltaTime;
            _renderer.enabled = true;
        }
        else
        {
            _renderer.enabled = false;
        }
    }

    public void HitOcclude(float time)
    {
        displayTime = time;
        _renderer.enabled = true;
    }
}
