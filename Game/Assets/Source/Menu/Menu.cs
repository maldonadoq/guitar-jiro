using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Leap;

public class Menu : MonoBehaviour {

	public GameObject flecha, lista;	// flecha, lista
	int idx_item = 0;					// indice de la lista
	int idx_last;
	public Controller controller;		// leap motion controller

	// use this for initialization
	void Start () {
		Dibujar ();

		controller = new Controller ();
		// key tap
		controller.EnableGesture (Leap.Gesture.GestureType.TYPE_KEY_TAP);
		controller.Config.SetFloat ("Gesture.KeyTap.MinLength", 200.0f);
		controller.Config.SetFloat ("Gesture.KeyTap.HistorySeconds", .2f);
		controller.Config.SetFloat ("Gesture.KeyTap.MinVelocity", 750f);
		controller.Config.Save ();
	}
	
	// update is called once per frame
	void Update () {
		/*int type_gesture = 0;
		bool state = false;

		Frame frame = controller.Frame ();
		GestureList gestures = frame.Gestures ();

		for (int i = 0; i < gestures.Count; i++) {
			Leap.Gesture gesture = gestures [i];
			if (gesture.Type == Leap.Gesture.GestureType.TYPE_KEY_TAP) {
				KeyTapGesture keytap = new KeyTapGesture (gesture);
				Vector keytapDirection = keytap.Direction;

				if (keytapDirection.x != 0) {
					Debug.Log ("key tap");
					type_gesture = 1;
					state = true;
				}
			}
		}

		//if(up){
		//	idx_item--;
		//			if (idx_item < 0)
		//		idx_item = lista.transform.childCount - 1;
		//}

		switch (type_gesture) {
		case 1: 
			idx_item++;
			if (idx_item > lista.transform.childCount - 1)
				idx_item = 0;			
			break;
		default:
			Debug.Log ("ningún gesto");
			break;
		}

		if(state){
			Dibujar ();
		}

		if (Input.GetKeyDown ("return")) {
			Accion ();
		}*/


		bool up = Input.GetKeyDown ("up");
		bool down = Input.GetKeyDown ("down");

		idx_last = idx_item;
		if(up){			
			idx_item--;
			if (idx_item < 0)
				idx_item = lista.transform.childCount - 1;
		}

		if(down){
			idx_item++;
			if (idx_item > lista.transform.childCount - 1)
				idx_item = 0;
		}

		if(up || down){
			Dibujar ();
		}

		if (Input.GetKeyDown ("return")) {
			Accion ();
		}
	}

	// función dibujar index
	void Dibujar(){
		Transform opcion1 = lista.transform.GetChild (idx_item);	// obtener el objeto de la lista
		Transform opcion2 = lista.transform.GetChild (idx_last);	// obtener el objeto de la lista
		//opcion.Get
		opcion2.GetComponent<Text> ().color = new Color32 (0, 124, 42, 255);
		opcion2.GetComponent<Text> ().fontStyle = FontStyle.Normal;

		opcion1.GetComponent<Text> ().color = new Color32 (191, 15, 55, 255);
		opcion1.GetComponent<Text> ().fontStyle = FontStyle.Italic;

		flecha.transform.position = opcion1.position;
	}

	void Accion(){
		Transform opcion = lista.transform.GetChild (idx_item);	// obtener el objeto de la lista
		string name = opcion.gameObject.name;

		switch (name) {
		case "Salir":
			Application.Quit ();
			print ("Salir");
			break;
		case "Nuevo":
			SceneManager.LoadScene ("Select");
			print ("Nuevo");
			break;
		case "Creditos":
			SceneManager.LoadScene ("Creditos");
			print ("Creditos");
			break;
		default:
			break;
		}			

	}
}

