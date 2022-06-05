using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotScript : MonoBehaviour
{
    public Transform spotScriptTransform;
    public int valor;

    public void Start()
    {
        spotScriptTransform = GetComponent<Transform>();
    }
}
