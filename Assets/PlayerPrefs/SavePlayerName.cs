using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SavePlayerName : MonoBehaviour {

	public InputField playerNameInput;

	void Start() {
		if( PlayerPrefs.HasKey( "Player Name" ) ) {
			playerNameInput.text = PlayerPrefs.GetString( "Player Name", "No name" );
		}
	}

	public void PlayerNameSaved() {
		PlayerPrefs.SetString( "Player Name", playerNameInput.text );
	}
}
