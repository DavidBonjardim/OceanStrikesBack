using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    public GameObject myCanvasObject;
    public Canvas myCanvas;
    public GameObject cameraObject = null;
    public Camera myCamera;
    public GameObject buttons;


    public bool doOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        myCanvasObject.GetComponent<GameObject>();
        myCanvas = myCanvasObject.GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!doOnce)
        {
            cameraObject = GameObject.FindGameObjectWithTag("MainCamera");

            if(cameraObject != null)
            {
                myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                myCamera = cameraObject.GetComponent<Camera>();
                myCanvas.worldCamera = myCamera;

                buttons.transform.position = new Vector3(0, 0, 0);

                doOnce = true;
            }
        }
    }
}
