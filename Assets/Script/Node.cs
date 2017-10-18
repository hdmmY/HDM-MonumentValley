using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Node : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

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

   
    public Dictionary<ConnecPoint, Dictionary<Node, ConnecPoint>> m_adjNodes;

    public void AutoFindAdjPoint()
    {
        if ((WalkableAxis.Up & m_walkableAxis) == WalkableAxis.Up)
        {
            SetAdj(ConnecPoint.Upper_X, CastAdjRay(WalkableAxis.Right, WalkableAxis.Up));
            SetAdj(ConnecPoint.Upper_NX, CastAdjRay(WalkableAxis.Left, WalkableAxis.Up));
            SetAdj(ConnecPoint.Upper_Z, CastAdjRay(WalkableAxis.Forward, WalkableAxis.Up));
            SetAdj(ConnecPoint.Upper_NZ, CastAdjRay(WalkableAxis.Back, WalkableAxis.Up));
        }
    }


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
            if ((nodeCompn.m_walkableAxis & walkableFace) == walkableFace &&
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
                if (adjConnec != 0) result.Add(new AdjNode(adjConnec, node));
                //Debug.Log(this.name + "   " + node.name + "    " + adjConnec);
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


    // Transform world position into 2D screen position. 
    // (x_0, y_0, z_0) => (x, 0, z)
    private Vector3 WorldToScreen(Vector3 worldPosition)
    {
        int intY = Mathf.Abs((int)worldPosition.y);
        for (int i = 0; i < intY; i++)
        {
            worldPosition.x += Mathf.Sign(worldPosition.y);
            worldPosition.z += Mathf.Sign(worldPosition.y);
        }
        return new Vector3(worldPosition.x, 0, worldPosition.z);
    }


    private void Start()
    {
        m_adjNodes = new Dictionary<ConnecPoint, Dictionary<Node, ConnecPoint>>();
        foreach (ConnecPoint connecPointType in System.Enum.GetValues(typeof(ConnecPoint)))
        {
            m_adjNodes.Add(connecPointType, new Dictionary<Node, ConnecPoint>());
        }

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
            m_adjNodes[connecType][adjNodeToAdd.m_adjNode] = adjNodeToAdd.m_adjNodeConnecPoint;
        }
    }


    // Since unity don't have System.Enum.HasFlag method,
    // so I make a simple version of this method.
    private bool HasFlag(int value, int flag)
    {
        return ((value & flag) == 1) ? true : false;
    }

    private float nextTime = 0;
    private void Update()
    {
        if (Time.time > nextTime)
        {
            AutoFindAdjPoint();
            nextTime = Time.time + 2f;
            Debug.Log("Update");
        }
    }


    private void OnDrawGizmos()
    {
        Vector3 centre = transform.position;
        Vector3 up = centre + transform.up * 0.51f;
        Vector3 down = centre + transform.up * (-1) * 0.51f;
        Vector3 right = centre + transform.right * 0.51f;
        Vector3 left = centre + transform.right * (-1) * 0.51f;
        Vector3 forward = centre + transform.forward * 0.51f;
        Vector3 back = centre + transform.forward * (-1) * 0.51f;

        float sphereSize = 0.15f;
        float cubeSize = 0.12f;

        // Draw the walkable node
        Gizmos.color = Color.blue;
        if ((WalkableAxis.Up & m_walkableAxis) == WalkableAxis.Up)
        {
            Gizmos.DrawSphere(up, sphereSize);
            Gizmos.DrawLine(up - transform.right * 0.5f, up + transform.right * 0.5f);
            Gizmos.DrawLine(up - transform.forward * 0.5f, up + transform.forward * 0.5f);
        }
        if ((WalkableAxis.Down & m_walkableAxis) == WalkableAxis.Down)
        {
            Gizmos.DrawSphere(down, sphereSize);
            Gizmos.DrawLine(down - transform.right * 0.5f, down + transform.right * 0.5f);
            Gizmos.DrawLine(down - transform.forward * 0.5f, down + transform.forward * 0.5f);
        }
        if ((WalkableAxis.Right & m_walkableAxis) == WalkableAxis.Right)
        {
            Gizmos.DrawSphere(right, sphereSize);
            Gizmos.DrawLine(right - transform.up * 0.5f, right + transform.up * 0.5f);
            Gizmos.DrawLine(right - transform.forward * 0.5f, right + transform.forward * 0.5f);
        }
        if ((WalkableAxis.Left & m_walkableAxis) == WalkableAxis.Left)
        {
            Gizmos.DrawSphere(left, sphereSize);
            Gizmos.DrawLine(left - transform.up * 0.5f, left + transform.up * 0.5f);
            Gizmos.DrawLine(left - transform.forward * 0.5f, left + transform.forward * 0.5f);
        }
        if ((WalkableAxis.Forward & m_walkableAxis) == WalkableAxis.Forward)
        {
            Gizmos.DrawSphere(forward, sphereSize);
            Gizmos.DrawLine(forward - transform.up * 0.5f, forward + transform.up * 0.5f);
            Gizmos.DrawLine(forward - transform.right * 0.5f, forward + transform.right * 0.5f);
        }
        if ((WalkableAxis.Back & m_walkableAxis) == WalkableAxis.Back)
        {
            Gizmos.DrawSphere(back, sphereSize);
            Gizmos.DrawLine(back - transform.up * 0.5f, back + transform.up * 0.5f);
            Gizmos.DrawLine(back - transform.right * 0.5f, back + transform.right * 0.5f);
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
                Gizmos.DrawLine(start, GetAdjPointPosInWorld(endNode.Key, endNode.Value));
            }
        }
    }


    Vector3 GetAdjPointPosInWorld(Node node, ConnecPoint adjType)
    {
        Vector3 centre = node.transform.position;
        Vector3 up = centre + node.transform.up * 0.51f;
        Vector3 down = centre + node.transform.up * (-1) * 0.51f;
        Vector3 right = centre + node.transform.right * 0.51f;
        Vector3 left = centre + node.transform.right * (-1) * 0.51f;
        Vector3 forward = centre + node.transform.forward * 0.51f;
        Vector3 back = centre + node.transform.forward * (-1) * 0.51f;

        if (adjType == ConnecPoint.Upper_X)
            return up + node.transform.right * 0.5f;
        if (adjType == ConnecPoint.Upper_Z)
            return up + node.transform.forward * 0.5f;
        if (adjType == ConnecPoint.Upper_NX)
            return up - node.transform.right * 0.5f;
        if (adjType == ConnecPoint.Upper_NZ)
            return up - node.transform.forward * 0.5f;
        if (adjType == ConnecPoint.Middle_NXNZ)
            return centre - node.transform.right * 0.5f - node.transform.forward * 0.5f;
        if (adjType == ConnecPoint.Middle_NXZ)
            return centre - node.transform.right * 0.5f + node.transform.forward * 0.5f;
        if (adjType == ConnecPoint.Middle_XNZ)
            return centre + node.transform.right * 0.5f - node.transform.forward * 0.5f;
        if (adjType == ConnecPoint.Middle_XZ)
            return centre + node.transform.right * 0.5f + node.transform.forward * 0.5f;
        if (adjType == ConnecPoint.Down_X)
            return down + node.transform.right * 0.5f;
        if (adjType == ConnecPoint.Down_Z)
            return down + node.transform.forward * 0.5f;
        if (adjType == ConnecPoint.Down_NX)
            return down - node.transform.right * 0.5f;
        if (adjType == ConnecPoint.Down_NZ)
            return down - node.transform.forward * 0.5f;

        return Vector3.zero;
    }


    // Define the adjcent node property
    public class AdjNode
    {
        public ConnecPoint m_adjNodeConnecPoint;
        public Node m_adjNode;

        public AdjNode(ConnecPoint adjConnecPoint, Node adjNode)
        {
            m_adjNodeConnecPoint = adjConnecPoint;
            m_adjNode = adjNode;
        }
    }

}

