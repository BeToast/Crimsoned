using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRightTrigger : MonoBehaviour
{
  void Start()
  {
        
  }
  private void OnTriggerEnter(Collider other){
    Player.movingRight = false;
  }
  private void OnTriggerExit(Collider other){
    Player.movingRight = true;
  }
}