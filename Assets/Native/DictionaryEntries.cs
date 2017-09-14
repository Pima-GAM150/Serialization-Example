using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu( fileName = "Dictionary Entries", menuName = "Dictionary" )]
public class DictionaryEntries : ScriptableObject {
	public List<DictionaryEntry> entries = new List<DictionaryEntry>();
}