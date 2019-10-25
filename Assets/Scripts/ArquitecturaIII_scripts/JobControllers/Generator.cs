using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class Generator : MonoBehaviour
{
    public int numInstances;
    public GameObject prefab;
    public Transform target;
    public float speed;
    public float rangeMin;
    public float rangeMax;
    public bool paralell;

    Transform[] transforms;

    void Start()
    {
        transforms = new Transform[numInstances];
        for (int i = 0; i < numInstances; ++i)
        {
            Vector3 position = new Vector3(Random.Range(rangeMin, rangeMax), 0f, Random.Range(rangeMin, rangeMax));
            GameObject go = Instantiate(prefab, position, Quaternion.identity); transforms[i] = go.transform;
        }
    }

    // Update is called once per frame 
    void Update()
    {
        //MovingTransformsParallel 
        if (paralell)
            RunParallel();
        else
            RunSecuential();
    }

    private void RunParallel()
    {
        /*MovingTransformsParallel job = new MovingTransformsParallel(target.position, 1f, speed, Time.deltaTime);
        TransformAccessArray transAccArr = new TransformAccessArray(transforms);
        JobHandle handle = job.Schedule(transAccArr);
        handle.Complete();
        transAccArr.Dispose();
        */
    }

    private void RunSecuential()
    {
        for (int i = 0; i < transforms.Length; ++i)
        {
            Vector3 direction = target.position - transforms[i].position;
            if (direction.sqrMagnitude > 1f)
            {
                transforms[i].position = transforms[i].position + (direction.normalized * speed * Time.deltaTime);
            }
        }

    }

}
