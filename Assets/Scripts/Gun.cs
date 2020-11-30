using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int range;
    public int fireRate;
    public int bulletSize;
    public int power;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Vector3 orig, Vector3 dir)
    {
        Ray shootRay = new Ray(orig, dir);

        // find hit chunk groups in order of distance to gun
        List<RaycastHit> hits = new List<RaycastHit>(Physics.RaycastAll(shootRay, range));
        hits.Sort((x, y) => Vector3.Distance(x.point, orig).CompareTo(Vector3.Distance(y.point, orig)));

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.GetComponent<ChunkGroup>())
            {
                hit.transform.GetComponent<ChunkGroup>().RegisterHit(hit, shootRay, this);
            }
        }
    }
}
