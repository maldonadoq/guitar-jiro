using UnityEngine;
using UnityEngine.Networking;

public class PlayerC : NetworkBehaviour
{    
    public bool isDamage = false;
    public string nameSong;

    void Update() {
        if(!isLocalPlayer)  
            return; 

        if(isDamage){
            CmdDamage();
            isDamage = false;
        }            
    }

    public override void OnStartLocalPlayer() {
        GPlayer player = GameObject.Find("GPlayer").GetComponent("GPlayer") as GPlayer;
        print("local "+player.nameSong);
        GetComponent<MeshRenderer>().material.color = Color.blue;        

        base.OnStartLocalPlayer();
        gameObject.name = "Local";
        Vars.Username = ">";        
    }

    [Command]
    void CmdDamage() {
        var health = GetComponent<Health>();        
        health.TakeDamage(0.5);
    }
}
