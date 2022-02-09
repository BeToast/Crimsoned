using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CubePoolManager {
  private static LinkedList<GameObject> cubePool = new LinkedList<GameObject>();
  private static GameObject CubePrefab = (GameObject)Resources.Load("CubePrefab");
  private static Material Blue = Resources.Load("Blue") as Material;
  public static GameObject getCubeFromPool() {
    GameObject go;
    if (cubePool.Count>0) {
      go = cubePool.First.Value;
      go.SetActive(true);
      cubePool.RemoveFirst();
    }
    else {
      go = GameObject.Instantiate(CubePrefab);
      go.name = "CubePrefab"; // get rid of "(Clone)"
    }
    return go;
  }
  public static void returnCubeToPool(GameObject go) {
    go.SetActive(false);  //inactive
    go.GetComponent<Renderer>().material = Blue; //back to blue
    cubePool.AddFirst(go); //back to list
  }
}