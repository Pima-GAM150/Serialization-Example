using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( SaveGroups ) )]
public class SaveGroupsEditor : Editor {

	SaveGroups saveGroups;

	void OnEnable() {
		saveGroups = (SaveGroups)target;
	}

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		if( GUILayout.Button( "Reset All" ) ) {
			saveGroups.saveables = new List<SaveableWrapper>();
			
			var saveablesInScene = InterfaceHelper.FindObjects<ISaveableComponent>();
			foreach( ISaveableComponent saveableInScene in saveablesInScene ) {
				saveableInScene.id = SaveGroups.Register( saveableInScene );
			}
		}
	}
}
