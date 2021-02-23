using UnityEngine;

public class BTWait : BTBaseNode
{
	private float _waitSeconds;
	private float _currentDuration = 0;

	public BTWait(float seconds)
	{
		_waitSeconds = seconds;
	}

	public override BTNodeStatus Run()
	{
		_currentDuration += Time.fixedDeltaTime;
		if (_currentDuration < _waitSeconds)
		{
			return BTNodeStatus.Running;
		}
		_currentDuration = 0;
		return BTNodeStatus.Success;
	}
}