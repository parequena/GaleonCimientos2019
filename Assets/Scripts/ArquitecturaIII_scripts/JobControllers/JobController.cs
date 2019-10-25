using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobController : MonoBehaviour
{
    private int m_num = 1000;
    private NativeArray<int> m_result;
    private FibonacciJob m_fibJob;
    private JobHandle m_handle;
    private bool m_init = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            LaunchJob();
        }

        if(m_init)
        {
            if(m_handle.IsCompleted)
            {
                m_handle.Complete();
                m_init = false;

                for (var i = 0; i < m_num; ++i)
                    Debug.Log(m_result[i]);

                m_result.Dispose();
            }
        }
    }

    protected void LaunchJob()
    {
        m_init = true;
        m_num = 1000;
        m_result = new NativeArray<int>(m_num, Allocator.TempJob);
        m_fibJob = new FibonacciJob(m_num, ref m_result);
        m_handle = m_fibJob.Schedule();
    }
}
