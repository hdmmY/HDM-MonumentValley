using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{

    Node nodeScript;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        nodeScript = (Node)target;

        EditorGUILayout.Vector3Field("ScreenSpacePosition", nodeScript.ScreenSpacePosition);

        EditorGUILayout.Vector3Field("CameraSpacePosition", nodeScript.CameraSpacePosition);

        EditorGUILayout.FloatField("Depth", nodeScript.Depth);

        nodeScript.m_nodeType = (Node.NodeType)EditorGUILayout.EnumMaskField
                        ("NodeType", nodeScript.m_nodeType);

        nodeScript.m_walkableAxis = (Node.WalkableAxis)EditorGUILayout.EnumMaskField
                        ("WakableAxis", nodeScript.m_walkableAxis);

        EditorGUILayout.Space();


        // Display nodes that connected to
        System.Array adjPoints = System.Enum.GetValues(typeof(Node.ConnecPoint));
        foreach (Node.ConnecPoint connectType in adjPoints)
        {
            EditorGUILayout.LabelField(connectType.ToString());

            if (nodeScript.m_adjNodes == null) continue;
            if (nodeScript.m_adjNodes[connectType] == null) continue;

            EditorGUI.indentLevel++;
            foreach (var adjNode in nodeScript.m_adjNodes[connectType])
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField(adjNode.Value.ToString());
                EditorGUILayout.ObjectField(adjNode.Key, typeof(Node), true);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
    }
}
