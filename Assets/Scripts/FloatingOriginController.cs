using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingOriginController : MonoBehaviour
{
    public Transform player;

    float threshold;

    public delegate void ToOrigin(float reference);
    public static event ToOrigin OnFloatingOrigin;

    bool originShift = false;
    float delay = 2f;
    float timer = 0f;

    void Awake()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Vector3 extents = boxCollider.size * 0.5f;
        threshold = extents.z;
    }

    void Update()
    {
        if (originShift)
        {
            timer += Time.deltaTime;

            if(timer >= delay)
            {
                originShift = false;
            }
        }
    }

    void LateUpdate()
    {
        float referencePosition = player.position.z;

        if (referencePosition >= threshold && !originShift)
        {
            if (OnFloatingOrigin != null)
            {
                OnFloatingOrigin(threshold);
            }
            player.position = new Vector3(player.position.x, player.position.y, player.position.z - threshold);
            timer = 0f;
            originShift = true;
        }
        else if(referencePosition <= -threshold && !originShift)
        {
            if (OnFloatingOrigin != null)
            {
                OnFloatingOrigin(-threshold);
            }
            player.position = new Vector3(player.position.x, player.position.y, player.position.z + threshold);
            timer = 0f;
            originShift = true;
        }
    }
}
