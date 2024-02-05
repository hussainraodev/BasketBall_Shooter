using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float time=2;
    // Start is called before the first frame update
    private void OnEnable()
    {
        Destroy(this.gameObject, time);
    }
}
