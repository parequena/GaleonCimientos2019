using Unity.Collections;
using UnityEngine;

public struct Map  {


    private NativeArray<WayPointStruct> _waypoints;
   

    public Map(int numWaypoints)
    {
        _waypoints = new NativeArray<WayPointStruct>(numWaypoints, Allocator.Persistent);
    }

    public int Count
    {
        get { return _waypoints.Length; }
    }


    public Neigborns GetNeighbors(int i)
    {
        return _waypoints[i].GetNeigborns();
    }

    public void Add(ref WayPointStruct w)
    {
        _waypoints[w.ID] = w;
    }

    public WayPointStruct this[int i]
    {
        get { return _waypoints[i]; }
        set { _waypoints[i] = value; }
    }

    public void set(int i, float g, int parent, int target)
    {
        WayPointStruct wp = _waypoints[i];
        wp.G= g;
        wp.Parent = parent;
        wp.H = wp.CalcF(_waypoints[target].Position);
        _waypoints[i] = wp;
    }

    public bool NeedUpdatePath(int current, int child, bool wpChildIsClose, bool wpChildIsOpen, out float gCostFromThisPath)
    {
        WayPointStruct wpCurrent = _waypoints[current];
        WayPointStruct wpChild = _waypoints[child];
        float f = wpCurrent.CalcF(wpChild.Position);
        gCostFromThisPath = wpCurrent.G + f;
       
        if (!(wpChildIsClose && gCostFromThisPath >= wpChild.G))
        {
            //Si el nodo seleccionado no está en la lista de _abierta o el coste del seleccionado por el camino
            // actual es menor que el coste anterior....
            return (!wpChildIsOpen) || (gCostFromThisPath < wpChild.G);
        }

        return false;
    }

    public float GetCostFromThisPath(int current, int child)
    {
        //float gCostFromThisPath = current.GetG() + current.CalcF(child);
        WayPointStruct wp = _waypoints[current];
        float f = wp.CalcF(_waypoints[child].Position);
        return wp.G + f;
    }

    public void Release()
    {
        _waypoints.Dispose();
    }
}
