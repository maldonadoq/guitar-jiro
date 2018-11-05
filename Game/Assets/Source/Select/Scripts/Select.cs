using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SimpleJSON;
using System.IO;
using Leap;

public class Select : MonoBehaviour {
	protected Controller leap;

	public GameObject menuitemobj;
	public List<GameObject> menulist = new List<GameObject> ();
	protected string playingsong ;

	protected Dictionary<string,int> diff ;
	protected Dictionary<string,string> song ;
	protected Dictionary<string,string> songname ;
	protected Dictionary<string,float> songstart ;
	protected Dictionary<string,Color> diffcolormap ;
	protected List<string> diffcollection;
	protected Dictionary<string,List<string>> difflist;
	protected Dictionary<string,float> backward;
	public int musicnum = 0;

	protected bool motionlock = false;
	protected double lastx;
	protected int lastmotion;

	protected GUITexture CoverTexture;
	protected bool covermotion = true;
	protected float alphadirection = -1.0f;
	protected double changedifftime = 1.5;
	protected string difftonext;
	protected float changedifftimeendtime = 0;

	protected bool QuitGameFlag =false;

	GUIText difftextobj;

	// Use this for initialization
	void Start () {
		difflist = new Dictionary<string, List<string>> ();
		CoverTexture = GameObject.Find ("Cover").GetComponent<GUITexture> ();

		this.diff = new Dictionary<string, int> (){{"Easy",0}, {"Normal",1}, {"Hard",2}}; 	// dificultad
		this.song = new Dictionary<string,string> ();										// concion
		this.songname = new Dictionary<string, string> ();									// nombre de canciones
		this.songstart = new Dictionary<string, float> ();
		this.backward = new Dictionary<string, float> ();

		leap = new Controller ();		// leap motion controller

		diffcollection = new List<string> (){"Easy","Normal","Hard"};
		diffcolormap = new Dictionary<string, Color> {
			{"Hard",new Color(249.0f/255,90.0f/255,101.0f/255,1.0f)},
			{"Easy",new Color(191.0f/255,255.0f/255,160.0f/255,1.0f)},
			{"Normal",new Color(58.0f/255,183.0f/255,239.0f/255,1.0f)}
		};

		// read from resource music
		string[] files = (Resources.Load ("Music/MusicList") as TextAsset).text.Replace ("\r\n", "\n").Replace ("\r", "\n").Split ("\n" [0]);
		Debug.Log (files.Length.ToString () + " music in list");


		foreach (var item in files) {
			string folder = item;
			TextAsset f = Resources.Load ("Music/" + folder + "/beatmap") as TextAsset;

			if (f == null) {
				return;
			}
				
			JSONNode Beatmap = JSON.Parse (f.ToString ());

			difflist [Beatmap ["Title"]] = new List<string> ();

			foreach (string difficulty in diffcollection) {
				if (Beatmap ["Difficulty"] [difficulty] ["Enable"].AsBool) {
					difflist [Beatmap ["Title"]].Add (difficulty);
				}
			}

			Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
			this.backward [Beatmap ["Title"]] = (this.musicnum * 0.5f);
			if (PlayerPrefs.HasKey ("BACKWARD")) {
				pos.x -= PlayerPrefs.GetFloat ("BACKWARD");
			}

			pos.x += (this.musicnum * 0.5f);
			GameObject menuitem_tmp = (GameObject)Instantiate (menuitemobj, pos, transform.rotation);
			menuitem_tmp.GetComponent<GUITexture> ().texture = (Texture)Resources.Load ("Music/" + folder + "/" + Beatmap ["Album"] ["Name"]);

			menuitem_tmp.GetComponent<GUIText> ().text = Beatmap ["Title"];// +'\n'+ difficulty;
			Vector2 tpos = new Vector2 (0, - UnityEngine.Screen.height / 3.5f);
			menuitem_tmp.GetComponent<GUIText> ().pixelOffset = tpos;
			menuitem_tmp.GetComponent<GUIText> ().fontSize = (int)((tpos.y) / -83.5f * 15.0f);

			menuitem_tmp.transform.GetChild (0).GetComponent<GUIText> ().text = difflist [Beatmap ["Title"]] [0];
			menuitem_tmp.transform.GetChild (0).GetComponent<GUIText> ().color = diffcolormap [difflist [Beatmap ["Title"]] [0]];
			tpos.y += ((tpos.y) / -83.5f);
			menuitem_tmp.transform.GetChild (0).GetComponent<GUIText> ().pixelOffset = tpos;
			menuitem_tmp.transform.GetChild (0).GetComponent<GUIText> ().fontSize = (int)((tpos.y) / -83.5f * 15.0f);

			menuitem_tmp.transform.GetChild (1).GetComponent<GUIText> ().text = Beatmap ["Artist"];
			menuitem_tmp.transform.GetChild (1).GetComponent<GUIText> ().fontSize = (int)((tpos.y) / -83.5f * 10.0f);
			tpos.y -= ((tpos.y) / -83.5f * 20.0f);
			menuitem_tmp.transform.GetChild (1).GetComponent<GUIText> ().pixelOffset = tpos;

			// save the Songs identity
			menulist.Add (menuitem_tmp);

			song [Beatmap ["Title"]] = folder;
			this.songname [Beatmap ["Title"]] = Beatmap ["Audio"] ["Name"];
			songstart [Beatmap ["Title"]] = float.Parse (Beatmap ["PreviewTime"]);
			this.musicnum ++;

			if (pos.x < 0.6 && pos.x > 0.4) {
				string songpath = this.song [Beatmap ["Title"]];
				string songname = this.songname [Beatmap ["Title"]];
				var music = GetComponent<AudioSource> ();
				music.clip = Resources.Load ("Music/" + songpath + "/" + songname) as AudioClip;
				music.time = this.songstart [Beatmap ["Title"]];
				music.loop = true;
				music.Play ();

				playingsong = Beatmap ["Title"];
				if (PlayerPrefs.HasKey ("Difficulty")) {
					menuitem_tmp.transform.GetChild (0).GetComponent<GUIText> ().text = diffcollection [PlayerPrefs.GetInt ("Difficulty")];
					menuitem_tmp.transform.GetChild (0).GetComponent<GUIText> ().color = diffcolormap [diffcollection [PlayerPrefs.GetInt ("Difficulty")]];
				}
			}
		}

		//open the guesture
		leap.EnableGesture (Leap.Gesture.GestureType.TYPESWIPE);
		//leap.Config.SetFloat("Gesture.Swipe.MinLength", 100.0f);
		leap.Config.SetFloat ("Gesture.Swipe.MinVelocity", 750f);
		leap.Config.Save ();
	}

	// Update is called once per frame
	void Update () {
		//the cover	
		if (covermotion) {
			Color tcolor = CoverTexture.color;
			float a = tcolor.a + Time.deltaTime * 2 * alphadirection;
			if (a < 0.0f) {

				tcolor.a = 0.0f;
				covermotion = false;
				alphadirection = 1.0f;

			}else{
				tcolor.a = a;
				CoverTexture.color = tcolor;
			}


			if(QuitGameFlag && a>0.8f ){

				QuitGameFlag = false;
				Debug.Log("system exit!");
				Application.Quit();
			}
		}

		// guesture judgement
		if (motionlock) {
			if (lastmotion == 2) {
				// change diff

				var color = difftextobj.color;
				if (changedifftime > 1.2f) {
					color.a -= (float)(Time.deltaTime / 0.3);
				}
				else if (changedifftime > 0.9f) {
					color.a += (float)(Time.deltaTime / 0.3);
				}

				difftextobj.color = color;
				//change when half time
				if ((changedifftime - Time.deltaTime - 1.2) * (changedifftime - 1.2) < 0) {
					difftextobj.text = difftonext;
					var clr = diffcolormap [difftonext];
					clr.a = 0f;
					difftextobj.color = clr;
				}
				//
				changedifftime -= Time.deltaTime;
				if (changedifftime < changedifftimeendtime) {
					changedifftime = 1.5;
					motionlock = false;
				}

			} else if (lastmotion == -1||lastmotion == 1) {

				foreach (var item in menulist) {
					// move items and test move finished
					Vector3 position = item.transform.position;
					double lx = position.x;
					position.x += 0.5f * lastmotion * Time.deltaTime;
					item.transform.position = position;
					// if any item cross the 0.5f , release the lock;
					if ((position.x - 0.5) * (lx - 0.5) < 0) {
						if (playingsong != item.GetComponent<GUIText> ().text) {
							// play this item music
							foreach (var in_item in menulist) {
								Vector3 in_postion = in_item.transform.position;
								in_postion.x -= 0.5f * lastmotion * Time.deltaTime;
								in_item.transform.position = in_postion;
							}

							string folder = this.song [item.GetComponent<GUIText> ().text];
							string name = this.songname [item.GetComponent<GUIText> ().text];
							var music = GetComponent<AudioSource> ();
							music.clip = Resources.Load ("Music/" + folder + "/" + name) as AudioClip;
							Debug.Log ("play music demo " + "/Music/" + folder + "/" + name);
							music.time = this.songstart [item.GetComponent<GUIText> ().text];
							music.loop = true;
							music.Play ();
							playingsong = item.GetComponent<GUIText> ().text;
						}
						// unlock 
						this.motionlock = false;
					}
				}
				GetComponent<AudioSource> ().volume -= Time.deltaTime * 1.2f;
			} else if (lastmotion == 0){
				foreach (var item in menulist) {
					Vector3 position = item.transform.position;
					if (position.x > 0.4 && position.x < 0.6) {
						// select movement
						position.y += 0.6f * Time.deltaTime;
						item.transform.position = position;
					}
					if (item.transform.position.y > 0.8) {  
						// enter the InGame Scene
						// select success
						PlayerPrefs.SetString ("song", this.song [item.GetComponent<GUIText> ().text]);

						PlayerPrefs.SetInt ("enableSE", 1);
						PlayerPrefs.SetInt ("enableBG", 1);
						PlayerPrefs.SetFloat ("BACKWARD", this.backward [item.GetComponent<GUIText> ().text]);
						PlayerPrefs.SetInt ("Difficulty", this.diff [item.transform.GetChild (0).GetComponent<GUIText> ().text]);
						SceneManager.LoadScene ("Game");
					}

				}
			}

		} else {
			// fix the low sound
			if (GetComponent<AudioSource> ().volume < 1.0) {
				GetComponent<AudioSource> ().volume += Time.deltaTime * 0.5f;
			}
			// keyboard
			if (!QuitGameFlag && Input.GetKey(KeyCode.Escape)){
				Debug.Log("get key escape");
				//Application.Quit();
				alphadirection = 1f;
				covermotion = true;
				QuitGameFlag = true;
				lastmotion = 3;
				motionlock = true;
			}
			if (Input.GetKey (KeyCode.A)||Input.GetKey (KeyCode.LeftArrow)) {
				if(menulist [0].transform.position.x < 0.4f){
					lastmotion = 1;
					motionlock = true;
				}
			}
			if (Input.GetKey (KeyCode.D)||Input.GetKey (KeyCode.RightArrow)) {
				if((menulist [0].transform.position.x + (menulist.Count - 1) * 0.5) > 0.6f){
					lastmotion = -1;
					motionlock = true;
				}
			}
			if (Input.GetKey (KeyCode.S)||Input.GetKey (KeyCode.DownArrow)|| Input.GetKey (KeyCode.Space)) {
				changedifftimeendtime = 0.8f;
				motionlock = true;
				foreach (var item in menulist) {
					Vector3 position = item.transform.position;
					if (position.x > 0.4 && position.x < 0.6) {
						var list = difflist [item.GetComponent<GUIText> ().text];
						int index = list.IndexOf (item.transform.GetChild (0).GetComponent<GUIText> ().text);
						Debug.Log ("diff content list " + list.ToString () + " index: " + index);

						index = (index + 1) % list.Count;
						difftonext = list [index];
						difftextobj = item.transform.GetChild (0).GetComponent<GUIText> ();
						//item.transform.GetChild(0).GetComponent<GUIText>().text = list[index];
						//item.transform.GetChild(0).GetComponent<GUIText>().color = diffcolormap[list[index]];
					}
				}
				lastmotion = 2;
			}
			if(Input.GetKey (KeyCode.W)||Input.GetKey (KeyCode.UpArrow)||Input.GetKey (KeyCode.Return)){
				covermotion = true;
				lastmotion = 0;
				motionlock = true;
			}
			//
			Frame sfream = leap.Frame ();
			foreach (var gesture in sfream.Gestures()) {
				if (gesture.Type == Leap.Gesture.GestureType.TYPESWIPE) {
					SwipeGesture swipeGesture = new SwipeGesture (gesture);
					Debug.Log (" gesture read success : " + swipeGesture.Direction.ToString ());
					Vector gestureVector = swipeGesture.Direction;
					float x = gestureVector.x;
					float y = gestureVector.y;
					if (x < - 0.7f && (menulist [0].transform.position.x + (menulist.Count - 1) * 0.5) > 0.6f) {
						lastmotion = -1;
					} else if (x > 0.7f && menulist [0].transform.position.x < 0.4f) {
						lastmotion = 1;
					} else if (y < -0.7f) { 
						// change difficult
						changedifftimeendtime = 0.0f;
						motionlock = true;
						foreach (var item in menulist) {
							Vector3 position = item.transform.position;
							if (position.x > 0.4 && position.x < 0.6) {
								var list = difflist [item.GetComponent<GUIText> ().text];
								int index = list.IndexOf (item.transform.GetChild (0).GetComponent<GUIText> ().text);
								Debug.Log ("diff content list " + list.ToString () + " index: " + index);

								index = (index + 1) % list.Count;
								difftonext = list [index];
								difftextobj = item.transform.GetChild (0).GetComponent<GUIText> ();
							}
						}
						lastmotion = 2;

					} else if (y > 0.6) {
						Debug.Log ("select music guesture");
						lastmotion = 0;
						covermotion = true;
					} else {
						return;
					}
					motionlock = true;
				}
			}
		}
	}
}
