using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeftTrigger : MonoBehaviour
{
  void Start()
  {
        
  }
  private void OnTriggerEnter(Collider other){
    Player.movingLeft = false;
  }
  private void OnTriggerExit(Collider other){
    Player.movingLeft = true;
  }
}