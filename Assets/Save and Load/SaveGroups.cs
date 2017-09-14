using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class SaveGroups : MonoBehaviour {

	const string saveFolderName = "SavedData";
	public string currentSavePath { get { return saveFolder.FullName + "/" + currentSaveName; } }

	DirectoryInfo saveFolder;
	string currentSaveName;

	public List<SaveableWrapper> saveables = new List<SaveableWrapper>();

	// validates singleton
	public static SaveGroups singleton {
		get {
			if( _singleton == null ) {
				_singleton = FindObjectOfType( typeof( SaveGroups ) ) as SaveGroups;

				if( _singleton == null ) {
					GameObject saveGroupObj = new GameObject( "Save Groups");
					_singleton = saveGroupObj.AddComponent<SaveGroups>();

					#if UNITY_EDITOR
					EditorSceneManager.MarkAllScenesDirty();
					#endif
				}
			}

			return _singleton;
		}
		set {
			_singleton = value;
		}
	}
	static SaveGroups _singleton;

	void Awake() {
		if( singleton != this ) {
			DestroyImmediate( this );
			return;
		}

		saveFolder = new DirectoryInfo( Application.persistentDataPath + "/" + saveFolderName );
		if( !saveFolder.Exists ) saveFolder.Create();

		// so you don't bother loading when testing
		if( Time.time < 1f ) {
			return;
		}

		// load serialized objects from file

		LoadAllFromFile();
	}

	public void LoadAll( string saveSlush ) {

		SerializedStringArray serializedSavedTypes = JsonUtility.FromJson<SerializedStringArray>( saveSlush );
		string[] savedTypes = serializedSavedTypes.contents;

		for( int typeIndex = 0; typeIndex < savedTypes.Length; typeIndex++ ) {

			if( typeIndex < saveables.Count ) { // don't overrun the array in case there is a mismatch between saved data and updated game data model
				SaveableWrapper wrapper = saveables[typeIndex];

				SerializedStringArray serializedEntries = JsonUtility.FromJson<SerializedStringArray>( savedTypes[typeIndex] );

				for( int entryIndex = 0; entryIndex < serializedEntries.contents.Length; entryIndex++ ) {

					if( entryIndex < wrapper.list.Count ) { // don't overrun the internal array
						string savedEntrySlush = serializedEntries.contents[entryIndex];
						if( string.IsNullOrEmpty( savedEntrySlush ) ) continue;

						wrapper.list[entryIndex].Load( savedEntrySlush );
					}
				}
			}
		}
	}

	public void LoadAllFromFile() {
		currentSaveName = PlayerPrefs.GetString("currentSaveName", "Untitled");

		FileInfo savedFile = new FileInfo( currentSavePath );
		if( !savedFile.Exists ) return;

		string saveSlush = File.ReadAllText( savedFile.FullName );

		LoadAll( saveSlush );
	}

	// returns id for object
	public static int Register( ISaveableComponent saveable ) {

		Type saveableType = saveable.GetType();

		// continue here

		for( int index = 0; index < singleton.saveables.Count; index++ ) {
			SaveableWrapper wrapper = singleton.saveables[index];

			if( wrapper.type == saveableType ) {
				int existingIndex = wrapper.list.IndexOf( saveable );
				if( existingIndex != -1 ) return existingIndex;

				wrapper.list.Add( saveable );
				MarkDirty();

				return wrapper.list.Count - 1;
			}
		}

		singleton.saveables.Add( new SaveableWrapper { type = saveableType, list = new List<ISaveableComponent>() { saveable } } );

		MarkDirty();

		return 0;
	}

	public static void Unregister( ISaveableComponent saveable ) {
		Type saveableType = saveable.GetType();

		for( int typeIndex = 0; typeIndex < singleton.saveables.Count; typeIndex++ ) {
			SaveableWrapper wrapper = singleton.saveables[typeIndex];

			if( wrapper.type == saveableType ) {
				for( int entryIndex = 0; entryIndex < singleton.saveables[typeIndex].list.Count; entryIndex++ ) {
					ISaveableComponent saved = wrapper.list[entryIndex];

					if( saved == saveable ) {
						saved = null;

						MarkDirty();
						return;
					}
				}
			}
		}

		MarkDirty();
	}

	public void Clean() {
		// remove nulls and reassign ids

		List<SaveableWrapper> oldSaveables = new List<SaveableWrapper>( saveables );
		saveables = new List<SaveableWrapper>();

		for( int typeIndex = 0; typeIndex < oldSaveables.Count; typeIndex++ ) {
			SaveableWrapper oldSaveable = oldSaveables[typeIndex];

			if( oldSaveable.list.Count > 0 ) {
				saveables.Add( new SaveableWrapper { type = oldSaveable.type, list = new List<ISaveableComponent>() } );

				for( int entryIndex = 0; entryIndex < oldSaveable.list.Count; entryIndex++ ) {
					ISaveableComponent newSaveable = oldSaveable.list[entryIndex];

					if( newSaveable != null ) {
						saveables[typeIndex].list.Add( newSaveable );
						newSaveable.id = entryIndex;
					}
				}
			}
		}
	}

	public T Get<T>( int id ) {
		for( int index = 0; index < saveables.Count; index++ ) {
			if( typeof(T) == saveables[index].type ) {
				return (T)saveables[index].list[id];
			}
		}

		return default(T);
	}

	public T GetInherited<T>( int searchForId ) {
		for( int typeIndex = 0; typeIndex < saveables.Count; typeIndex++ ) {
			SaveableWrapper saveable = saveables[typeIndex];

			if( typeof(T).IsInstanceOfType( saveable.type ) ) {
				for( int entryIndex = 0; entryIndex < saveable.list.Count; entryIndex++ ) {
					ISaveableComponent saved = saveable.list[entryIndex];

					if( saved.id == searchForId ) return (T)saved;
				}
			}
		}

		return default(T);
	}

	public List<T> GetAll<T>() {
		for( int index = 0; index < saveables.Count; index++ ) {
			if( saveables[index].type == typeof(T) ) {
				return saveables[index].list.Select( x => (T)x ).ToList();
			}
		}

		return new List<T>();
	}

	public List<T> GetAllInherited<T>() {
		List<T> listToReturn = new List<T>();

		for( int index = 0; index < saveables.Count; index++ ) {
			if( typeof(T).IsInstanceOfType( saveables[index].type ) ) {
				listToReturn.AddRange( saveables[index].list.Select( x => (T)x ).ToList() );
			}
		}

		return listToReturn;
	}

	public string SaveAll() {
		
		SerializedStringArray typeList = new SerializedStringArray {};
		typeList.contents = new string[saveables.Count];

		for( int index = 0; index < saveables.Count; index++ ) {
			SaveableWrapper saveable = saveables[index];

			SerializedStringArray entriesList = new SerializedStringArray {};
			entriesList.contents = new string[saveable.list.Count];

			for( int listIndex = 0; listIndex < saveable.list.Count; listIndex++ ) {
				if( saveable.list[listIndex] == null ) continue;

				entriesList.contents[listIndex] = saveable.list[listIndex].Save();
			}

			typeList.contents[index] = JsonUtility.ToJson( entriesList );
		}

		return JsonUtility.ToJson( typeList );
	}

	public void SaveAllToFile() {
		currentSaveName = PlayerPrefs.GetString("currentSaveName", "Untitled");

		File.WriteAllText( currentSavePath, SaveAll() );
	}

	public void DeleteSaveFile() {
		FileInfo savedFile = new FileInfo( currentSavePath );
		if( !savedFile.Exists ) return;

		savedFile.Delete();
	}

	public string SaveById<T>( int id ) {
		for( int index = 0; index < saveables.Count; index++ ) {
			if( saveables[index].type == typeof(T) ) {
				return saveables[index].list[id].Save();
			}
		}

		return "";
	}

	static void MarkDirty( ISaveableComponent saveable = null ) {
		#if UNITY_EDITOR
			EditorUtility.SetDirty( singleton );
		#endif
	}

	// self serialization

	// public void Reregister( ISaveableComponent saveable ) {

	// 	Type saveableType = saveable.GetType();

	// 	for( int index = 0; index < saveables.Count; index++ ) {
	// 		SaveableWrapper wrapper = saveables[index];

	// 		if( wrapper.type == saveableType ) {

	// 			// check if the wrapper's list count has been reset during deserialization
	// 			if( wrapper.list.Count == 0 && wrapper.listLength > 0 ) {
	// 				wrapper.list = new ISaveableComponent[wrapper.listLength].ToList();
	// 			}

	// 			wrapper.list[saveable.id] = saveable;
	// 		}
	// 	}
	// }

	// [HideInInspector] public string serializedGroups;

	// public void OnBeforeSerialize() {
	// 	serializedGroups = SerializeAll();

	// 	Debug.Log("Saving groups: " + serializedGroups);
	// }

	// public void OnAfterDeserialize() {
	// 	DeserializeAll( serializedGroups );

	// 	Debug.Log("Loading groups.  Have " + saveables.Count + " saveables.  Inside each saveable:");
	// 	for( int index = 0; index < saveables.Count; index++ ) {
	// 		Debug.Log("Saveable with type " + saveables[index].type + " has " + saveables[index].list.Count + " list items saved");
	// 	}

	// 	serializedGroups = null;
	// }

	// void SerializeAll() {
	// 	for( int index = 0; index < saveables.Count; index++ ) {
	// 		saveables[index].SerializeWrapper();
	// 	}
	// }

	// void DeserializeAll( string json ) {

	// }
}

[Serializable]
public class SaveableWrapper : ISerializationCallbackReceiver {
	public Type type {
		get {
			if( _type == null ) {
				_type = Type.GetType( serializableType );
			}

			return _type;
		}
		set {
			serializableType = value.AssemblyQualifiedName;
		}
	}
	Type _type;

	[HideInInspector] public string serializableType;

	public List<ISaveableComponent> list = new List<ISaveableComponent>();
	public List<UnityEngine.Object> serializableSaveables = new List<UnityEngine.Object>();

	public void OnBeforeSerialize() {
		serializableSaveables = list.Cast<UnityEngine.Object>().ToList();
	}

	public void OnAfterDeserialize() {
		list = serializableSaveables.Cast<ISaveableComponent>().ToList();
		serializableSaveables = null;
	}

	public string Save() {
		return serializableType;
	}
}

public struct SerializedStringArray { public string[] contents; }
