using Unity.Collections;
using Unity.Jobs;

public struct FibonacciJob : IJob
{
    // Number of fibonacci number to calculate.
    private int m_n;

    // Array to store the fibonacci numbers.
    private NativeArray<int> m_nativeArray;

    // Ctor
    public FibonacciJob(int n, ref NativeArray<int> arr)
    {
        m_n = n;
        m_nativeArray = arr;
    }

    /// <summary>
    /// This function will be called on Job execution.
    /// </summary>
    public void Execute()
    {
        int aux = 0,
            b = 1,
            a = 0;

        for(var i = 0; i < m_n; ++i)
        {
            aux = a;
            a = b;
            b = aux + a;
            m_nativeArray[i] = a;
        }
    }

}
