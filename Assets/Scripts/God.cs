using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    public BlockGroupFactory factoryPrefab;

    // Start is called before the first frame update
    void Start()
    {
        BlockGroupFactory factory = Instantiate<BlockGroupFactory>(factoryPrefab);
        BlockGroup prism = factory.CreateRectangularPrism(48, 48, 48, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
