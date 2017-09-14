using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeSerializer : MonoBehaviour {

	public DictionaryEntries nativeCollection;
	public WordList wordList;

	void Start() {
		wordList.dictionaryEntries = nativeCollection.entries;

		wordList.Regenerate();
	}
}
