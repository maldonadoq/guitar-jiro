using UnityEngine;

public class NoteGenerator : MonoBehaviour
{
	public GameObject note;
	public GameObject slidenote;

	public void GenerateNote (int type, float speed)
	{
		Quaternion rt = Quaternion.identity;
		rt.eulerAngles = note.transform.rotation.eulerAngles + transform.rotation.eulerAngles;

		switch (type) {
		case 0:
			(((GameObject)Instantiate (note, transform.position, rt)).GetComponent ("Drop") as Drop).speed = speed;
			break;
		case 1:
			(((GameObject)Instantiate (slidenote, transform.position, rt)).GetComponent ("SlideDrop") as SlideDrop).speed = speed;
			break;
		default:
			break;
		}
	}
}
