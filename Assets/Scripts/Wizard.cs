using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    public Phobject phobjectPrefab;

    public List<Phobject> phobjects;

    // Start is called before the first frame update
    void Start()
    {
        phobjects.Add(Instantiate<Phobject>(phobjectPrefab, Vector3.up * 500f, Quaternion.identity)
            .SetScale(Vector3.one * 3f)
            .SetDimensions(100, 100, 100, 20));
        phobjects.Add(Instantiate<Phobject>(phobjectPrefab, Vector3.up * 500f + Vector3.right * 500f, Quaternion.identity)
            .SetScale(Vector3.one * 3f)
            .SetDimensions(100, 100, 100, 20));
        phobjects.Add(Instantiate<Phobject>(phobjectPrefab, Vector3.up * 500f - Vector3.right * 500f, Quaternion.identity)
            .SetScale(Vector3.one * 3f)
            .SetDimensions(100, 100, 100, 20));
        phobjects.Add(Instantiate<Phobject>(phobjectPrefab, Vector3.up * 500f + Vector3.forward * 500f, Quaternion.identity)
            .SetScale(Vector3.one * 3f)
            .SetDimensions(100, 100, 100, 20));
        phobjects.Add(Instantiate<Phobject>(phobjectPrefab, Vector3.up * 500f + Vector3.right * 500f + Vector3.forward * 500f, Quaternion.identity)
            .SetScale(Vector3.one * 3f)
            .SetDimensions(100, 100, 100, 20));
        phobjects.Add(Instantiate<Phobject>(phobjectPrefab, Vector3.up * 500f - Vector3.right * 500f + Vector3.forward * 500f, Quaternion.identity)
            .SetScale(Vector3.one * 3f)
            .SetDimensions(100, 100, 100, 20));

        foreach (Phobject phobject in phobjects)
        {
            phobject.gameObject.AddComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
