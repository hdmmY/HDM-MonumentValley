﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Node : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    #region public properties         
    // The node's position in camera space
    public Vector3 CameraSpacePosition
    {
        get
        {
            if (_camera == null)
            {
                Debug.LogError("The \'_camera\' not initialize!");
                return Vector3.zero;
            }
            return _camera.worldToCameraMatrix * transform.position;
        }
    }


    public Vector3 ScreenSpacePosition
    {
        get
        {
            Vector3 position = transform.position;
            int intY = Mathf.Abs((int)position.y);
            for (int i = 0; i < intY; i++)
            {
                position.x += Mathf.Sign(position.y);
                position.z += Mathf.Sign(position.y);
            }
            return new Vector3(position.x, 0, position.z);
        }
    }


    // The node's depth in camera space
    public float Depth
    {
        get
        {
            return CameraSpacePosition.z;
        }
    }

    public Vector3 Centre
    {
        get
        {
            return transform.position;
        }
    }

    public Vector3 Up
    {
        get
        {
            return transform.position + transform.up * 0.5f;
        }
    }

    public Vector3 Down
    {
        get
        {
            return transform.position - transform.up * 0.5f;
        }
    }

    public Vector3 Right
    {
        get
        {
            return transform.position + transform.right * 0.5f;
        }
    }

    public Vector3 Left
    {
        get
        {
            return transform.position - transform.right * 0.5f;
        }
    }

    public Vector3 Forward
    {
        get
        {
            return transform.position + transform.forward * 0.5f;
        }
    }

    public Vector3 Back
    {
        get
        {
            return transform.position - transform.forward * 0.5f;
        }
    }

    public Vector3 UpperX
    {
        get
        {
            return transform.position + (transform.up + transform.right) * 0.5f;
        }
    }

    public Vector3 UpperNX
    {
        get
        {
            return transform.position + (transform.up - transform.right) * 0.5f;
        }
    }

    public Vector3 UpperZ
    {
        get
        {
            return transform.position + (transform.up + transform.forward) * 0.5f;
        }
    }

    public Vector3 UpperNZ
    {
        get
        {
            return transform.position + (transform.up - transform.forward) * 0.5f;
        }
    }

    public Vector3 MiddleXNZ
    {
        get
        {
            return transform.position + (transform.right - transform.forward) * 0.5f;
        }
    }

    public Vector3 MiddleXZ
    {
        get
        {
            return transform.position + (transform.right + transform.forward) * 0.5f;
        }
    }

    public Vector3 MiddleNXNZ
    {
        get
        {
            return transform.position - (transform.right + transform.forward) * 0.5f;
        }
    }

    public Vector3 MiddleNXZ
    {
        get
        {
            return transform.position + (transform.forward - transform.right) * 0.5f;
        }
    }

    public Vector3 DownX
    {
        get
        {
            return transform.position + (transform.right - transform.up) * 0.5f;
        }
    }

    public Vector3 DownNX
    {
        get
        {
            return transform.position - (transform.right + transform.up) * 0.5f;
        }
    }

    public Vector3 DownZ
    {
        get
        {
            return transform.position + (transform.forward - transform.up) * 0.5f;
        }
    }

    public Vector3 DownNZ
    {
        get
        {
            return transform.position - (transform.forward + transform.up) * 0.5f;
        }
    }

    public bool UpWalkable
    {
        get
        {
            return HasFlag((int)m_walkableAxis, (int)WalkableAxis.Up);
        }
    }

    public bool DownWalkable
    {
        get
        {
            return HasFlag((int)m_walkableAxis, (int)WalkableAxis.Down);
        }
    }

    public bool LeftWalkable
    {
        get
        {
            return HasFlag((int)m_walkableAxis, (int)WalkableAxis.Left);
        }
    }

    public bool RightWalkable
    {
        get
        {
            return HasFlag((int)m_walkableAxis, (int)WalkableAxis.Right);
        }
    }

    public bool ForwardWalkable
    {
        get
        {
            return HasFlag((int)m_walkableAxis, (int)WalkableAxis.Forward);
        }
    }

    public bool BackWalkable
    {
        get
        {
            return HasFlag((int)m_walkableAxis, (int)WalkableAxis.Back);
        }
    }


    /// <summary>
    /// Contain all node's poins for iterator
    /// </summary>
    private Dictionary<string, Vector3> PointsIterator
    {
        get
        {
            var result = new Dictionary<string, Vector3>();
            result.Add("Up", Up);
            result.Add("Down", Down);
            result.Add("Left", Left);
            result.Add("Right", Right);
            result.Add("Forward", Forward);
            result.Add("Back", Back);
            result.Add("UpperX", UpperX);
            result.Add("UpperNX", UpperNX);
            result.Add("UpperZ", UpperZ);
            result.Add("UpperNZ", UpperNZ);
            result.Add("MiddleXZ", MiddleXZ);
            result.Add("MiddleNXZ", MiddleNXZ);
            result.Add("MiddleXNZ", MiddleXNZ);
            result.Add("MiddleNXNZ", MiddleNXNZ);
            result.Add("DownX", DownX);
            result.Add("DownNX", DownNX);
            result.Add("DownZ", DownZ);
            result.Add("DownNZ", DownNZ);
            return result;
        }
    }

    #endregion


    #region public variables
    [System.Flags]
    public enum WalkableAxis
    {
        Up = 1,
        Down = 2,
        Right = 4,
        Left = 8,
        Forward = 16,
        Back = 32
    };
    [HideInInspector] public WalkableAxis m_walkableAxis;


    [System.Flags]
    public enum ConnecPoint
    {
        Upper_X = 1,
        Upper_NX = 2,
        Upper_Z = 4,
        Upper_NZ = 8,
        Middle_XNZ = 16,
        Middle_XZ = 32,
        Middle_NXNZ = 64,
        Middle_NXZ = 128,
        Down_X = 256,
        Down_NX = 512,
        Down_Z = 1024,
        Down_NZ = 2048
    };
    [HideInInspector] public ConnecPoint _adjPoints;


    [System.Flags]
    public enum NodeType
    {
        Special_Semi_Cube = 1,
        Ladder = 2
    };
    [HideInInspector] public NodeType m_nodeType;


    public Dictionary<ConnecPoint, List<AdjNode>> m_adjNodes;
    #endregion


    #region Public method
    public void AutoFindAdjPoint()
    {
        foreach(var adjPair in m_adjNodes)
        {
            adjPair.Value.Clear();
        }

        if ((WalkableAxis.Up & m_walkableAxis) == WalkableAxis.Up)
        {
            SetAdj(ConnecPoint.Upper_X, CastAdjRay(WalkableAxis.Right, WalkableAxis.Up));
            SetAdj(ConnecPoint.Upper_NX, CastAdjRay(WalkableAxis.Left, WalkableAxis.Up));
            SetAdj(ConnecPoint.Upper_Z, CastAdjRay(WalkableAxis.Forward, WalkableAxis.Up));
            SetAdj(ConnecPoint.Upper_NZ, CastAdjRay(WalkableAxis.Back, WalkableAxis.Up));
        }
    }

    #endregion


    #region Private connect method
    /// <summary>
    /// Check the adj node that walkable
    /// </summary>
    /// <param name="adjNodePos"> adj node to check </param>
    /// <param name="faceDir"> self node walkable face </param>
    /// <returns> adj node that walkable </returns>
    private List<AdjNode> CastAdjRay(WalkableAxis adjNodePos, WalkableAxis walkableFace)
    {
        Vector3 nodeCentre = transform.position;
        switch (adjNodePos)
        {
            case WalkableAxis.Up: nodeCentre += transform.up; break;
            case WalkableAxis.Down: nodeCentre -= transform.up; break;
            case WalkableAxis.Right: nodeCentre += transform.right; break;
            case WalkableAxis.Left: nodeCentre -= transform.right; break;
            case WalkableAxis.Forward: nodeCentre += transform.forward; break;
            case WalkableAxis.Back: nodeCentre -= transform.forward; break;
        }

        // get all walkable node
        Ray ray = new Ray();
        ray.direction = new Vector3(1, -1, 1);
        ray.origin = nodeCentre - ray.direction * 100;
        List<Node> originNodes = new List<Node>();
        foreach (var hitInfo in Physics.RaycastAll(ray, 300))
        {
            Node nodeCompn = hitInfo.collider.GetComponent<Node>();
            if (nodeCompn == null) continue;
            if (HasFlag((int)nodeCompn.m_walkableAxis, (int)walkableFace) &&
                nodeCompn.ScreenSpacePosition == WorldToScreen(nodeCentre))
            {
                originNodes.Add(nodeCompn);
            }
        }

        // Get legal node
        List<AdjNode> result = new List<AdjNode>();

        // origin.y+ walkable
        if (walkableFace == WalkableAxis.Up)
        {
            foreach (var node in originNodes)
            {
                ConnecPoint adjConnec = CheckUpWalkable(node, adjNodePos);
                if (adjConnec != 0) result.Add(new AdjNode(adjConnec, WalkableAxis.Up, node));
            }
            return result;
        }

        return result;
    }


    // Check whether adjNode'up face wakeable.
    // If walkable, return adjNode connect point type
    // if not, return 0
    private ConnecPoint CheckUpWalkable(Node node, WalkableAxis adjNodeType)
    {
        if ((WalkableAxis.Up & node.m_walkableAxis) != WalkableAxis.Up)
            return 0;  // equal to: return ConnectPoint.None


        switch (adjNodeType)
        {
            case WalkableAxis.Left:
                if (node.transform.position.y >= transform.position.y ||
                    HasFlag((int)node.m_nodeType, (int)NodeType.Special_Semi_Cube) ||
                    HasFlag((int)m_nodeType, (int)NodeType.Special_Semi_Cube))
                    return ConnecPoint.Upper_X;
                break;
            case WalkableAxis.Right:
                if (node.transform.position.y <= transform.position.y ||
                    HasFlag((int)node.m_nodeType, (int)NodeType.Special_Semi_Cube) ||
                    HasFlag((int)m_nodeType, (int)NodeType.Special_Semi_Cube))
                    return ConnecPoint.Upper_NX;
                break;
            case WalkableAxis.Back:
                if (node.transform.position.y >= transform.position.y ||
                   HasFlag((int)node.m_nodeType, (int)NodeType.Special_Semi_Cube) ||
                    HasFlag((int)m_nodeType, (int)NodeType.Special_Semi_Cube))
                    return ConnecPoint.Upper_Z;
                break;
            case WalkableAxis.Forward:
                if (node.transform.position.y <= transform.position.y ||
                    HasFlag((int)node.m_nodeType, (int)NodeType.Special_Semi_Cube) ||
                    HasFlag((int)m_nodeType, (int)NodeType.Special_Semi_Cube))
                    return ConnecPoint.Upper_NZ;
                break;
        }

        return 0;
    }




    // Set m_adjPoint
    private void SetAdj(ConnecPoint connecType, List<AdjNode> adjNodes)
    {
        // Delete adjPoint
        if (adjNodes == null || adjNodes.Count == 0)
        {
            _adjPoints &= ~connecType;
            m_adjNodes[connecType].Clear();
            return;
        }

        _adjPoints |= connecType;  // Add adjPoint
        foreach (AdjNode adjNodeToAdd in adjNodes)
        {
            if (!m_adjNodes[connecType].Contains(adjNodeToAdd))
            {
                m_adjNodes[connecType].Add(adjNodeToAdd);
            }
        }
    }
    #endregion


    #region static functions

    /// <summary>
    /// Get a node's adjpoint in world space
    /// </summary>
    /// <param name="node"></param>
    /// <param name="adjType"></param>
    /// <returns></returns>
    public static Vector3 GetAdjPointPosInWorld(Node node, ConnecPoint adjType)
    {
        if (adjType == ConnecPoint.Upper_X)
            return node.UpperX;
        if (adjType == ConnecPoint.Upper_Z)
            return node.UpperZ;
        if (adjType == ConnecPoint.Upper_NX)
            return node.UpperNX;
        if (adjType == ConnecPoint.Upper_NZ)
            return node.UpperNZ;
        if (adjType == ConnecPoint.Middle_NXNZ)
            return node.MiddleNXNZ;
        if (adjType == ConnecPoint.Middle_NXZ)
            return node.MiddleNXZ;
        if (adjType == ConnecPoint.Middle_XNZ)
            return node.MiddleXNZ;
        if (adjType == ConnecPoint.Middle_XZ)
            return node.MiddleXZ;
        if (adjType == ConnecPoint.Down_X)
            return node.DownX;
        if (adjType == ConnecPoint.Down_Z)
            return node.DownZ;
        if (adjType == ConnecPoint.Down_NX)
            return node.DownNX;
        if (adjType == ConnecPoint.Down_NZ)
            return node.DownNZ;

        return Vector3.zero;
    }

    // Transform world position into 2D screen position. 
    // (x_0, y_0, z_0) => (x, 0, z)
    public static Vector3 WorldToScreen(Vector3 worldPosition)
    {
        int intY = Mathf.Abs((int)worldPosition.y);
        for (int i = 0; i < intY; i++)
        {
            worldPosition.x += Mathf.Sign(worldPosition.y);
            worldPosition.z += Mathf.Sign(worldPosition.y);
        }
        return new Vector3(worldPosition.x, 0, worldPosition.z);
    }

    // Since unity don't have System.Enum.HasFlag method,
    // so I make a simple version of this method.
    public static bool HasFlag(int value, int flag)
    {
        return ((value & flag) == flag) ? true : false;
    }

    /// <summary>
    /// Get the connect point in specific walk axis
    /// </summary>                
    public static Node.ConnecPoint[] GetConnectTypeOnFace(Node.WalkableAxis face)
    {
        if (face == Node.WalkableAxis.Up)
        {
            return new Node.ConnecPoint[]
            {
                Node.ConnecPoint.Upper_X,
                Node.ConnecPoint.Upper_Z,
                Node.ConnecPoint.Upper_NX,
                Node.ConnecPoint.Upper_NZ
            };
        }

        if (face == Node.WalkableAxis.Down)
        {
            return new Node.ConnecPoint[]
            {
                Node.ConnecPoint.Down_X,
                Node.ConnecPoint.Down_Z,
                Node.ConnecPoint.Down_NX,
                Node.ConnecPoint.Down_NZ
            };
        }

        if (face == Node.WalkableAxis.Left)
        {
            return new Node.ConnecPoint[]
            {
                Node.ConnecPoint.Upper_NX,
                Node.ConnecPoint.Down_NX,
                Node.ConnecPoint.Middle_NXNZ,
                Node.ConnecPoint.Middle_NXZ
            };
        }

        if (face == Node.WalkableAxis.Right)
        {
            return new Node.ConnecPoint[]
            {
                Node.ConnecPoint.Upper_X,
                Node.ConnecPoint.Down_X,
                Node.ConnecPoint.Middle_XNZ,
                Node.ConnecPoint.Middle_XZ
            };
        }

        if (face == Node.WalkableAxis.Forward)
        {
            return new Node.ConnecPoint[]
            {
                Node.ConnecPoint.Upper_Z,
                Node.ConnecPoint.Down_Z,
                Node.ConnecPoint.Middle_NXZ,
                Node.ConnecPoint.Middle_XZ
            };
        }

        if (face == Node.WalkableAxis.Back)
        {
            return new Node.ConnecPoint[]
            {
                Node.ConnecPoint.Upper_NZ,
                Node.ConnecPoint.Down_NZ,
                Node.ConnecPoint.Middle_NXNZ,
                Node.ConnecPoint.Middle_XNZ
            };
        }

        return null;
    }

    /// <summary>
    /// Get the connect point in specific walk axis
    /// </summary>    
    public static Node.ConnecPoint[] GetConnectTypeOnFace(string faceName)
    {
        faceName = faceName.ToLower();

        if (faceName == "up")
            return GetConnectTypeOnFace(WalkableAxis.Up);
        if (faceName == "down")
            return GetConnectTypeOnFace(WalkableAxis.Down);
        if (faceName == "right")
            return GetConnectTypeOnFace(WalkableAxis.Right);
        if (faceName == "left")
            return GetConnectTypeOnFace(WalkableAxis.Left);
        if (faceName == "forward")
            return GetConnectTypeOnFace(WalkableAxis.Forward);
        if (faceName == "back")
            return GetConnectTypeOnFace(WalkableAxis.Back);

        return null;
    }

    #endregion


    // Init anchor child for path find
    private void InitAnchor()
    {
        GameObject temple = new GameObject();
        temple.transform.rotation = Quaternion.identity;

        foreach (var point in PointsIterator)
        {
            var go = Instantiate(temple, transform);
            go.transform.position = point.Value;
            go.name = point.Key;
        }
        Destroy(temple);

        m_hasInitAnchor = true;
    }

    public bool m_hasInitAnchor = false;
    private void Start()
    {
        m_adjNodes = new Dictionary<ConnecPoint, List<AdjNode>>();
        foreach (ConnecPoint connecPointType in System.Enum.GetValues(typeof(ConnecPoint)))
        {
            m_adjNodes.Add(connecPointType, new List<AdjNode>());
        }

        if (m_hasInitAnchor == false && m_walkableAxis != 0)
            InitAnchor();
    }

    private float nextTime = 0;
    private void Update()
    {
        if (Time.time > nextTime)
        {
            AutoFindAdjPoint();
            nextTime = Time.time + 0.1f;
            Debug.Log("Update");
        }
    }


    private void OnDrawGizmos()
    {
        float sphereSize = 0.15f;
        float cubeSize = 0.12f;

        // Draw the walkable node
        Gizmos.color = Color.blue;
        if (UpWalkable)
        {
            Gizmos.DrawSphere(Up, sphereSize);
            Gizmos.DrawLine(UpperX, UpperNX);
            Gizmos.DrawLine(UpperZ, UpperNZ);
        }
        if (DownWalkable)
        {
            Gizmos.DrawSphere(Down, sphereSize);
            Gizmos.DrawLine(DownX, DownNZ);
            Gizmos.DrawLine(DownZ, DownNZ);
        }
        if (RightWalkable)
        {
            Gizmos.DrawSphere(Right, sphereSize);
            Gizmos.DrawLine(MiddleXZ, MiddleXNZ);
            Gizmos.DrawLine(UpperX, DownX);
        }
        if (LeftWalkable)
        {
            Gizmos.DrawSphere(Left, sphereSize);
            Gizmos.DrawLine(MiddleNXZ, MiddleNXNZ);
            Gizmos.DrawLine(UpperNX, DownNX);
        }
        if (ForwardWalkable)
        {
            Gizmos.DrawSphere(Forward, sphereSize);
            Gizmos.DrawLine(MiddleXZ, MiddleNXZ);
            Gizmos.DrawLine(UpperZ, DownZ);
        }
        if (BackWalkable)
        {
            Gizmos.DrawSphere(Back, sphereSize);
            Gizmos.DrawLine(MiddleXNZ, MiddleNXNZ);
            Gizmos.DrawLine(UpperNZ, DownNZ);
        }


        // Draw the path connected cube
        Gizmos.color = Color.green;
        foreach (ConnecPoint adjPoint in System.Enum.GetValues(typeof(ConnecPoint)))
        {
            if ((_adjPoints & adjPoint) == adjPoint)
                Gizmos.DrawCube(GetAdjPointPosInWorld(this, adjPoint), Vector3.one * cubeSize);
        }

        // Draw the connected line
        if (m_adjNodes == null) return;
        foreach (var line in m_adjNodes)
        {
            if (line.Key == 0 || line.Value == null) continue;

            Vector3 start = GetAdjPointPosInWorld(this, line.Key);

            foreach (var endNode in line.Value)
            {
                Gizmos.DrawLine(start, GetAdjPointPosInWorld(
                    endNode.m_adjNode, endNode.m_adjNodeConnecPoint));
            }
        }
    }


    // Define the adjcent node property
    public class AdjNode
    {
        public ConnecPoint m_adjNodeConnecPoint;
        public WalkableAxis m_adjWalkAxis;
        public Node m_adjNode;

        public AdjNode(ConnecPoint adjConnecPoint, WalkableAxis adjWalkAxis, Node adjNode)
        {
            m_adjNodeConnecPoint = adjConnecPoint;
            m_adjWalkAxis = adjWalkAxis;
            m_adjNode = adjNode;
        }

        public override bool Equals(object obj)
        {
            return (((AdjNode)obj).m_adjNodeConnecPoint == m_adjNodeConnecPoint) &&
                   (((AdjNode)obj).m_adjNode == m_adjNode) &&
                   (((AdjNode)obj).m_adjWalkAxis == m_adjWalkAxis);
        }
    }

}

