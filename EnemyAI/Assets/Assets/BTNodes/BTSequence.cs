using TMPro;
using UnityEngine;

public class BTSequence : BTBaseNode
{
    private BTBaseNode[] nodes;
    private TextMeshProUGUI txt;
    private int i = 0;
    public BTSequence(TextMeshProUGUI txt, params BTBaseNode[] inputNodes)
    {
        this.txt = txt;
        nodes = inputNodes;
    }

    public override BTNodeStatus Run()
    {
        for(; i < nodes.Length; i++)
        {
            BTNodeStatus result = nodes[i].Run();
            switch (result)
            {
                case BTNodeStatus.Failed: return BTNodeStatus.Failed;
                case BTNodeStatus.Success: txt.text = nodes[i].GetType().Name; continue;
                case BTNodeStatus.Running: txt.text = nodes[i].GetType().Name; return BTNodeStatus.Running;
            }
        }
        i = 0;
        return BTNodeStatus.Success;
    }
}