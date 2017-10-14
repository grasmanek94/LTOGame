using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionOffsets : MonoBehaviour
{
    // 0 shall be entry point (exit in case of 180 degree turn), 1 shall be forward / exit point (entry in case of 180 degree turn), 2 left, 3 right
    public Vector3[] position_offsets;
    public Vector3[] rotation_offsets;
    public GameObject[] taken; 

    private void DisconnectMyselfFrom(int taken_slot)
    {
        GameObject other_go = taken[taken_slot];
        if (other_go != null)
        {
            ConnectionOffsets other = other_go.GetComponent<ConnectionOffsets>();
            for (int j = 0; j < other.taken.Length; ++j)
            {
                if (other.taken[j] == this)
                {
                    other.taken[j] = null;
                    break;
                }
            }
            taken[taken_slot] = null;
        }
    }

    public void ResetTaken()
    {
        int len = taken.Length;
        for(int i = 0; i < len; ++i)
        {
            DisconnectMyselfFrom(i);
        }
    }
}
