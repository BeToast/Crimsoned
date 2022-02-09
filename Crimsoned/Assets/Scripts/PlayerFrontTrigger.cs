using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFrontTrigger : MonoBehaviour
{
  void Start()
  {
        
  }
  private void OnTriggerEnter(Collider other){
    Player.movingForward = false;
  }
  private void OnTriggerExit(Collider other){
    Player.movingForward = true;
  }
}
