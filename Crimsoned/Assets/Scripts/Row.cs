using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row {
  public List<GameObject> cubeList;
  public static int totalRows = 0;
  private static Vector3 nextCubePosition, temp; //it is not nessicary to store this data statically, however, we only create one Vector3 in the heap this way.
  private static System.Random rng;
  private int numCubes = 0, numCrimson = 0;

  public Row(){ //used to create an empty row
    cubeList = new List<GameObject>(); //not using pool. just for startup
  }
  public Row(List<GameObject> list){ //Overload for creating a custom row
  cubeList = new List<GameObject>(); //we do not use the pool for this so we can create rows of different lengths. this is only called once on startup.
    foreach(GameObject cube in list){
      cubeList.Add(cube);
      numCubes++;
    }
    GameManager.countBlue += numCubes;
  }

  public Row setRowVariables(bool top){
    //nessicary variables
    float y; 
    int nextCubeX = -4;
    nextCubePosition = new Vector3(0,0,0);
    y = (top ? 2.5f : -2.5f); //set variables if it is a top or bottom row.
    foreach(GameObject cube in cubeList){
      nextCubePosition.Set(nextCubeX, y, totalRows);
      cube.transform.position = nextCubePosition;
      cube.transform.Rotate((top ? 90f : -90f), 0, 0, Space.Self); //rotate the cubes properly so the face the centre
      if(Random.Range(0,4)<2){  //cube probability
        cube.SetActive(true);
        numCubes++;
      }else{
        cube.SetActive(false);
      }
      nextCubeX++;
    }
    GameManager.countBlue += numCubes;  //count the total number of cubes for scoring purposes
    return this;
  }

  public Row reuseRow(){
    this.Shuffle();
    float xCoord = -4;
    foreach(GameObject cube in cubeList){
      if(cube.GetComponent<Renderer>().material.name == "Crimson (Instance)"){
        if(rng.Next(2) == 0){ //removes crimson half the time.
          cube.SetActive(false);
        }
      }
      temp.Set(xCoord, cube.transform.position.y, cube.transform.position.z + totalRows);
      cube.transform.position = temp;
      xCoord++;
    }
    return this;
  }

  private void Shuffle()  
  {  
    int n = 9;
    while (n > 1) {  
      n--;  
      int k = rng.Next(n + 1);  
      GameObject cube = cubeList[k];  
      cubeList[k] = cubeList[n];  
      cubeList[n] = cube;  
    }  
  } 

  public List<GameObject> getCubeList(){
    return cubeList;
  }

  public void setCubeList(List<GameObject> cubeList){
    this.cubeList = cubeList;
  }

  public void setRng(){
    rng = new System.Random();
  }

  public void setNumCubes(int numCubes){
    this.numCubes = numCubes;
  }

  public void setNumCrimson(int numCrimson){
    this.numCrimson = numCrimson;
  }

  // Origional Row constructor.
  // public Row(bool top){
  //   //nessicary variables
  //   float y, yIdk; 
  //   int nextCubeX = -4;
  //   nextCubePosition = new Vector3(0,0,0);

  //   if(top) { y=2f; yIdk= 0.5f; } else { y=-2f; yIdk=-0.5f;} //set variables if it is a top or bottom row.
  //   cubeList = RowPoolManager.getRowFromPool();
  //   foreach(GameObject cube in cubeList){
  //     nextCubePosition.Set(nextCubeX, ((Random.Range(0,4)==1) ? y : y+yIdk), totalRows);
  //     cube.transform.position = nextCubePosition;
  //     //cubeList.AddFirst(cube);
  //     nextCubeX++;
  //   }
  // }
}

