using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionOffsets : MonoBehaviour
{
    public Vector3[] position_offsets;
    public Vector3[] rotation_offsets;
    public bool[] taken;

    private void ResetTaken()
    {
        int len = taken.Length;
        for(int i = 0; i < len; ++i)
        {
            taken[i] = false;
        }
    }
}
