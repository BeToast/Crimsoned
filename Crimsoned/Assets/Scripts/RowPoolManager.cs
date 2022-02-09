using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RowPoolManager {
  private static LinkedList<Row> rowPool = new LinkedList<Row>();

  public static Row getRowFromPool() {
    Row row;
    if (rowPool.Count>0) {
      row = rowPool.First.Value;
      for(int i = row.cubeList.Count; i < 9; i++){  //add cubes if the row is cleared of its cubeList.
        row.cubeList.Add(CubePoolManager.getCubeFromPool());
      }
      rowPool.RemoveFirst();
    }
    else {
      row = new Row();
      for(int i=0; i<9; i++){
        row.cubeList.Add(CubePoolManager.getCubeFromPool());
      }
    }
    return row;
  }

  public static void returnRowToPool(Row row){
    row.setNumCubes(0); //previously, i wasnt resetting these and my scoring was all off and it was very difficult to figure out why.
    row.setNumCrimson(0);
    foreach(GameObject cube in row.cubeList){
      CubePoolManager.returnCubeToPool(cube);
    }
    row.cubeList.Clear();
    rowPool.AddFirst(row);
  }
}