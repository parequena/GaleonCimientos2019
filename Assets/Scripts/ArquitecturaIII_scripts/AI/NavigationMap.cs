using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationMap  : MonoBehaviour 
{
	public string WayPointTag = "Waypoint";
	public bool _buildNavigationMapAutomatic = false;
    private Map _waypointList;
    private List<WayPoint> _navigation;

    protected void Start () 
	{

        GameMgr.GetInstance().GetCustomMgrs().Register(this);
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag(WayPointTag);
        _waypointList = new Map(waypoints.Length);
        _navigation = new List<WayPoint>();
        for (int i = 0; i < waypoints.Length; ++i)
		{
            WayPoint wp = waypoints[i].GetComponent<WayPoint>();
            WayPointStruct wps = new WayPointStruct(wp.ID, wp.transform.position);
            _waypointList.Add(ref wps);
            _navigation.Add(wp);
        }

        for (int i = 0; i < waypoints.Length; ++i)
        {
            WayPoint wp = waypoints[i].GetComponent<WayPoint>();
            WayPoint[] neighborns = wp.getNeighbors();
            WayPointStruct wps = _waypointList[wp.ID];
            for (int j = 0; j < neighborns.Length; ++j)
            {
                WayPoint neighborn = neighborns[j];
                wps.AddNeighborn(neighborn.ID);
                WayPointStruct neighbornStatic = _waypointList[neighborn.ID];
                neighbornStatic.AddNeighborn(wps.ID);
                _waypointList[neighborn.ID] = neighbornStatic;
            }
            _waypointList[wp.ID] = wps;
        }

        /*if (_buildNavigationMapAutomatic)
			BuildNavigationPaths();*/
    }
	
	protected void OnDestroy()
	{
        //@TODO 2 Desregistrar el navigation map como servidor temporal.
        #region TODO2
        GameMgr.GetInstance().GetCustomMgrs().UnRegister<NavigationMap>();
        _waypointList.Release();
        #endregion
    }

    public Map getMap()
    {
        return _waypointList;
    }


    /*public WayPoint FindWayPointWithName(string name)
	{
		WayPoint ret = null;
		for (int i = 0; (i < _waypointList.Count && ret == null); ++i)
		{
			WayPoint wp = _waypointList[i];
			
			if(wp.name == name)
			{
				ret = wp;
			}
		}
		return ret;
	}*/

    /*public void Clean()
	{
		for ( int i = 0; i < (_waypointList.Count-1); ++i)
		{
			WayPoint w = (WayPoint) _waypointList[i];
			//w.Clean();
		}
	}*/

    /*public void BuildNavigationPaths()
	{
		DestroyNavigationPath();
		for ( int i = 0; i < (_waypointList.Count-1); ++i)
		{
			WayPoint wp1 =  _waypointList[i];
			for (int j = i+1; j < _waypointList.Count; ++j)
			{
				WayPoint wp2 = _waypointList[j];
                //@TODO 3 si es accesible enlazar ambos.
                #region TODO3
                if (wp1.IsAccesible(wp2))
                {
                    //wp1.AddNeighbor(wp2);
                    //wp2.AddNeighbor(wp1);
                }
                #endregion
            }
		}
	}*/


    public WayPoint FindWayPointNear(Vector3 position)
	{
        float minSqrDistance = float.MaxValue;
		WayPoint wayPoint = null;
		for ( int i = 0; i < _navigation.Count; ++i)
		{
			WayPoint wp1 = _navigation[i];
			Vector3 vDirector = wp1.transform.position - position;
			float sqrDistance = vDirector.sqrMagnitude;

            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = vDirector.sqrMagnitude;
                wayPoint = wp1;
            }

        }
		return wayPoint;
	}
	
	/*public WayPoint FindEndWayPoint(WayPoint init)
	{
		Hashtable visited = new Hashtable();
		WayPoint result = FindWayPointRecursive(init.GetInstanceID(),init, visited);
		return result;
	}*/
	
	/*public WayPoint FindWayPointRecursive(int id,WayPoint wp, Hashtable visited)
	{
		WayPoint result = null;
		if(visited[wp.name] == null)
		{
			List<WayPoint> neighbors = wp.GetNeighbors();
			visited.Add(wp.name,wp);
			for(int i = 0; (i < neighbors.Count) && (result == null); ++i)
			{
				WayPoint wpNext =  neighbors[i];
				if(visited[wpNext.name] == null)
				{
					if(wpNext.GetInstanceID() == id)
					{
						result = wpNext;
						visited.Add(wpNext.name,wpNext);
					}
					else
					{
						result = FindWayPointRecursive(id,wpNext,visited);
					}
				}
			}
		}
		return result;
	}*/
	
	/*public void DrawNavigationPath()
	{
		for ( int i = 0; i < _waypointList.Count; ++i)
		{
			WayPoint wp1 = (WayPoint) _waypointList[i];
			wp1.DrawLineAllNeighbor();
		}
	}*/
	
	
	
	/*private void DestroyNavigationPath()
	{
		for ( int i = 0; i < _waypointList.Count; ++i)
		{
			WayPoint wp =  _waypointList[i];
			//wp.DeleteNeighbor();
		}
	}*/
	
	
	
}


