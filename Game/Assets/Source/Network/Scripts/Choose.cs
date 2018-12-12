using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class Choose : MonoBehaviour
{
    public GameObject flecha, lista;        // flecha, lista
    int idx_item = 0;                       // indice de la lista
    int idx_last;
    MyNetwork myNetwork;                    // network

    // network
    void Awake()
    {
        myNetwork = FindObjectOfType<MyNetwork>();
    }

    public void CreateGame()
    {
        StopDiscovery();
        myNetwork.StartAsServer();
        NetworkManager.singleton.StartHost();
    }

    private void StopDiscovery()
    {
        if (myNetwork.running)
        {
            myNetwork.StopBroadcast();
        }
    }


    void Start()
    {
        Draw();	
        StopDiscovery();
    }	   

    void Update()
    {
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
        Transform opcion = lista.transform.GetChild(idx_item);
        string name = opcion.gameObject.name;

        switch (name)
        {
            case "Single":
                SceneManager.LoadScene("Game");
                print("Single");
                break;
            case "Multi-Player":
                CreateGame();
                print("Multi-Player");
                break;            
            case "Back":
                SceneManager.LoadScene("Select");
                print("Back");
                break;
            default:
                break;
        }

    }
}

// -1.4582
//
// -3.433547

