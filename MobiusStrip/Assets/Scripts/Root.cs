using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    private const int NUM_OF_ITEMS = 1250;

    [SerializeField] private GameObject terrainItem;



    private GameObject InstantiateItem(float posRadians)
    {
        Vector3 pos = new Vector3(Mathf.Cos(posRadians), Mathf.Sin(posRadians), 0f);
        pos = pos * WorldPhysics.GetTerrainRadius();
        GameObject res = Object.Instantiate(terrainItem, pos, Quaternion.identity);
        float degreesPos = RadToDeg(posRadians);
        res.transform.Rotate(new Vector3(Mathf.Cos(posRadians + Mathf.PI / 2f), Mathf.Sin(posRadians + Mathf.PI / 2f), 0f), degreesPos / 2f);
        return res;
    }


    private float RadToDeg(float radians)
    {
        return radians * 180f / Mathf.PI;
    }

    void Start()
    {
        float stepRadians = 2 * Mathf.PI / NUM_OF_ITEMS;
        for (int i = 0; i<NUM_OF_ITEMS; i++) InstantiateItem(i * stepRadians);
    }


}