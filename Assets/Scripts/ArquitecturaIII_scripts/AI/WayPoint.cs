using System;
using UnityEngine;

public class WayPoint : MonoBehaviour 
{
    private static int __idGenerator__ = 0;
    public float _radiousDebug = 2.0f;

	public WayPoint[] _neighborsDefined;
	
	protected bool _isRunning = false;
    private int _id;

    void OnDrawGizmos() 
	{
		Color color = Gizmos.color ;
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position,_radiousDebug);
		Gizmos.color = color;
		DrawLineAllNeighbor();
	}
	
    public int ID
    {
        get { return _id; }
    }

	protected void Awake () 
	{
        _id = __idGenerator__++;
        _isRunning = true;
	}

    public WayPoint[] getNeighbors()
    {
        return _neighborsDefined;
    }

    public bool IsAccesible(WayPoint other)
	{
		bool ret = false;
		Vector3 directionNoNorm = other.transform.position - transform.position;
		Vector3 direction = directionNoNorm.normalized;
		RaycastHit hit;
		if(Physics.Raycast(transform.position,direction,out hit))
		{
			if(hit.collider.gameObject == other.gameObject)
			{
				Debug.Log("WayPoint 2:"+other.name+" con ide"+other.GetInstanceID()+" es accesible desde WayPoint 1:"+name);
				ret = true;
			}
		}
		return ret;
	}
	
	
	public bool IsPerpendicular(WayPoint other)
	{
		float xInc = transform.position.x - other.transform.position.x;
		float yInc = transform.position.z - other.transform.position.z;
		return (Math.Abs(xInc) < 5) || (Math.Abs(yInc) < 5);
	}
	
	public void DrawLineAllNeighbor()
	{
        if (_neighborsDefined == null)
            return;

        for (int i = 0; i < _neighborsDefined.Length; ++i)
		{
            if (_neighborsDefined[i] != null)
            {

                WayPoint wp = (WayPoint)_neighborsDefined[i];
                Debug.DrawLine(transform.position, wp.transform.position, Color.red);
            }
		}
		if(!_isRunning)
		{
			for(int i = 0; i < _neighborsDefined.Length; ++i)
			{
                if(_neighborsDefined[i] != null)
				    Debug.DrawLine(transform.position,_neighborsDefined[i].transform.position,Color.red);
			}
		}
	}

    private void OnDestroy()
    {
        __idGenerator__ = 0;
    }

}


