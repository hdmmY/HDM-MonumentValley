using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private bool _isMoving = false;

    [SerializeField]
    private Camera _camera;

    public Transform m_standFace;
    public Transform m_targetFace;

    public float m_moveSpeed;
    public float m_distanceToGround;

    public List<PathNode> m_path;

    private Animator _animator;

    private int _curMoveStart;  // current move path start point tranform index
    private int _curMoveEnd;    // current move path end point transform index
    private float _sampleT;  // sample through the path

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
                
    private void Start()
    {
        m_path = new List<PathNode>();

        _animator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        // Determine movement
        if (Input.GetMouseButtonDown(0) && !_isMoving)
        {
            Ray cameraRay = _camera.ScreenPointToRay(Input.mousePosition);
            m_targetFace = GetNodeFace(cameraRay);

            if (m_targetFace != null)
            {
                m_path.Clear();

                List<Transform> pathNodeTrans = GetPath(m_standFace, m_targetFace);
                if (pathNodeTrans != null)
                {
                    foreach (var nodeTrans in pathNodeTrans)
                    {
                        m_path.Add(new PathNode(nodeTrans, nodeTrans.up));
                    }
                }

                if (m_path != null && m_path.Count > 0)
                {
                    _isMoving = true;
                    _curMoveStart = 0;
                    _curMoveEnd = 1;
                    _sampleT = 0;
                }
                else
                {
                    _isMoving = false;
                }
            }
        }


        // Standing waiting 
        if (!_isMoving)
        {
            Ray playerDownRay = new Ray(transform.position, (-1f) * transform.up);
            m_standFace = GetNodeFace(playerDownRay);

            _animator.SetFloat("Speed", 0);
        }
        else
        {
            MoveOnPath();

            _animator.SetFloat("Speed", m_moveSpeed);
            
        }
        
    }


    // Move player along the path.
    private void MoveOnPath()
    {
        Vector3 startPos = m_path[_curMoveStart].m_trans.position;
        Vector3 startNormal = m_path[_curMoveStart].m_worldNormal;
        Vector3 startScreenPos = Node.WorldToScreen(startPos);

        Vector3 endPos = m_path[_curMoveEnd].m_trans.position;
        Vector3 endNormal = m_path[_curMoveEnd].m_worldNormal;
        Vector3 endScreenPos = Node.WorldToScreen(endPos);

        // Start point and end point overlap
        while ((endScreenPos - startScreenPos).magnitude < 0.1f)
        {
            _curMoveStart += 1;
            _curMoveEnd += 1;

            startPos = m_path[_curMoveStart].m_trans.position;
            startNormal = m_path[_curMoveStart].m_worldNormal;
            startScreenPos = Node.WorldToScreen(startPos);

            endPos = m_path[_curMoveEnd].m_trans.position;
            endNormal = m_path[_curMoveEnd].m_worldNormal;
            endScreenPos = Node.WorldToScreen(endPos);
        }      

        Vector3 direction = endPos - startPos;

        float movement = m_moveSpeed * Time.deltaTime;

        float tmpT = _sampleT + movement / direction.magnitude;
        bool first = true;

        // If overhead, recalculate 
        while(tmpT > 1)
        {           
            // If at the end of the path, break
            if(_curMoveEnd == m_path.Count - 1)
            {
                tmpT = 1;
                break;
            }

            if(first)
            {
                movement -= (1 - _sampleT) * direction.magnitude;
                first = false;
            }
            else
            {
                movement -= direction.magnitude;
            }

            _curMoveStart += 1;
            _curMoveEnd += 1;

            startPos = m_path[_curMoveStart].m_trans.position;
            startNormal = m_path[_curMoveStart].m_worldNormal;
            startScreenPos = Node.WorldToScreen(startPos);

            endPos = m_path[_curMoveEnd].m_trans.position;
            endNormal = m_path[_curMoveEnd].m_worldNormal;
            endScreenPos = Node.WorldToScreen(endPos);

            direction = endPos - startPos;
            tmpT = movement / direction.magnitude;
        }
        _sampleT = tmpT;

        transform.up = Vector3.Lerp(startNormal, endNormal, _sampleT);
        transform.forward = direction.normalized;
        transform.position = Vector3.Lerp(startPos, endPos, _sampleT);
        transform.position += transform.up * m_distanceToGround;


        // At the path end, stop move.
        if((_curMoveEnd == m_path.Count - 1) && Mathf.Approximately(_sampleT, 1f))
        {
            _isMoving = false;
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
        List<Transform> pathTrans = pathTree.FindPathFromStartToEnd(endNode, endAxis);

        if (pathTrans == null) return null;

        return pathTrans;
    }



    private void OnDrawGizmos()
    {
        if (m_path == null || m_path.Count == 0)
            return;

        Gizmos.color = Color.black;
        foreach (var point in m_path)
        {
            Gizmos.DrawCube(point.m_trans.position, Vector3.one * 0.12f);
        }
    }


    [System.Serializable]
    public class PathNode
    {
        public Transform m_trans;
        public Vector3 m_worldNormal;

        public PathNode()
        {
            m_trans = null;
            m_worldNormal = Vector3.up;
        }

        public PathNode(Transform trans, Vector3 worldNormal)
        {
            m_trans = trans;
            m_worldNormal = worldNormal;
        }
    }

}
