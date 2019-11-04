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
        phobjects.Add(Instantiate<Phobject>(phobjectPrefab, Vector3.up * 32f, Quaternion.identity)
            .SetScale(Vector3.one)
            .SetDimensions(16, 64, 4, 4));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
