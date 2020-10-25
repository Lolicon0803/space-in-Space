using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class ChangeFontWindow : EditorWindow
{
    [MenuItem("Tools/更換字體")]
    public static void Open()
    {
        EditorWindow.GetWindow(typeof(ChangeFontWindow));
    }

    Font toChange;
    static Font toChangeFont;
    FontStyle toFontStyle;
    static FontStyle toChangeFontStyle;

    void OnGUI()
    {
        toChange = (Font)EditorGUILayout.ObjectField(toChange, typeof(Font), true, GUILayout.MinWidth(100f));
        toChangeFont = toChange;
        toFontStyle = (FontStyle)EditorGUILayout.EnumPopup(toFontStyle, GUILayout.MinWidth(100f));
        toChangeFontStyle = toFontStyle;
        if (GUILayout.Button("更換"))
        {
            Change();
        }
    }

    public static void Change()
    {
        Transform canvas = GameObject.Find("MainMenu_Canvas").transform;
        if (!canvas)
        {
            Debug.Log("NO Canvas");
            return;
        }
        Transform[] tArray = canvas.GetComponentsInChildren<Transform>();
        for (int i = 0; i < tArray.Length; i++)
        {
            Text t = tArray[i].GetComponent<Text>();
            if (t)
            {
                Undo.RecordObject(t, t.gameObject.name);
                t.font = toChangeFont;
                t.fontStyle = toChangeFontStyle;
                EditorUtility.SetDirty(t);
            }
        }  
        Debug.Log("Succed");  
    }
}