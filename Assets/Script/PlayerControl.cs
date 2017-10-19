using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Transform m_standFace;
    public Transform m_targetFace;

    public List<Transform> m_pathDebug;

    /// <summary>
    /// Get player current standing axis.
    /// Return 0 if unkown the axis.
    /// </summary>
    public Node.WalkableAxis StandingAxis
    {
        get
        {
            if (transform.up == Vector3.up) return Node.WalkableAxis.Up;
            if (transform.up == Vector3.down) return Node.WalkableAxis.Down;
            if (transform.up == Vector3.right) return Node.WalkableAxis.Right;
            if (transform.up == Vector3.left) return Node.WalkableAxis.Left;
            if (transform.up == Vector3.forward) return Node.WalkableAxis.Forward;
            if (transform.up == Vector3.back) return Node.WalkableAxis.Back;

            return 0;  // unkown the standing axis
        }
    }

    [SerializeField]
    private bool _isMoving = false;

    [SerializeField]
    private Camera _camera;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray cameraRay = _camera.ScreenPointToRay(Input.mousePosition);
            m_targetFace = GetNodeFace(cameraRay);

            if(m_targetFace != null)
            {
                m_pathDebug = GetPath(m_standFace, m_targetFace);
            }
        }


        if (!_isMoving)
        {
            Ray playerDownRay = new Ray(transform.position, (-1f) * transform.up);
            m_standFace = GetNodeFace(playerDownRay);
        }
    }


    /// <summary>
    /// Get the node's face transfrom from a ray.
    /// </summary>
    /// <returns> Return walkable point transform. If hit point is not walkable, return null.</returns>
    private Transform GetNodeFace(Ray rayToDetect)
    {
        RaycastHit hitInfo;

        Physics.Raycast(rayToDetect, out hitInfo, 500f);

        Transform hitTrans = hitInfo.transform;
        if (hitTrans == null) return null;

        Node hitNode = hitTrans.GetComponent<Node>();
        if (hitNode == null) return null;

        Vector3 hitPoint = (hitInfo.point - hitTrans.position) * 2;

        if (Mathf.Approximately(hitPoint.x, 1) && hitNode.RightWalkable)
        {
            return hitTrans.Find("Right");
        }
        if (Mathf.Approximately(hitPoint.x, -1) && hitNode.LeftWalkable)
        {
            return hitTrans.Find("Left");
        }
        if (Mathf.Approximately(hitPoint.y, 1) && hitNode.UpWalkable)
        {
            return hitTrans.Find("Up");
        }
        if (Mathf.Approximately(hitPoint.y, -1) && hitNode.DownWalkable)
        {
            return hitTrans.Find("Down");
        }
        if (Mathf.Approximately(hitPoint.z, 1) && hitNode.ForwardWalkable)
        {
            return hitTrans.Find("Forward");
        }
        if (Mathf.Approximately(hitPoint.z, -1) && hitNode.BackWalkable)
        {
            return hitTrans.Find("Back");
        }

        return null;
    }


    private List<Transform> GetPath(Transform startTrans, Transform endTrans)
    {
        Node startNode = startTrans.parent.GetComponent<Node>();
        Node.WalkableAxis startAxis = StandingAxis;

        Node endNode = endTrans.parent.GetComponent<Node>();
        Node.WalkableAxis endAxis = Node.GetWalkAxisByName(endTrans.name);

        PathTree pathTree = new PathTree(new PathTreeNode(startNode, startAxis, null));
        pathTree.Init();
        return pathTree.FindPathFromStartToEnd(endNode, endAxis);
    }


    



    

}
