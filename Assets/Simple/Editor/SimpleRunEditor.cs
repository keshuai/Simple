using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleRun))]
public class SimpleRunEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var _this = this.target as SimpleRun;
        if (GUILayout.Button("StartRun"))
        {
            _this.StartRun();
        }
        if (GUILayout.Button("StopRun"))
        {
            _this.StopRun();
        }
        GUILayout.Space(5);
        if (GUILayout.Button("ShowHelpLine"))
        {
            _this.ShowHelperLine();
        }
        if (GUILayout.Button("HideHelpLine"))
        {
            _this.HideHelperLine();
        }
    }
}
