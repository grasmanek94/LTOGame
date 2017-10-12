using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class RandomStateExtension : MonoBehaviour {

    public static byte[] Save(System.Random random)
    {
        var binaryFormatter = new BinaryFormatter();
        using (var temp = new MemoryStream())
        {
            binaryFormatter.Serialize(temp, random);
            return temp.ToArray();
        }
    }

    public static System.Random Restore(byte[] state)
    {
        var binaryFormatter = new BinaryFormatter();
        using (var temp = new MemoryStream(state))
        {
            return (System.Random)binaryFormatter.Deserialize(temp);
        }
    }
}
