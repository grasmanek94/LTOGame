using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour {

    void Start()
    {
        Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();
        poly.outside = new List<Vector3>() {
            /*new Vector3(0,-0.33f,0),
            new Vector3(1,0.33f,0),
            new Vector3(1,0.33f,1),
            new Vector3(2,0.33f,1),
            new Vector3(2,-0.33f,2),
            new Vector3(0.5f,-0.33f,1.5f)*/
            new Vector3(0,-1,0),
            new Vector3(1,1,0),
            new Vector3(1,-1,10),
            new Vector3(0,1,10)
        };
        /*poly.holes.Add(new List<Vector3>() {
            new Vector3(60,110,110),
            new Vector3(90,110,110),
            new Vector3(90,140,140),
            new Vector3(60,140,140),
        });*/

        // Set up game object with mesh;
        GameObject go = Poly2Mesh.CreateGameObject(poly);
        go.transform.Rotate(0.0f, 0.0f, 180.0f);
        go.AddComponent<MeshCollider>();
        Material newMat = Resources.Load("NavyGrid", typeof(Material)) as Material;
        go.GetComponent<Renderer>().material = newMat;
    }
}
