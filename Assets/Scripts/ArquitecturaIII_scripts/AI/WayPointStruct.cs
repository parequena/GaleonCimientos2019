using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public struct Neigborns
{
    private int n0;
    private int n1;
    private int n2;
    private int n3;
    private int numNeighborns;

    public int this[int i]
    {
        get
        {
            if (i >= numNeighborns)
            {
                Debug.LogError("A Waypoint only can have 4 neighborns "+i);
                return -1;
            }
            switch (i)
            {
                case 0:
                    return n0;
                case 1:
                    return n1;
                case 2:
                    return n2;
                case 3:
                    return n2;
                default:
                    Debug.LogError("A Waypoint only can have 4 neighborns "+i);
                    return -1;
            }
        }
        set
        {
            if (i > numNeighborns)
            {
                Debug.LogError("A Waypoint only can have 4 neighborns "+i);
            }
            switch (i)
            {
                case 0:
                    n0 = value;
                    break;
                case 1:
                    n1 = value;
                    break;
                case 2:
                    n2 = value;
                    break;
                case 3:
                    n2 = value;
                    break;
                default:
                    Debug.LogError("A Waypoint only can have 4 neighborns "+1);
                    break;
            }
        }
    }

    public void Add(int neig)
    {
        this[numNeighborns] = neig;
        numNeighborns++;
    }

    public int Count
    {
        get { return numNeighborns; }
    }
}

public struct WayPointStruct
{
    private int _id;
    private float h;
    private float _g;
    private int _parent;
    private Vector3 _position;
    private Neigborns _neigborns;

    public WayPointStruct(int id, Vector3 pos)
    {
        _position = pos;
        _g = 0f;
        h = 0f;
        _id = id;
        _parent = -1;
        _neigborns = new Neigborns();
    }

    public int Parent
    {
        get { return _parent; }
        set { _parent = value; }
    }

    public int ID
    {
        get { return _id; }
    }

    public Vector3 Position
    {
        get { return _position; }
        set { _position = value; }
    }
    public float F
    {
        get { return G + H; }
    }

    public float H
    {
        get { return h; }
        set { h = value; }
    }

    public float G
    {
        get { return _g; }
        set { _g = value; }
    }



    public float CalcF(Vector3 w)
    {
        Vector3 vdirection = w - _position;
        return vdirection.magnitude;
    }

    public void AddNeighborn(int neigh)
    {
        _neigborns.Add(neigh);
    }

    public Neigborns GetNeigborns()
    {
        return _neigborns;
    }
    
    public int NumNeighborns
    {
        get { return _neigborns.Count; }
    }

}
