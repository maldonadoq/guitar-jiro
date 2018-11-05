using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class Gesture : MonoBehaviour {

	Controller controller;
	// Use thissd for initialization
	void Start () {
		controller = new Controller ();
		controller.EnableGesture (Leap.Gesture.GestureType.TYPESWIPE);
		controller.Config.SetFloat ("Gesture.Swipe.MinLength", 200.0f);
		controller.Config.SetFloat ("Gesture.Swipe.MinVelocity", 750f);
		controller.Config.Save ();
	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = controller.Frame();
		GestureList gestures = frame.Gestures ();

		for (int i = 0; i < gestures.Count; i++) {
			Leap.Gesture gesture = gestures [i];
			if (gesture.Type == Leap.Gesture.GestureType.TYPESWIPE) {
				SwipeGesture Swipe = new SwipeGesture (gesture);
				Vector swipeDirection = Swipe.Direction;

				if (swipeDirection.x < 0) {
					Debug.Log ("Left");
				} else if(swipeDirection.x > 0){
					Debug.Log ("Right");
				}
			}
		}
	}



}
