using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class BinarySerializer : MonoBehaviour {

	const string DICTIONARY_NAME = "Dictionary Entries";

	public WordList wordList;

	void Start () {
		wordList.save += SerializeDictionary;

		DeserializeDictionary();
	}

	void SerializeDictionary() {
		DirectoryInfo streamingAssetsFolder = new DirectoryInfo( Application.streamingAssetsPath );
		if( !streamingAssetsFolder.Exists ) streamingAssetsFolder.Create();

		string pathToSave = System.IO.Path.Combine( Application.streamingAssetsPath, DICTIONARY_NAME );

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create( pathToSave );
		bf.Serialize( file, wordList.Save() );
		file.Close();
	}

	void DeserializeDictionary() {
		string pathToLoad = System.IO.Path.Combine( Application.streamingAssetsPath, DICTIONARY_NAME );
		FileInfo savedEntries = new FileInfo( pathToLoad );

		if( savedEntries.Exists ) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open( pathToLoad, FileMode.Open );
			wordList.dictionaryEntries = ((SerializableDictionary)bf.Deserialize( file )).entries.ToList();
			file.Close();
		}

		wordList.Regenerate();
	}
}
