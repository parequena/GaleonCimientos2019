using Unity.Collections;
using Unity.Jobs;

public struct CalcWithFibonacciJob : IJob
{
    private int m_divisor;
    public NativeArray<int> m_fibonacciSerie;
    public CalcWithFibonacciJob(int div, ref NativeArray<int> arr)
    {
        m_divisor = div;
        m_fibonacciSerie = arr;
    }
    private void FibonacciDivisor(int m_divisor)
    {
        for (int i = 0; i < m_fibonacciSerie.Length; ++i)
        {
            m_fibonacciSerie[i] = m_fibonacciSerie[i] / m_divisor;
        }
    }
    public void Execute()
    {
        FibonacciDivisor(m_divisor);
    }
}
