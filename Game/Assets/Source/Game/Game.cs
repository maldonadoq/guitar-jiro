using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class Game : MonoBehaviour {
	Controller controller;
	public GameObject blockOne, blockTwo;

	// Use this for initialization
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
		HandList hands = frame.Hands;

		if (hands.Count == 1) {			
			Hand hand = hands.Frontmost;
			print ("Mano cercana!!");

			if (hand.IsRight) {
				print ("Soy derechoo!!!");
			}

			Vector vhand_palm = hand.PalmPosition;
			int size = 0;
			double distance = 0.0;

			foreach(Finger finger in hand.Fingers){
				size++;
				distance += vhand_palm.DistanceTo(finger.TipPosition);
			}

			distance = distance / size;
			if(distance < 5){
				print ("Fist!!!!");
			}
		}

		GestureList gestures = frame.Gestures ();

		for (int i = 0; i < gestures.Count; i++) {
			Leap.Gesture gesture = gestures [i];
			if (gesture.Type == Leap.Gesture.GestureType.TYPESWIPE) {
				SwipeGesture Swipe = new SwipeGesture (gesture);
				Vector swipeDirection = Swipe.Direction;

				if (swipeDirection.x < 0) {
					Debug.Log ("Left");
					DestroyObject (blockOne);
				} else if(swipeDirection.x > 0){
					Debug.Log ("Right");
					DestroyObject (blockTwo);
				}
			}
		}
	}
}
