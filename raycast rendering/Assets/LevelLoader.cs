#if UNITY_EDITOR //Dont remove this, game wont build because idk, just keep it there
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Ill finish this, maybe..
public class LevelLoader : EditorWindow
{
    private Texture2D texture;
    [MenuItem("Window/Util/Map Creator")]

    public static void Initialize()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(LevelLoader), false, "Map Creator");
    }
    public static void ShowWindow()
    {
        GetWindow<LevelLoader>();
    }

    private void OnGUI()
    {
        if(EditorGUILayout.ObjectField("Image Texture", texture, typeof(Texture2D), false))
        {
            if(!texture.isReadable)
            {
                Debug.LogError("Image isnt readable!");
                return;
            }
            texture.filterMode = FilterMode.Point;
        }
    }
}
#endif
