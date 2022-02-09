using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public static Material Crimson, Blue;
    // Start is called before the first frame update
    private Renderer rend;
    void Start()
    {
      if(!Crimson)
        Crimson = Resources.Load("Crimson") as Material;
      if(!Blue)
        Blue = Resources.Load("Blue") as Material;
      rend = GetComponent<Renderer>();
      rend.material = Blue;
    }

  void OnCollisionEnter(Collision collision)
  {
    if(collision.gameObject.tag == "Player")
    {
      this.setRed();
      GameManager.countCrimson++;
      GameManager.countBlue--;
    }
  }

  public void setBlue(){
    rend.material = Blue;
  }

  public void setRed(){
    rend.material = Crimson;
  }
}
