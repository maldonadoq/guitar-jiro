using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VPlayer : MonoBehaviour {
	public MovieTexture movTexture = null;
	
	void Start () {
		if ((GameObject.Find ("GPlayer").GetComponent ("GPlayer") as GPlayer).enableBG) {
			if (movTexture == null) {
				(GameObject.Find ("GPlayer").GetComponent ("GPlayer") as GPlayer).enableBG = false;
				Destroy(gameObject);
				return;
			}
			GetComponent<Renderer> ().material.mainTexture = movTexture;
			movTexture.loop = true;
		}
	}
}
