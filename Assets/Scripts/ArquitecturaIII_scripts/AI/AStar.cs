using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public struct AStar
{
    private int _target;
    private int _init;
    private Map _map;
    private NativeArray<int> _open;
    private NativeArray<Byte> _isOpen;
    private NativeArray<Byte> _isClose;
    private NativeArray<int> numWayPoints;
    private int _minQueue;
    private int _maxQueue;
    private NativeArray<Vector3> _path;

	
	public AStar(Map map, int init, int end, ref NativeArray<Vector3> nativeList, ref NativeArray<int> numElements)
	{
		_init = init;
		_target = end;
        _map = map;
        _open = new NativeArray<int>(map.Count, Allocator.TempJob);
        _isOpen = new NativeArray<Byte>(map.Count, Allocator.TempJob);
        _isClose = new NativeArray<Byte>(map.Count, Allocator.TempJob);
        numWayPoints = numElements;
        _path = nativeList;
        _minQueue = 0;
        _maxQueue = 0;
    }

    public void Delete()
    {
        _open.Dispose();
        _isOpen.Dispose();
        _isClose.Dispose();
    }

    public NativeArray<Vector3> Path
    {
        get { return _path; }
    }

    public void Process()
    {
        bool findGoal = false;
        _map.set(_init,0.0f,-1, _target);
        EnqueueOpen(_init);
        while (OpenSize > 0 && !findGoal)
        {
            int current = DequeueOpen();
            if (current != _target) //si no es el final...
            {
                SetClose(current);
                Neigborns successors = _map.GetNeighbors(current);
                for (int i = 0; i < successors.Count; ++i)
                {
                    int child = successors[i];
                    float gCostFromThisPath;// = _map.GetCostFromThisPath(current, child);
                    bool inOpen = IsOpen(child);
                    bool isClose = IsClose(child);
                    if (_map.NeedUpdatePath(current,child, inOpen, isClose, out gCostFromThisPath))
                    {
                        _map.set(child, gCostFromThisPath, current, _target);
                        if (!inOpen)
                        {
                            EnqueueOpen(child);
                        }
                    }
                }
            }
            else
            {
                findGoal = true;
            }
            Sort();
        }

        if (findGoal)
        {
            numWayPoints[0]=ReconstructPath(_target,0);   
        }

    }

    public void Sort()
    {
        //array.Sort(delegate (WayPoint c1, WayPoint c2) { return WayPoint.Compare(c1, c2); });
        for (int i = _minQueue; i < _maxQueue; i++)
        {
            for (int j = i + 1; j < _maxQueue; j++)
            {

                if (_map[_open[i]].F > _map[_open[j]].F)
                {
                    var aux = _open[i];
                    _open[i] = _open[j];
                    _open[j] = aux;
                }
            }
        }
    }

    private int ReconstructPath(int final,int count)
    {
        if (final > 0)
        {
            WayPointStruct wp = _map[final];
            _path[count] = wp.Position;
            return ReconstructPath(wp.Parent, count + 1);
        }
        else return count;
    }

    public bool IsOpen(int i)
    {
        return byte.MinValue != _isOpen[i];
    }

    public bool IsClose(int i)
    {
        return byte.MinValue != _isClose[i];
    }

    public void SetClose(int i)
    {
        _isClose[i] = byte.MaxValue;
    }
    /*Queue<int> q = new Queue<int>();
    q.Enqueue;
    q.Dequeue;
    q.Peek*/
    private int DequeueOpen()
    {
        int d = _open[_minQueue++];
        _isOpen[d] = byte.MaxValue;
        return d;
    }

    private int OpenSize
    {
        get { return _maxQueue - _minQueue; }
    }

    private int PeekOpen()
    {
        return _open[_minQueue];
    }

    private void EnqueueOpen(int wpID)
    {
        _open[_maxQueue++] = wpID;
        _isOpen[wpID] = byte.MaxValue;

    }

    /*private void SetNodeValues(WayPoint currentNode, int parent, float g)
    {
        currentNode.SetG(g);
        currentNode.SetH(currentNode.CalcF(_target));
        currentNode.SetFather(parent);
    }*/



    /*public WayPointStruct GetWaypoint(int i)
    {
        return (WayPointStruct)_path[i]; 
    }*/

    /*public void Sort(List<WayPoint>  array)
	{
		array.Sort(delegate(WayPoint c1, WayPoint c2) { return WayPoint.Compare(c1, c2);});
	}
	

	
	private bool IsInclose(WayPoint child)
	{
		return _close[child.name] != null;
	}
	
	
	
	private void ReconstructPath(WayPoint waypoint)
	{
		if(waypoint != null)
		{
			_path.Add(waypoint);
			ReconstructPath(waypoint.GetFather());
		}
	}


	

	*/






}


