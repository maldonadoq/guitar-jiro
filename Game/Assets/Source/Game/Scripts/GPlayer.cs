using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class GPlayer : MonoBehaviour
{
	public string beatmapName;
    public string nameSong;
	public int difficulty;
    public bool defaultSE;
    public bool enableBG;    

    public bool enableSE;
    bool useMovBG;
	bool defaultBG;

    public JSONNode SEname;
	public Text ScoreText;
	public Text ComboText;

	public GameObject PanelChat;

	public Button PauseButton;
	public Text PauseButtonText;
	public Button StopButton;
	public Text StopButtonText;

	public long ScoreCounter;
	public long ScoreNow;
	public int ComboCounter;
	public int MaxCombo;
	public int PerfectCount;
	public int GoodCount;
	public int BadCount;
	public int MissCount;

	public bool pause;
	public bool[] trackbusy = {false, false, false, false};

    bool loadFail;
	bool gameover;
	float gameoverTimer;
	bool isConnect;

	AudioSource music;
	MovieTexture mov;

    NoteGenerator NGDL;     // down left
	NoteGenerator NGDR;     // down right
	NoteGenerator NGRD;     // right down
	NoteGenerator NGLD;     // left down   

	TapDetector TD;

	JSONNode Beatmap;
	JSONArray HitObjects;
	JSONArray now;

	int i;

	bool NotesBeforeDone;
	float Timer;

	void Start () {

		isConnect = false;
		PanelChat = GameObject.Find ("Panel");		

		PlayerPrefs.SetInt ("GameStarted", 1);
        enableSE = PlayerPrefs.GetInt("enableSE") != 0;
        enableBG = PlayerPrefs.GetInt("enableBG") != 0;

        this.beatmapName = PlayerPrefs.GetString ("song");

		print(" Play " + this.beatmapName);
		
		this.difficulty = PlayerPrefs.GetInt ("Difficulty");

		loadFail = true;	// asume load fail

		TextAsset f = Resources.Load ("Music/" + beatmapName + "/beatmap") as TextAsset;
		if (f == null)
			return;

		string s = f.ToString ();
		Beatmap = JSON.Parse (s);
		if (Beatmap == null)
			return;

		music = GetComponent<AudioSource> ();
		music.clip = Resources.Load ("Music/" + beatmapName + "/" + Beatmap ["Audio"] ["Name"]) as AudioClip;
        nameSong = "Music/" + beatmapName + "/" + Beatmap["Audio"]["Name"];

        if (music.clip == null)
			return;

        if (Beatmap["SoundEffect"]["Enable"].AsBool) {
            defaultSE = false;
            SEname = Beatmap["SoundEffect"]["Name"];
            Debug.Log("use custom SE");
        }
        else {
            defaultSE = true;
            Debug.Log("use default SE");
        }

        useMovBG = false;
		if (enableBG) {
			if (Beatmap ["Background"] ["Enable"].AsBool) {
				switch (Beatmap ["Background"] ["Type"].AsInt) {
					case 0:
						// use Picture as background
						break;
					case 1:
						// use video as background
						mov = Resources.Load ("Music/" + beatmapName + "/" + Beatmap ["Background"] ["Name"]) as MovieTexture;
						print("Music/" + beatmapName + "/" + Beatmap ["Background"] ["Name"]);
						if (mov == null){
							print("fail background");
							return;
						}

						print("ok background");
						(GameObject.Find ("VideoPlayer").GetComponent ("VPlayer") as VPlayer).movTexture = mov;
						useMovBG = true;
						break;
					default:
						break;
				}
			}
			else {
				Destroy (GameObject.Find ("VideoPlayer"));
				Debug.Log ("use default BG");
			}
		}
		else {
			Destroy (GameObject.Find ("VideoPlayer"));
			Debug.Log ("not use BG");
		}

		switch (difficulty) {
		case 0:
			HitObjects = Beatmap ["GameObject"] ["Easy"].AsArray;
			break;
		case 1:
			HitObjects = Beatmap ["GameObject"] ["Normal"].AsArray;
			break;
		case 2:
			HitObjects = Beatmap ["GameObject"] ["Hard"].AsArray;
			break;
		default:
			return;
		}

		if (HitObjects == null)
			return;

		loadFail = false;	// load success
		Debug.Log ("load success");

		NGDL = GameObject.Find ("NoteGeneratorDL").GetComponent ("NoteGenerator") as NoteGenerator;
		NGDR = GameObject.Find ("NoteGeneratorDR").GetComponent ("NoteGenerator") as NoteGenerator;
		NGRD = GameObject.Find ("NoteGeneratorRD").GetComponent ("NoteGenerator") as NoteGenerator;
		NGLD = GameObject.Find ("NoteGeneratorLD").GetComponent ("NoteGenerator") as NoteGenerator;
		TD = GameObject.Find ("TapDetector").GetComponent ("TapDetector") as TapDetector;

		// init
		i = 0;
		now = HitObjects [0].AsArray;
		gameover = false;
		pause = false;
		ScoreCounter = ComboCounter = MaxCombo = PerfectCount = GoodCount = BadCount = MissCount = 0;
		ScoreNow = 0;

		ScoreText.text = "Score: " + ScoreCounter.ToString ();
		ComboText.text = "Combo: " + ComboCounter.ToString ();
		ScoreText.fontSize = ComboText.fontSize = (int)(Screen.width * 0.03f);

		PauseButton.onClick.AddListener (PauseResume);
		StopButton.onClick.AddListener (StopGame);
		
		Timer = 0f;
		NotesBeforeDone = false;

		if (now [1].AsFloat - 7 / now [3].AsFloat < 0) {
			Timer = Mathf.Abs (now [1].AsFloat - 7 / now [3].AsFloat);
			NotesBeforeDone = false;
		} else {
			StartGame ();
		}

	}

	void FixedUpdate() {
		if (!pause) {
			if (ScoreNow < ScoreCounter)
				if (ScoreCounter - ScoreNow < 10)
					ScoreNow++;
			else
				ScoreNow += (ScoreCounter - ScoreNow) / 10;
		}
	}

	void Update () {
		if (loadFail)
			return;	

		if (TD.PauseTrigger){
			PauseResume ();
		}
		
		if (TD.Exit)
			StopGame ();

		if (!pause)
			ScoreText.text = "Score: " + ScoreNow.ToString ();

		if (pause && !TD.Exit)
			return;

        if (TD.IncVol) {
            if(music.volume < 1)
                music.volume += 0.01f;
        }

        if (TD.DecVol) {
            if (music.volume > 0)
                music.volume -= 0.01f;
        }

        if (HitObjects.Count <= i) {
			if (!gameover) {
				gameover = true;
				if (now [0].AsInt == 3)
					gameoverTimer = 4f + now [3].AsFloat;
				else
					gameoverTimer = 4f + 7 / now [3].AsFloat;

				if (TD.Exit)
					gameoverTimer = 0.5f;
			}

			gameoverTimer -= Time.deltaTime;
			if (gameoverTimer < 4 && !TD.Exit)
				music.volume -= Time.deltaTime / 4f;

			if (gameoverTimer < 0.5 && !TD.Exit)
				music.volume -= Time.deltaTime * 2;			

			if (gameoverTimer < 0) {
				if (TD.Exit) {
                    SceneManager.LoadScene("Select");
					return;
				}

				PlayerPrefs.SetString ("ScoreCount", ScoreCounter.ToString ());
				PlayerPrefs.SetInt ("ComboCount", MaxCombo);
				PlayerPrefs.SetInt ("PerfectCount", PerfectCount);
				PlayerPrefs.SetInt ("GoodCount", GoodCount);
				PlayerPrefs.SetInt ("BadCount", BadCount);
				PlayerPrefs.SetInt ("MissCount", MissCount);

				if (PlayerPrefs.GetString (beatmapName + "MaxScore").Equals (""))
					PlayerPrefs.SetString (beatmapName + "MaxScore", ScoreCounter.ToString ());
				else if (long.Parse (PlayerPrefs.GetString (beatmapName + "MaxScore")) < ScoreCounter)
					PlayerPrefs.SetString (beatmapName + "MaxScore", ScoreCounter.ToString ());

				if (PerfectCount == HitObjects.Count)
					PlayerPrefs.SetString ("Judgement", "X");
				else if (PerfectCount >= HitObjects.Count * 0.9f)
					PlayerPrefs.SetString ("Judgement", "S");
				else if (PerfectCount >= HitObjects.Count * 0.75f)
					PlayerPrefs.SetString ("Judgement", "A");
				else if (PerfectCount >= HitObjects.Count * 0.6f)
					PlayerPrefs.SetString ("Judgement", "B");
				else if (PerfectCount >= HitObjects.Count * 0.5f)
					PlayerPrefs.SetString ("Judgement", "C");
				else
					PlayerPrefs.SetString ("Judgement", "D");

				SceneManager.LoadScene ("Result");
			}
			return;
		}

		if(!isConnect){
			var lplayer = GameObject.Find("Local");

            if (lplayer != null){
				var oplayer = GameObject.Find("Player(Clone)");
				if(oplayer == null){
					music.Pause ();
                    mov.Pause();
					return;
				}
				else{				
					music.Play();
                    mov.Play();
                    isConnect = true;
                    PanelChat.SetActive(false);
                }
            }
		}		

		if (!NotesBeforeDone) {
			Timer -= Time.deltaTime;
			if (Timer <= 0) {
				NotesBeforeDone = true;
				Timer = 0;
				StartGame ();
			}
		}

		switch (now [0].AsInt) {
		case 3:
			if (HitObjects.Count > i && music.time >= now [1].AsFloat) {
				i++;
				if (HitObjects.Count > i)
					now = HitObjects [i].AsArray;	// get current note
			}
			break;
		default:
                // time > generate time
                while (HitObjects.Count > i && music.time - Timer >= (now [1].AsFloat - 7 / now [3].AsFloat)) {
				switch (now [2].AsInt) {
				case 1:
					NGDL.GenerateNote (now [0].AsInt, now [3].AsFloat);
					break;
				case 2:
					NGDR.GenerateNote (now [0].AsInt, now [3].AsFloat);
					break;
				case 3:
					break;
				case 4:
					NGLD.GenerateNote (now [0].AsInt, now [3].AsFloat);
					break;
				case 6:
					NGRD.GenerateNote (now [0].AsInt, now [3].AsFloat);	
					break;
				default:
					break;
				}
				i++;
				if (HitObjects.Count > i)
					now = HitObjects [i].AsArray;
			}
			break;
		}
	}

	void StartGame () {
		music.Play ();
		if (useMovBG)
			mov.Play ();
	}

	void PauseResume ()
	{
		TD.PauseTrigger = false;
		if (!music.isPlaying) {
			pause = false;
			music.Play ();
			if (useMovBG)
				mov.Play ();

			PauseButtonText.text = "Pause";
			PanelChat.SetActive (false);
		} else {
			pause = true;
			music.Pause ();
			if (useMovBG)
				mov.Pause ();

			PauseButtonText.text = "Resume";			
			PanelChat.SetActive (true);
		}
	}

	void StopGame ()
	{
		i = HitObjects.Count;
		TD.Exit = true;
	}
}
