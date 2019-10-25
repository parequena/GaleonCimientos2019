using Unity.Collections;
using Unity.Jobs;

public struct CalcWithFinbonacciParallel : IJobParallelFor
{
    private int divisor;
    public NativeArray<int> fibonacciSerie;
    public CalcWithFinbonacciParallel(int a_divisor, ref NativeArray<int> arr)
    {
        divisor = a_divisor;
        fibonacciSerie = arr;
    }
    public void Execute(int index)
    {
        fibonacciSerie[index] = fibonacciSerie[index] / divisor;
    }
}