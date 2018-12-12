using UnityEngine;
using UnityEngine.Networking;

public class Drop : MonoBehaviour
{
	public float speed;
	public GameObject judgement;
	public bool pause;
	public bool stop;

	AudioSource hitSE;
	AudioSource missSE;
	GPlayer status;
	Vector3 notePos;
	
	bool hit;
	bool hitable;
	int track;
	bool busy;

	TapDetector TD;

	void Start ()
	{
		hit = pause = stop = hitable = busy = false;

		status = GameObject.Find ("GPlayer").GetComponent ("GPlayer") as GPlayer;
		TD = GameObject.Find ("TapDetector").GetComponent ("TapDetector") as TapDetector;

		hitSE = GetComponent<AudioSource> ();
        if (!status.defaultSE)
            hitSE.clip = Resources.Load("Music/" + status.beatmapName + "/" + status.SEname["hit"]) as AudioClip;
        else
            hitSE.clip = Resources.Load("Default/hit") as AudioClip;
        missSE = hitSE;
        if (!status.enableSE)
            hitSE.mute = true;

        float y_basef = 1.15f;		
		notePos = transform.position;

		if (notePos.x > 0) {
			if (notePos.y < y_basef - 0.5) {				
				track = 0;
			} else if (notePos.y < y_basef + 2) {				
				track = 2;
			}
		} else {
			if (notePos.y < y_basef - 0.5) {				
				track = 1;
			} else if (notePos.y < y_basef + 2) {				
				track = 3;
			}
		}
	}

	void Update ()
	{
		if (status.pause)
			return;

		if ((notePos.z <= -1.75 - speed / 10 && !missSE.isPlaying) || (hit && !hitSE.isPlaying)) {
			Destroy (gameObject);
		}

        if (hit && hitSE.isPlaying)
            GetComponent<LensFlare>().brightness -= Time.deltaTime;

		if (!hit)
			transform.Translate (new Vector3 (10, 0, 0) * Time.deltaTime * speed);

		notePos = transform.position;
		if (!hit && notePos.z < 2 && notePos.z > -1.75 - speed / 10) {
			if (status.trackbusy [track] && !busy)
				return;

			status.trackbusy [track] = busy = true;
			
			bool tap_success = false;
			switch (track) {
			case 0:	// DR
				tap_success = TD.DownRight;
				break;
			case 1:	// DL	
				tap_success = TD.DownLeft;
				break;
			case 2:	// R
				tap_success = TD.Right;
				break;
			case 3:	// L
				tap_success = TD.Left;
				break;
			}

			if (tap_success) {
				if (!hitable)
					return;
								
				int ScoreGet;
				if (Mathf.Abs (notePos.z) < 0.75 * speed / 1.4) {
					ScoreGet = 300 + 300 / 25 * status.ComboCounter;
					status.ComboCounter++;
					status.PerfectCount++;
				} else if (Mathf.Abs (notePos.z) < 1.5 * speed / 1.4) {
					ScoreGet = 100 + 100 / 25 * status.ComboCounter;
					status.ComboCounter++;
					status.GoodCount++;
				} else if (Mathf.Abs (notePos.z) < 1.75 * speed / 1.4) {					
					ScoreGet = 50 + 50 / 25 * status.ComboCounter;
					status.ComboCounter = 0;
					status.BadCount++;
				} else {
					status.MissCount++;								
                    ScoreGet = 0;
				}
				status.ScoreCounter += ScoreGet;
				if (status.ComboCounter > status.MaxCombo)
					status.MaxCombo = status.ComboCounter;
                
				status.ComboText.text = "Combo: " + status.ComboCounter.ToString ();

				hit = true;
				status.trackbusy [track] = false;

                if (status.enableSE) {
                    hitSE.enabled = true;
                    hitSE.Play();
                }

                GetComponent<Renderer> ().enabled = false;
                GetComponent<LensFlare>().brightness *= 2;
                return;
			}			
			hitable = true;
		}

		if (notePos.z <= -1.75 - speed / 10 && !hit && !missSE.isPlaying) {			
			status.ComboCounter = 0;

            if (status.enableSE) {
                missSE.mute = true;
                missSE.enabled = true;
                missSE.Play();
            }

            status.MissCount++;

            var lplayer = GameObject.Find("Local");
			if(lplayer != null){
				var lh = lplayer.GetComponent<PlayerC>();
				lh.isDamage = true;
			}		

            status.ComboText.text = "Combo: " + status.ComboCounter.ToString ();
			status.trackbusy [track] = false;			
			GetComponent<Renderer> ().enabled = false;
		}
	}
}