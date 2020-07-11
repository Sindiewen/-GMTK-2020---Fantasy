﻿using UnityEngine;

public class CamEarthquake : MonoBehaviour
{
    public float magnitude;
    private Vector3 originalPos = Vector3.zero;

    private void Start()
    {
        originalPos = Vector3.zero;
    }

    private void Update()
    {
        float x = Random.Range(-1, 1) * magnitude;
        float y = Random.Range(-1, 1) * magnitude;
        transform.localPosition = new Vector3(x, y, 0);
    }
}
