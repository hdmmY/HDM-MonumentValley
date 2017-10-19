using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Transform m_standFace;

    public Transform m_targetFace;

    [SerializeField]
    private bool _isMoving = false;

    [SerializeField]
    private Camera _camera;


    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray cameraRay = _camera.ScreenPointToRay(Input.mousePosition);
            m_targetFace = GetNodeFace(cameraRay);
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


    //private List<Transform> GetPath(Transform startTrans, Transform endTrans)
    //{
    //    PathTreeNode starNode = new PathTreeNode(startTrans, null);

    //    List<Node> visited = new List<Node>();
    //    visited.Add(startTrans.parent.GetComponent<Node>());

    //    Stack<PathTreeNode> treeNodeStack = new Stack<PathTreeNode>();
    //    treeNodeStack.Push(starNode);

    //    while (treeNodeStack.Count > 0)
    //    {
    //        foreach (Node.ConnecPoint connecType in )
    //    }
    //}

    


    /// <summary>
    /// Basic path tree node
    /// </summary>
    public class PathTreeNode
    {
        public Transform m_data;

        private List<PathTreeNode> _nodes;

        public PathTreeNode m_father;

        /// <summary>
        /// Create a tree node. 
        /// </summary>                   
        public PathTreeNode(Transform data, PathTreeNode father)
        {
            m_data = data;
            m_father = father;
            _nodes = new List<PathTreeNode>(2);
        }

        /// <summary>
        /// Add a child to this tree node. If add fail, return false.
        /// </summary>                 
        public bool AddChild(PathTreeNode node)
        {
            if (_nodes == null)
                _nodes = new List<PathTreeNode>(2);

            if (_nodes.Count == 0)
            {
                _nodes.Add(node);
                return true;
            }

            if (_nodes.Contains(node))
            {
                return false;
            }

            _nodes.Add(node);
            return true;
        }

        /// <summary>
        /// Iterator the node childs.
        /// </summary>
        public IEnumerator GetChilds()
        {
            int length = _nodes.Count;

            for (int i = 0; i < length; i++)
            {
                int curLength = _nodes.Count;
                if (curLength != length)
                {
                    Debug.LogError("Iterator break.");
                    yield break;
                }
                yield return _nodes[i];
            }
        }
    }

}
