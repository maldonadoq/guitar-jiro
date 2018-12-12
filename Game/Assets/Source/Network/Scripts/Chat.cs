using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Chat : NetworkBehaviour {
	public InputField TField;
	public Text TChat;

	void Start(){;
		TChat = GameObject.Find("Msg").GetComponent<Text>();
		TField = GameObject.Find("TIField").GetComponent<InputField>();

		TChat.text = "Guitar Hero - Messenger\n";
	}

	void Update(){
		if(!isLocalPlayer)
			return;

		if(Input.GetKeyDown(KeyCode.Return)){
			if(TField.text != ""){
				string tmp = TField.text;
				TField.text = "";

				CmdSend(tmp);
			}
		}
	}

	[Command]
	void CmdSend(string msg){
		RpcReceive(msg);
	}

	[ClientRpc]
	public void RpcReceive(string msg){
		TChat.text += " - "+ msg + "\n";
	}
}
