using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingPlanes : MonoBehaviour {

  public Material mat1;
  public Material mat2;
  public Transform playerTransform;

  private const int VIEW_X = 120;
  private const int VIEW_Z = 80;


  void Update() {

    Plane plane1 = new Plane(Vector3.right, new Vector3(playerTransform.position.x + VIEW_X, 0, 0));
    Plane plane2 = new Plane(Vector3.forward, new Vector3(0, 0, playerTransform.position.z + VIEW_Z));
    Plane plane3 = new Plane(Vector3.left, new Vector3(playerTransform.position.x - VIEW_X, 0, 0));
    Plane plane4 = new Plane(Vector3.back, new Vector3(0, 0, playerTransform.position.z - VIEW_Z));

    Vector4 planeRepresentation1 = new Vector4(plane1.normal.x, plane1.normal.y, plane1.normal.z, plane1.distance);
    Vector4 planeRepresentation2 = new Vector4(plane2.normal.x, plane2.normal.y, plane2.normal.z, plane2.distance);
    Vector4 planeRepresentation3 = new Vector4(plane3.normal.x, plane3.normal.y, plane3.normal.z, plane3.distance);
    Vector4 planeRepresentation4 = new Vector4(plane4.normal.x, plane4.normal.y, plane4.normal.z, plane4.distance);




    mat1.SetVector("_Plane1", planeRepresentation1);              // <--  pass vector to shader
    mat1.SetVector("_Plane2", planeRepresentation2);
    mat1.SetVector("_Plane3", planeRepresentation3);
    mat1.SetVector("_Plane4", planeRepresentation4);
    mat2.SetVector("_Plane1", planeRepresentation1);              // <--  pass vector to shader
    mat2.SetVector("_Plane2", planeRepresentation2);
    mat2.SetVector("_Plane3", planeRepresentation3);
    mat2.SetVector("_Plane4", planeRepresentation4);


  }





}








