using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveableComponent {

	int id { get; set; }

	string Save();
	void Load( string json );
	void Reset();
}
