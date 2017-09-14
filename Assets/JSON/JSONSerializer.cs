using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using UnityEngine.UI;

public class JSONSerializer : MonoBehaviour {

	const string DICTIONARY_NAME = "Dictionary Entries.txt";

	public WordList wordList;

	void Start() {
		wordList.save += SerializeDictionary;
		DeserializeDictionary();
	}

	void SerializeDictionary() {
		string jsonObj = JsonUtility.ToJson( wordList.Save() );
		
		string pathToSave = System.IO.Path.Combine( Application.streamingAssetsPath, DICTIONARY_NAME );

		DirectoryInfo streamingAssetsFolder = new DirectoryInfo( Application.streamingAssetsPath );
		if( !streamingAssetsFolder.Exists ) streamingAssetsFolder.Create();

		File.WriteAllText( pathToSave, jsonObj );

		wordList.Regenerate();
	}

	void DeserializeDictionary() {
		string pathToLoad = System.IO.Path.Combine( Application.streamingAssetsPath, DICTIONARY_NAME );
		FileInfo savedEntries = new FileInfo( pathToLoad );

		if( savedEntries.Exists ) {
			string jsonObj = File.ReadAllText( pathToLoad );
			wordList.dictionaryEntries = new List<DictionaryEntry>( JsonUtility.FromJson<SerializableDictionary>( jsonObj ).entries );
		}

		wordList.Regenerate();
	}
}
