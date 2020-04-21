using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(GenerateWorld))]
public class EditorGenerateWorld : Editor {


  public override void OnInspectorGUI() {

    GenerateWorld createTiles = (GenerateWorld) target;
    DrawDefaultInspector();

    if(GUILayout.Button("Test")) {
      createTiles.TestFunction();
    }

    if (GUILayout.Button("Clear World")) {
      createTiles.ClearWorld();
    }

    if (GUILayout.Button("Generate Tiles")) {
      createTiles.GenerateTiles();
    }


  }





}













