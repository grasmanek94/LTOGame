using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionOffsets : MonoBehaviour
{
    // 0 shall be entry point (exit in case of 180 degree turn), 1 shall be forward / exit point (entry in case of 180 degree turn), 2 left, 3 right
    public Vector3[] position_offsets;
    public Vector3[] rotation_offsets;
    public GameObject[] taken; 

    public void ResetTaken()
    {
        int len = taken.Length;
        for(int i = 0; i < len; ++i)
        {
            taken[i] = null;
        }
    }
}
