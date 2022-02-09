using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  public GameObject GravityDestinationGlow;

  private AudioSource[] whoosh = new AudioSource[3];
  private AudioSource plop;
  private Vector3 playerSpeed, temp;
  private static System.Random rng;
  private bool whooshing, hasPloped, onTop;
  private RaycastHit[] raycastHit;
  public static bool movingForward, movingRight, movingLeft;

  void Start()
  {
    playerSpeed = new Vector3(0,0,3f);
    movingForward = movingRight = movingLeft = true;
    //ray = new Ray();
    //raycastHit = new RaycastHit[1]; 
    GravityDestinationGlow = Instantiate(Resources.Load("GravityDestinationGlowPrefab") as GameObject);
    GravityDestinationGlow.name = "GravityDestinationGlow";
    GravityDestinationGlow.transform.position = new Vector3(0,0,-10f);
    rng = new System.Random();
    whooshing = false;
    hasPloped = false;
    onTop = true;
  }

  void Update()
  {
    if(GameManager.gameState == GameManager.GAMESTATES.PLAYING){
      updateGravityFlipDestinationGlow(); //cast ray across to indicate teleport location
      doMovement(); //thought about passing data in, however they are already global for other purposes. 
                    //Should i still pass them in as parameters or is it more efficient to just access the global variables?
      if(Input.GetKey(KeyCode.Space) && GameManager.gameState == GameManager.GAMESTATES.PLAYING && GameManager.seconds > 1 && Time.timeScale == 1){
        if(transform.position.y < -1.5f){
          reverseGravity(false);
          onTop = true;
        } else if(transform.position.y > 1.5f){
          reverseGravity(true);
          onTop = false;
        }
        if(!whooshing) {
          StartCoroutine(playWhoosh());
          whooshing = true;
        }
      }
    }
    else if(Input.GetKey(KeyCode.Space) && GameManager.gameState == GameManager.GAMESTATES.MENU){
      GameManager.instance.begin();  //startgame on spacebar press
    }
    if(hasPloped == false){
      if(transform.position.y < -1.65 && !onTop){
        playPlop();
      }else if(transform.position.y > 1.65 && onTop){
        playPlop();
      }
    }
  }

  private void updateGravityFlipDestinationGlow(){
    if(transform.position.y < -1.65){ //only update the position if currently not in air.
      temp.Set(transform.position.x, 2f,transform.position.z+2.5f); //the destination of the gravity flip is just the playerspeed in distance ahead at a set y
      GravityDestinationGlow.transform.position = temp; 
    } else if(transform.position.y > 1.65) { 
      temp.Set(transform.position.x, -2f,transform.position.z+2.5f);
      GravityDestinationGlow.transform.position = temp;
    }
  }

  //OLD IMPLEMENTATION OF TELEPORT GLOW AND TELEPORT
    // private void rayCastAcross(Ray ray, RaycastHit[] raycastHit){
    //   if(transform.position.y < 0){
    //     ray.direction = Vector3.up;
    //     ray.origin = transform.position;
    //     Physics.RaycastNonAlloc(ray, raycastHit, 5);
    //     TeleportGlow.transform.position = raycastHit[0].point + Vector3.down*.4f;
    //     TeleportGlow.transform.LookAt(raycastHit[0].point);
    //     if(Input.GetKey(KeyCode.Space))
    //       teleportPlayer(true, raycastHit);

    //   } else if(transform.position.y > 0) {
    //     ray.direction = Vector3.down;
    //     ray.origin = transform.position;
    //     Physics.RaycastNonAlloc(ray, raycastHit, 5);
    //     TeleportGlow.transform.position = raycastHit[0].point + Vector3.up*.4f;
    //     TeleportGlow.transform.LookAt(raycastHit[0].point);
    //     if(Input.GetKey(KeyCode.Space))
    //       teleportPlayer(false, raycastHit);
    //   }
    // }
 
  private void reverseGravity(bool onTop){
    Physics.gravity = (onTop) ? Vector3.down*8 : Vector3.up*8;
  }

  private void doMovement(){
    if(movingForward) {
      transform.position = (transform.position + playerSpeed*Time.deltaTime);
    }
    float horz = Input.GetAxis("Horizontal");
    if(horz > 0 && movingRight){
      transform.position = transform.position + (Vector3.right * 2.5f * Time.deltaTime);
    }else if(horz < 0 && movingLeft){
      transform.position = transform.position + (Vector3.right * -2.5f * Time.deltaTime);
    }
  }

  public void teleGlowOn(){
    GravityDestinationGlow.SetActive(true);
  }
  public void teleGlowOff(){
    GravityDestinationGlow.SetActive(false);
  }

  IEnumerator playWhoosh(){
    whoosh[rng.Next(3)].Play();
    yield return new WaitForSeconds(.7f);
    whooshing = false;
    hasPloped = false; //allowing the plop sound to be played after in the air
  }

  private void playPlop(){
    plop.Play();
    hasPloped = true;
  }

  public void setWoosh(AudioSource plop, AudioSource whoosh1, AudioSource whoosh2, AudioSource whoosh3){
    this.plop = plop;
    this.whoosh[0] = whoosh1;
    this.whoosh[1] = whoosh2;
    this.whoosh[2] = whoosh3;
  }

  public void setOnTop(bool onTop){
    this.onTop = onTop;
  }
}
