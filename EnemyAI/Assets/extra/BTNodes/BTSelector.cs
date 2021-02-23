using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSelector : BTBaseNode
{
    protected List<BTBaseNode> m_nodes = new List<BTBaseNode>();

    public BTSelector(List<BTBaseNode> nodes)
    {
        m_nodes = nodes;
    }
    public override BTNodeStatus Run()
    {
        foreach (BTBaseNode node in m_nodes)
        {
            switch (node.Run())
            {
                case BTNodeStatus.Failed:
                    continue;
                case BTNodeStatus.Success:
                    return BTNodeStatus.Success; 
                case BTNodeStatus.Running:
                    return BTNodeStatus.Running;
                default:
                    continue;
            }
        }
        return BTNodeStatus.Failed;
    }
}
