using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{
public enum GAMESTATES { 
    MENU, PLAYING 
  }
  public static GAMESTATES gameState = GAMESTATES.MENU;
  List<GameObject> startCubeList;
  public GameObject Audio, Player, MenuCanvas, GameCanvas, PauseCanvas;
  public AudioSource plop, whoosh1, whoosh2, whoosh3;
   public static TMPro.TMP_Text Crimson, Timer, Score, Highscore, PreviousScore, MenuHighscore;
  public static GameManager instance; 
  public Queue<Row>[] rowQueueArray;
  public static Vector3 temp;
  private Ray ray;
  private RaycastHit hitFromRay;
  protected Row startRow;
  private static bool newHighscore;
  public static int currentOldestRow, countCrimson, countBlue, score, previousScore, highscore;
  public static float timer, minutes, seconds, milliseconds;
  private static string scoreText;

  void Start()
  {
    Row referenceRow = new Row();
    referenceRow.setRng(); //so we can call setRng(); i think it has a finite number of numbers so we have to call it each new game.
    //using C# rng not unity rng
    instance = this;
    Camera.main.fieldOfView = 100f;
    this.rowQueueArray = new Queue<Row>[2] {new Queue<Row>(), new Queue<Row>()};
    startCubeList = new List<GameObject>();
    highscore = PlayerPrefs.GetInt("highscore", 0); //get stored highscore
    countBlue = 0;
    countCrimson = 0;
    newHighscore = false;
    scoreText = null;
    Camera.main.transform.position = new Vector3 (0, 0, -8);
    //once only 
    Camera.main.transform.LookAt (new Vector3(0, 0, 50), new Vector3 (0, 1, 0)); 
    //whenever gamestate changes
    instance.MenuCanvas.SetActive(true);
    instance.GameCanvas.SetActive(false);
    instance.PauseCanvas.SetActive(false);
    createSpawnCubes();
    StartCoroutine(generateTerrain(50, 0.05f));
    //player instantiation
    Player = Instantiate(Resources.Load("PlayerPrefab") as GameObject);
    Player.name = "Player";
    temp.Set(0,-1.5f,-5);
    Player.transform.position = temp;
    Player.GetComponent<Player>().setWoosh(plop, whoosh1, whoosh2, whoosh3);

    //ORIGIONAL LIGHT SOURCES.
      //light instantiacion
      // LightSource1 = Instantiate(Resources.Load("LightSource") as GameObject);
      // LightSource1.name = "LightSource1";
      // temp.Set(4,0,Player.transform.position.z - 3f);
      // LightSource1.transform.position = temp;
      // LightSource1.transform.LookAt (new Vector3(-8, 0, 8), new Vector3 (0, 1, 0));
      // LightSource2 = Instantiate(Resources.Load("LightSource") as GameObject);
      // LightSource2.name = "LightSource2";
      // temp.Set(-4,0,Player.transform.position.z - 3f);
      // LightSource2.transform.position = temp;
      // LightSource2.transform.LookAt (new Vector3(8, 0, 8), new Vector3 (0, 1, 0));

    //game canvas text fields
    Crimson = instance.GameCanvas.transform.Find("Crimson").GetComponent<TMPro.TMP_Text>();
    Crimson.text = "Remaining Blue: 000";
    Timer = instance.GameCanvas.transform.Find("Timer").GetComponent<TMPro.TMP_Text>();
    Timer.text = "00:00:00";
    Highscore = instance.GameCanvas.transform.Find("Highscore").GetComponent<TMPro.TMP_Text>();
    Highscore.text = "Highscore: "+highscore.ToString().PadLeft(4, '0');
    Score = instance.GameCanvas.transform.Find("Score").GetComponent<TMPro.TMP_Text>();
    Score.text = "Score: 0000";
    //lobby text
    MenuHighscore = instance.MenuCanvas.transform.Find("menuHighscore").GetComponent<TMPro.TMP_Text>();
    MenuHighscore.text = "Highscore: "+highscore.ToString().PadLeft(4, '0');
    PreviousScore = instance.MenuCanvas.transform.Find("previousScore").GetComponent<TMPro.TMP_Text>();

    //these do run even if the game isint running but its okay because they are not much overhead and that doesnt really matter in the menu.
    InvokeRepeating("rowDeletionChecker", 3f, 0.1f);    
    InvokeRepeating("checkIfNewHighscore", 1f, 1f);
    InvokeRepeating("audioToCamera", 1f, 1f); 
  }

  public void reset(){
    gameState = GAMESTATES.MENU;
    Player.GetComponent<Player>().teleGlowOff();
    clearBlockQueue(rowQueueArray);
    countBlue = 0;
    countCrimson = 0;
    previousScore = score;
    score = 0;
    newHighscore = false; 
    Camera.main.transform.position = new Vector3 (0, 0, -8);
    resetSpawnCubes(); //just turns them blue again really 
    temp.Set(0,-1.5f,-5);
    Player.transform.position = temp;
    Player.GetComponent<Player>().setOnTop(true);
    MenuHighscore.text = "Highscore: "+highscore.ToString().PadLeft(4, '0');
    PreviousScore.text = "Previous Score: "+previousScore.ToString().PadLeft(4, '0');
    instance.MenuCanvas.SetActive(true);
    instance.GameCanvas.SetActive(false);
    Physics.gravity = Vector3.down*8;
    Row.totalRows = 0;
    StartCoroutine(generateTerrain(50, 0.05f));
    newHighscore = false;
    PlayerPrefs.SetInt("highscore", highscore); //saves highscore :)
    PlayerPrefs.Save();
  }

  //MENU BUTTON METHODS
  public void begin(){ 
    currentOldestRow = 0;
    instance.MenuCanvas.SetActive(false);
    gameState = GAMESTATES.PLAYING;
    timer = 0;
    instance.GameCanvas.SetActive(true);
    Player.GetComponent<Player>().teleGlowOn();
  }
  public void options(){
    Debug.Log("options");
  }
  public void scoreboard(){
    Debug.Log("scoreboard");
  }

  // Update is called once per frame
  void Update()
  {
    if(gameState == GAMESTATES.PLAYING){
      temp.Set(0,0,Player.transform.position.z - 2f);
      Camera.main.transform.position = temp;
      updateTimer();
      updateRemainingBlue();
      updateScore();
      if(newHighscore) updateHighscore();

      if(Player.transform.position.y < -3.5 || Player.transform.position.y > 3.5){ //this means they are dead.
        Debug.Log("you died :P");
        reset();
      }
      if(Input.GetKeyDown(KeyCode.Escape)){ //pausing logic
        if(Time.timeScale == 1) {pauseGame();}
        else {resumeGame();}
      }
      //added resuming with spacebar but its not good for gameplay because you are already holding space as soon as the game begins again.
      // if(Input.GetKey(KeyCode.Space) && Time.timeScale == 0){
      //   resumeGame();
      // }
    }
  }
  public void pauseGame(){
    Time.timeScale = 0;
    instance.PauseCanvas.SetActive(true);
  }
  public void resumeGame(){
    
    instance.PauseCanvas.SetActive(false);
    Time.timeScale = 1;
  }
  IEnumerator generateTerrain(int rowsToMake, float delay){ //put a delay between each row generation to make a cool affect using Coroutine
    while(Row.totalRows<rowsToMake){
      this.rowQueueArray[0].Enqueue(RowPoolManager.getRowFromPool().setRowVariables(true));
      this.rowQueueArray[1].Enqueue(RowPoolManager.getRowFromPool().setRowVariables(false));
      Row.totalRows++;
      yield return new WaitForSeconds(delay);
    }
  }
 
  private void rowDeletionChecker(){
    if(Camera.main.transform.position.z > currentOldestRow+2){
      currentOldestRow++;
      this.rowQueueArray[0].Enqueue((this.rowQueueArray[0].Dequeue()).reuseRow());
      this.rowQueueArray[1].Enqueue((this.rowQueueArray[1].Dequeue()).reuseRow());
    }
  }

  private void audioToCamera(){
    Audio.transform.position = Camera.main.transform.position;
  }

  private void checkIfNewHighscore(){
    if(score > highscore) newHighscore = true;
  }

  private void updateRemainingBlue(){
    Crimson.text = "Blue Remaining: " +countBlue.ToString().PadLeft(3, '0');
  }
  private void updateTimer(){
    timer += Time.deltaTime;
    minutes = timer/60;
    seconds = timer%60;
    milliseconds = (int)((timer*100)%100);
    Timer.text = minutes.ToString("00")+ ":" +seconds.ToString("00")+ ":" +milliseconds.ToString("00");
  }

  private void updateScore(){
    score = countCrimson * (int)timer;
    scoreText = score.ToString().PadLeft(4, '0'); //stored to be used in updateHighscore();
    Score.text = "Score: " + score.ToString().PadLeft(4, '0');
  }

  private void updateHighscore(){
    Highscore.text = "Highscore: " + scoreText;
    highscore = score;
  }

  public void clearBlockQueue(Queue<Row>[] rowQueue){
    foreach(Queue<Row> queue in rowQueue){
      foreach(Row r in queue){
        RowPoolManager.returnRowToPool(r);
      }
      queue.Clear();
    }
  }

  private void resetSpawnCubes(){
    foreach(GameObject go in startRow.cubeList){
      go.GetComponent<Cube>().setBlue();
    }
  }

  private void createSpawnCubes(){
    for(int i=-5; i<0; i++){
      GameObject cube = CubePoolManager.getCubeFromPool();
      temp.Set(0, -2.5f, i);
      cube.transform.position = temp;
      startCubeList.Add(cube);
    }
    startRow = new Row(startCubeList);
  }

  public void quitGame(){
    Debug.Log("quit");
    Application.Quit();
  }
  void OnApplicationQuit(){
    PlayerPrefs.SetInt("highscore", highscore); //saves highscore :)
    //PlayerPrefs.Save(); // not needed as unity is supposed to automatically do this.
  }
}
