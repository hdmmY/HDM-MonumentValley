using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                         
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
