using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private const float OSCILLATION_MAGNITUDE = 80.0f;
    private const float OSCILLATION_RATE = 0.2f;
    private float currentTime = 0.0f;

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        transform.position = new Vector3(0.0f, 0.0f, OSCILLATION_MAGNITUDE * Mathf.Sin(OSCILLATION_RATE * currentTime));
    }
}
