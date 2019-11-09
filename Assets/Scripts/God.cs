using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    public BlockGroupFactory factoryPrefab;
    public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        BlockGroupFactory factory = Instantiate<BlockGroupFactory>(factoryPrefab);
        BlockGroup prism = factory.CreateRectangularPrism(4096, 16, 4096, player.transform.position, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
