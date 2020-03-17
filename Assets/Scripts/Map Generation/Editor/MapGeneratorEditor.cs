using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]    //this affects MapGenerator Scripts
public class MapGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		MapGenerator mapGen = (MapGenerator) target; //target is what component this script affects

		if (DrawDefaultInspector()) { //function draws the inspector, and if something is changed it returns true
			if (mapGen.autoUpdate) {    //if we want to autoUpdate without having to press the button
				mapGen.GenerateMap();   //generate our map
			}
		}

		if (GUILayout.Button("Generate")) {   //if a button called generate is pressed
			mapGen.GenerateMap();   //generate our map	
		}
	}
}
