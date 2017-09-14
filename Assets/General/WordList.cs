using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WordList : MonoBehaviour {

	[HideInInspector] public List<DictionaryEntry> dictionaryEntries = new List<DictionaryEntry>();

	public InputField dictionaryWordInput;
	public InputField dictionaryDefinitionInput;

	public Text listLabel;

	public Action save;

	public void SaveWordToDictionary() {
		if( string.IsNullOrEmpty(dictionaryWordInput.text) ) return;
		if( string.IsNullOrEmpty(dictionaryDefinitionInput.text) ) return;

		DictionaryEntry dictionaryEntryToAdd = new DictionaryEntry{ 
			dictionaryWord = dictionaryWordInput.text, 
			dictionaryDefinition = dictionaryDefinitionInput.text
		};

		dictionaryEntries.Add( dictionaryEntryToAdd );

		if( save != null ) save();

		Regenerate();
	}

	public void Regenerate() {
		string listOfWords = "";

		for( int index = 0; index < dictionaryEntries.Count; index++ ) {
			listOfWords += "- " + dictionaryEntries[index].dictionaryWord + " (" + dictionaryEntries[index].dictionaryDefinition + ")" + "\n";
		}

		listLabel.text = listOfWords;
	}

	public SerializableDictionary Save() {
		return new SerializableDictionary { entries = dictionaryEntries.ToArray() };
	}
}

[Serializable]
public class SerializableDictionary {
	public DictionaryEntry[] entries;
}