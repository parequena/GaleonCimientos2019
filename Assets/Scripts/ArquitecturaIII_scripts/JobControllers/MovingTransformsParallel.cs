using UnityEngine;
using UnityEngine.Jobs;

public struct MovingTransformsParallel : IJobParallelForTransform
{
    private Vector3 target;
    private float distance;
    private float speed;
    private float time;

    public MovingTransformsParallel(Vector3 a_target, float dis, float spe, float t)
    {
        target = a_target;
        distance = dis;
        speed = spe;
        time = t;
    }

    public void Execute(int index, TransformAccess transform)
    {
        Vector3 direction = target - transform.position;
        if (direction.magnitude > distance)
        {
            transform.position = transform.position + (direction.normalized * speed * time);
        }
    }
}