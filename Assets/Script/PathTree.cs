using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTree : MonoBehaviour
{

    public PathTreeNode m_head;

    public PathTree()
    {
        m_head = null;
    }

    public PathTree(PathTreeNode head)
    {
        m_head = head;
    }

    public void Init()
    {
        if (m_head == null)
        {
            Debug.LogError("Init tree fail! 'm_head' is not set!");
            return;
        }

        m_head = RecusionGenerateTree(m_head, new List<Node>());
    }

    /// <summary>
    /// Get a tree node from the tree.
    /// </summary>                    
    /// <returns> return null if there is no such tree node </returns>
    public PathTreeNode FindTreeNode(Node nodeToFind, Node.WalkableAxis axisToFind)
    {
        if (m_head == null) return null;

        Queue<PathTreeNode> queue = new Queue<PathTreeNode>();
        queue.Enqueue(m_head);

        while (queue.Count > 0)
        {
            PathTreeNode treeNode = queue.Dequeue();
            if (treeNode == null) continue;

            foreach (var childTreeNode in treeNode.GetChilds())
            {
                if (childTreeNode == null) continue;

                if ((childTreeNode.m_node == nodeToFind) &&
                    (childTreeNode.m_axis == axisToFind))
                {
                    return childTreeNode;
                }
                else
                {
                    queue.Enqueue(childTreeNode);
                }
            }
        }

        return null;
    }


    /// <summary>
    /// Get a path, from head to endNode
    /// </summary>           
    public List<Transform> FindPathFromStartToEnd(Node endNode, Node.WalkableAxis endAxis)
    {
        PathTreeNode curTreeNode = FindTreeNode(endNode, endAxis);

        if (curTreeNode == null) return null;

        List<Transform> path = new List<Transform>();
        while(curTreeNode != m_head)
        {
            Node curNode = curTreeNode.m_node;
            Node.WalkableAxis curAxis = curTreeNode.m_axis;

            Node fatherNode = curTreeNode.m_father.m_node;
            Node.WalkableAxis fatherAxis = curTreeNode.m_father.m_axis;

            Transform curConnect = curNode.FindConnectTrans(fatherNode);
            Transform adjConnect = fatherNode.FindConnectTrans(curNode);

            path.Add(curNode.GetTransByAxis(curAxis));
            path.Add(curConnect);
            path.Add(adjConnect);
            path.Add(fatherNode.GetTransByAxis(fatherAxis));
        }

        path.Reverse();
        return path;
    }


    /// <summary>
    /// Recusion generate path tree
    /// </summary>                   
    private PathTreeNode RecusionGenerateTree(PathTreeNode father, List<Node> visited)
    {
        if (father == null) return null;

        var connecPoints = Node.GetConnectTypeOnFace(father.m_axis);
        if (connecPoints == null) return father;

        foreach (var connecPoint in connecPoints)
        {
            List<Node.AdjNodeInfo> nodeInfoList = father.m_node.m_adjNodes[connecPoint];

            if (nodeInfoList.Count == 0) continue;

            foreach (var nodeInfo in nodeInfoList)
            {
                if (visited.Contains(nodeInfo.m_adjNode)) continue;

                PathTreeNode newTreeNode = new PathTreeNode(nodeInfo.m_adjNode,
                                                            nodeInfo.m_adjWalkAxis,
                                                            father);
                visited.Add(newTreeNode.m_node);
                father.AddChild(newTreeNode);

                newTreeNode = RecusionGenerateTree(newTreeNode, visited);
            }
        }

        return father;
    }




}

/// <summary>
/// Basic path tree node
/// </summary>
public class PathTreeNode
{
    public Node m_node;
    public Node.WalkableAxis m_axis;

    private List<PathTreeNode> _nodes;

    public PathTreeNode m_father;

    public int ChildCout
    {
        get
        {
            return _nodes.Count;
        }
    }

    /// <summary>
    /// Create a tree node. 
    /// </summary>                   
    public PathTreeNode(Node node, Node.WalkableAxis axis, PathTreeNode father)
    {
        m_node = node;
        m_axis = axis;
        m_father = father;
        _nodes = new List<PathTreeNode>();
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
    public IEnumerable<PathTreeNode> GetChilds()
    {
        int length = _nodes.Count;

        if (length == 0)
        {
            yield return null;
            yield break;
        }

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

    public override bool Equals(object obj)
    {
        PathTreeNode node = (PathTreeNode)obj;
        return (node.m_node == m_node) &&
               (node.m_father == m_father) &&
               (node.m_axis == m_axis);
    }
}
