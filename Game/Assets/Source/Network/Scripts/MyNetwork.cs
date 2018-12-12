using UnityEngine.Networking;

public class MyNetwork : NetworkDiscovery
{
    void Awake()
    {
        Initialize();
    }
    

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);

        if (!NetworkManager.singleton.IsClientConnected())
        {
            NetworkManager.singleton.networkAddress = fromAddress;
            NetworkManager.singleton.StartClient();
        }
    }

}
