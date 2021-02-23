using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BTNodeStatus { Success, Failed, Running }
public abstract class BTBaseNode
{
    public abstract BTNodeStatus Run();
}
