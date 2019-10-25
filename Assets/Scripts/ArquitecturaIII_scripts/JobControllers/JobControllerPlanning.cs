using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
public class JobControllerPlanning : MonoBehaviour
{
    private NativeArray<int> result;
    private JobHandle handle;
    private JobHandle secondHandle;
    private bool init = false;
    private int num = 1000;
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            // LaunchJob();
            LaunchParallel();
        }
        if (init)
        {
            if (secondHandle.IsCompleted)
            {
                secondHandle.Complete();
                init = false;
                for (int i = 0; i < num; ++i)
                {
                    Debug.Log(result[i]);
                }
                result.Dispose();
            }
        }
    }

    protected void LaunchParallel()
    {
        init = true;
        result = new NativeArray<int>(num, Allocator.Persistent);
        FibonacciJob fibonacciJob = new FibonacciJob(num, ref result);
        CalcWithFinbonacciParallel calc = new CalcWithFinbonacciParallel(2, ref result);
        handle = fibonacciJob.Schedule();
        secondHandle = calc.Schedule(num, 100, handle);
    }

    protected void LaunchJob()
    {
        init = true;
        result = new NativeArray<int>(num, Allocator.Persistent);
        FibonacciJob fibJob = new FibonacciJob(num, ref result);
        CalcWithFibonacciJob calcWitJob = new CalcWithFibonacciJob(num, ref result);
        handle = fibJob.Schedule();
        secondHandle = calcWitJob.Schedule(handle);
    }
}