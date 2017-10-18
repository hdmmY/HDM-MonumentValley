using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public List<Node> m_WalkablePath;


    // Generate a path from player current position
    void GeneratePath()
    {
        m_WalkablePath.Clear();

        Node standNode = GetBottomNode();
        if (standNode == null) return;
        

    }


    private void DFSWalkablePath(Node curNode)
    {
        if (curNode == null) return;
        if (m_WalkablePath.Contains(curNode)) return;

        m_WalkablePath.Add(curNode);
        
    }


    // Get player bottom node
    private Node GetBottomNode()
    {
        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = -1f * transform.up;
        RaycastHit hitInfo;
        Physics.Raycast(ray, out hitInfo);

        if (hitInfo.collider == null)
        {
            Debug.LogWarning("Player is not stand on a node!");
            return null;
        }

        return hitInfo.collider.GetComponent<Node>();
    }
}
