
using UnityEngine;
using Leap;

public class TapDetector : MonoBehaviour
{
	public bool Right;
	public bool Left;
	public bool DownLeft;
	public bool DownRight;

    public bool IncVol;
    public bool DecVol;

    public bool Pause;
	public bool Exit;
	public bool PauseTrigger;

	public GameObject RightLine;
	public GameObject LeftLine;
	public GameObject DownRightLine;
	public GameObject DownLeftLine;

    Controller leap;
    Color tblue, blue;

    bool CR;
	bool CL;
	bool CDL;
	bool CDR;
	bool CPause;

	void Start ()
	{
		Right = Left = DownLeft = DownRight = false;
		Pause = Exit = CPause = false;
		CR = CL = CDL = CDR = false;
        IncVol = DecVol = false;

        leap = new Controller();

        leap.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
        leap.Config.SetFloat("Gesture.CircleTap.MinLength", 200.0f);
        leap.Config.SetFloat("Gesture.CircleTap.MinVelocity", 750f);
        leap.Config.Save();

        tblue = new Color32(0,150,100,255);
        blue = new Color32(0, 0, 150, 255);
    }

    void Update()
    {
        CR = Right;
        CL = Left;
        CDL = DownLeft;
        CDR = DownRight;

        Right = Left = DownLeft = DownRight = false;
        IncVol = DecVol = false;
        Pause = false;

        Frame frame = leap.Frame();

        GestureList gestures = frame.Gestures();
        for (int i = 0; i < gestures.Count; i++) {
            Gesture gesture = gestures[i];
            if (gesture.Type == Gesture.GestureType.TYPE_CIRCLE) {
                CircleGesture circle = new CircleGesture(gesture);

                string clockwiseness;
                if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Mathf.PI / 2) {
                    IncVol = true;
                    clockwiseness = "clockwise";
                }
                else {
                    DecVol = true;
                    clockwiseness = "counterclockwise";
                }

                print(clockwiseness);
            }
        }

        foreach (Hand hand in frame.Hands) {
            foreach (Finger finger in hand.Fingers) {
                Vector FingerPos = finger.TipPosition;

                // destroy note if tap success
                if (FingerPos.x > 0 && FingerPos.x < 200 && FingerPos.y < 120) {
                    DownRight = true;
                }
                if (FingerPos.x < 0 && FingerPos.x > -200 && FingerPos.y < 120) {
                    DownLeft = true;
                }
                if (FingerPos.x < -170 && FingerPos.y > 50 && FingerPos.y < 250) {
                    Left = true;
                }
                if (FingerPos.x > 170 && FingerPos.y > 50 && FingerPos.y < 250) {
                    Right = true;
                }

                // pause - stop
                if (FingerPos.y > 400) {
                    if (FingerPos.x > 100)
                        Exit = true;
                    if (FingerPos.x < -100)
                        Pause = true;
                }
            }
        }


        if (Input.GetKey(KeyCode.D)) {
            Left = true;
        }
        if (Input.GetKey(KeyCode.F)) {
            DownLeft = true;
        }
        if (Input.GetKey(KeyCode.J)) {
            DownRight = true;
        }
        if (Input.GetKey(KeyCode.K)) {
            Right = true;
        }
        if (Input.GetKey(KeyCode.Escape)) {
            Exit = true;
        }
        if (Input.GetKey(KeyCode.W)) {
            Pause = true;
        }
        if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.Alpha2)) {
            IncVol = true;
        }
        if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.Alpha1)) {
            DecVol = true;
        }


        // action pause
        if (CPause != Pause && Pause) {
            PauseTrigger = true;
        }

        CPause = Pause;

        if (DownRight != CDR) {
            if (DownRight) {
                DownRightLine.GetComponent<MeshRenderer>().material.color = blue;
            } else {
                DownRightLine.GetComponent<MeshRenderer>().material.color = tblue;
            }
        }

        if (DownLeft != CDL) {
            if (DownLeft) {
                DownLeftLine.GetComponent<MeshRenderer>().material.color = blue;
            } else {
                DownLeftLine.GetComponent<MeshRenderer>().material.color = tblue;
            }
        }

		if (Right != CR) {
	        if (Right) {
		        RightLine.GetComponent <MeshRenderer> ().material.color = blue;
	        } else {
		        RightLine.GetComponent <MeshRenderer> ().material.color = tblue;
	        }
        }

		if (Left != CL) {
	        if (Left) {
		        LeftLine.GetComponent <MeshRenderer> ().material.color = blue;
	        } else {
		        LeftLine.GetComponent <MeshRenderer> ().material.color = tblue;
	        }
        }
	}
}
