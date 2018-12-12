using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using Leap;

public class Menu : MonoBehaviour
{

    public GameObject flecha, lista;        // flecha, lista
    int idx_item = 0;                       // indice de la lista
    int idx_last;
    MyNetwork myNetwork;                    // network  
    public Controller controller;		    // leap motion controller

    // network
    void Awake()
    {
        myNetwork = FindObjectOfType<MyNetwork>();
    }

    public void JoinGame()
    {
        StopDiscovery();
        myNetwork.StartAsClient();
    }

    private void StopDiscovery()
    {
        if (myNetwork.running)
        {
            myNetwork.StopBroadcast();
        }
    }

    bool CheckFist(Hand hand)
    {
        float sum = 0;
        int n = 0;
        bool r = false;
        Vector meta, proxi, inter;
        foreach (Finger finger in hand.Fingers)
        {
            meta = finger.Bone(Bone.BoneType.TYPE_METACARPAL).Direction;
            proxi = finger.Bone(Bone.BoneType.TYPE_PROXIMAL).Direction;
            inter = finger.Bone(Bone.BoneType.TYPE_INTERMEDIATE).Direction;

            sum += meta.Dot(proxi);
            sum += proxi.Dot(inter);

            if (finger.IsExtended)
                n++;
        }

        sum = sum / 10;
        if (sum <= 0.5 && n == 0)
            r = true;

        return r;
    }

    void Start()
    {
        Draw();
        StopDiscovery();

        controller = new Controller();
        // key tap
        controller.EnableGesture(Leap.Gesture.GestureType.TYPE_KEY_TAP);
        controller.Config.SetFloat("Gesture.KeyTap.MinLength", 200.0f);
        controller.Config.SetFloat("Gesture.KeyTap.MinVelocity", 750f);

        controller.EnableGesture(Leap.Gesture.GestureType.TYPE_SCREEN_TAP);
        controller.Config.SetFloat("Gesture.ScreenTap.MinLength", 200.0f);
        controller.Config.SetFloat("Gesture.ScreenTap.MinVelocity", 750f);

        controller.Config.Save();
        StopDiscovery();
    }

    void Update()
    {
        int type_gesture = 0;
        bool state = false;

        Frame frame = controller.Frame();
        GestureList gestures = frame.Gestures();
        HandList hands = frame.Hands;

        Hand hand;
        if (hands.Count == 1) {
            hand = hands.Frontmost;
            if (CheckFist(hand)) {
                type_gesture = 2;
            }
        }

        for (int i = 0; i < gestures.Count; i++) {
            Leap.Gesture gesture = gestures[i];
            if (gesture.Type == Leap.Gesture.GestureType.TYPE_KEY_TAP) {
                print("key tap");
                type_gesture = 1;
                state = true;
            }
            else if (gesture.Type == Leap.Gesture.GestureType.TYPE_SCREEN_TAP) {
                print("screen tap");
                type_gesture = 2;
                state = true;
            }
        }

        switch (type_gesture) {
            case 1:
                idx_item++;
                if (idx_item > lista.transform.childCount - 1)
                    idx_item = 0;
                break;
            case 2:
                Action();
                break;
            default:
                break;
        }

        if (state) {
            Draw();
        }

        bool up = Input.GetKeyDown("up");
        bool down = Input.GetKeyDown("down");

        idx_last = idx_item;
        if (up)
        {
            idx_item--;
            if (idx_item < 0)
                idx_item = lista.transform.childCount - 1;
        }
        else if (down)
        {
            idx_item++;
            if (idx_item > lista.transform.childCount - 1)
                idx_item = 0;
        }

        if (up || down)
        {
            Draw();
        }

        if (Input.GetKeyDown("return"))
        {
            Action();
        }
    }

    // función Draw index
    void Draw()
    {
        Transform opcion1 = lista.transform.GetChild(idx_item);
        Transform opcion2 = lista.transform.GetChild(idx_last);

        opcion2.GetComponent<Text>().color = new Color32(0, 124, 42, 255);
        opcion2.GetComponent<Text>().fontStyle = FontStyle.Normal;

        opcion1.GetComponent<Text>().color = new Color32(191, 15, 55, 255);
        opcion1.GetComponent<Text>().fontStyle = FontStyle.Italic;

        flecha.transform.position = opcion1.position;
    }

    void Action()
    {
        Transform opcion = lista.transform.GetChild(idx_item);  // obtener el objeto de la lista
        string tname = opcion.gameObject.name;

        switch (tname)
        {
            case "New":
                SceneManager.LoadScene("Select");
                print("New");
                break;
            case "Credits":
                SceneManager.LoadScene("Credits");
                print("Credits");
                break;
            case "Exit":
                Application.Quit();
                print("Exit");
                break;
            case "Join-Game":
                JoinGame();
                print("Join-Game");
                break;
            case "Back":
                SceneManager.LoadScene("Menu");
                print("Back");
                break;
            default:
                break;
        }

    }
}

