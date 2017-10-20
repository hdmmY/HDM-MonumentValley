using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

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

        nodeScript.m_adjPoints = (Node.ConnecPoint)EditorGUILayout.EnumMaskField
                        ("ConnectPoint", nodeScript.m_adjPoints);

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        // Display nodes that connected to
        foreach (Node.ConnecPoint connectType in System.Enum.GetValues(typeof(Node.ConnecPoint)))
        {
            EditorGUILayout.EnumMaskField("selfAdjConnect", connectType);

            if (nodeScript.m_adjNodes == null) continue;
            if (nodeScript.m_adjNodes[connectType] == null) continue;
            //if (nodeScript.m_adjNodes[connectType].Count == 0) continue;
            
            EditorGUI.indentLevel++;
            foreach (var adjNode in nodeScript.m_adjNodes[connectType])
            {
                EditorGUILayout.ObjectField("adjNode", adjNode.m_adjNode, typeof(Node), true);
                EditorGUILayout.EnumMaskField("adjConnect", adjNode.m_adjNodeConnecPoint);
                EditorGUILayout.EnumMaskField("adjWalkAxis", adjNode.m_adjWalkAxis);
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
        }
    }
}
