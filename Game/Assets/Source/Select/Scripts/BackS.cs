using UnityEngine;

public class BackS : MonoBehaviour {
	void Start ()
	{		
		MovieTexture movie = GetComponent<Renderer> ().material.mainTexture as MovieTexture;

		movie.loop = true;
		if (!movie.isPlaying) {
			movie.Play ();
		}
	}
}