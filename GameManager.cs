using UnityEngine;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
using UnityEngine.UI;

namespace Completed
{
	

	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
		public float turnDelay = 0.1f;
		public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
		public BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
		public int playerFoodPoints = 100;
		[HideInInspector] public bool playersTurn = true;

		private Text levelText;
		private GameObject levelImage;
		private int level;                                  //Current level number, expressed in game as "Day 1".
		private List<Enemy> enemies;
		private bool enemiesMoving;
		private bool doingSetup;

		//Awake is always called before any Start functions
		void Awake()
		{
			//Check if instance already exists
			if (instance == null) {

				//if not, set instance to this
				instance = this;

				//If instance already exists and it's not this:
			} else if (instance != this) {

				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
				Destroy (gameObject);    
			}
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);

			enemies = new List<Enemy> ();

			//Get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();


			//Call the InitGame function to initialize the first level 
			//InitGame();
		}

		void OnLevelWasLoaded(int index)
		{
			//Add one to our level number.
			level++;
			//Call InitGame to initialize our level.
			InitGame();
		}


		//Initializes the game for each level.
		void InitGame()
		{
			doingSetup = true;
			levelImage = GameObject.Find ("LevelImage");
			levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
			levelText.text = "Day " + level;
			levelImage.SetActive (true);
			Invoke ("HideLevelImage", levelStartDelay);
			enemies.Clear ();

			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene(level);

		}

		private void HideLevelImage(){
			levelImage.SetActive (false);
			doingSetup = false;
		}

		public void GameOver(){
			levelText.text = "After " + level + " days you starved.";
			levelImage.SetActive (true);
			enabled = false;
		}



		//Update is called every frame.
		void Update()
		{
			//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
			if(playersTurn || enemiesMoving || doingSetup)

				//If any of these are true, return and do not start MoveEnemies.
				return;

			//Start moving enemies.
			StartCoroutine (MoveEnemies ());
		}

		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}


		IEnumerator MoveEnemies(){
			//While enemiesMoving is true player is unable to move.
			enemiesMoving = true;

			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(turnDelay);

			//If there are no enemies spawned (IE in first level):
			if (enemies.Count == 0) 
			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(turnDelay);
			}

			//Loop through List of Enemy objects.
			for (int i = 0; i < enemies.Count; i++)
			{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
				enemies[i].MoveEnemy ();

				//Wait for Enemy's moveTime before moving next Enemy, 
				yield return new WaitForSeconds(enemies[i].moveTime);
			}
			//Once Enemies are done moving, set playersTurn to true so player can move.
			playersTurn = true;

			//Enemies are done moving, set enemiesMoving to false.
			enemiesMoving = false;
		}
	}
}