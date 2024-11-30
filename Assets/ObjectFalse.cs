using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFalse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(False), 2f);

    }

    // Update is called once per frame
    void False()
    {
        gameObject.SetActive(false);
    }
}
