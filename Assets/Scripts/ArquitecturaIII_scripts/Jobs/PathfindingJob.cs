using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct PathfindingJob : IJob
{

    private AStar _astar;

    public PathfindingJob(Map m,int init, int end, ref NativeArray<Vector3> nativeList, ref NativeArray<int> numelements)
    {
        _astar = new AStar(m, init, end, ref nativeList, ref numelements);
    }
    
    // Use this for initialization
    public void Execute()
    {
        _astar.Process();
    }

    public void Delete()
    {
        _astar.Delete();
    }
}
