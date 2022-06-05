using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blip : MonoBehaviour
{
    // A Material with the Unity shader you want to process the image with
    public Material mat;

   // public GameManager gameManager;
   // bool doOnce = false;
   // bool doTime = false;

   // float time = 0;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Read pixels from the source RenderTexture, apply the material, copy the updated results to the destination RenderTexture
        Graphics.Blit(src, dest, mat);
    }

    /* private void Update()
    {
        if(gameManager.secondPhase == true && doOnce == false)
        {
            mat.SetFloat("SecondPhaseValue", 2);

            doOnce = true;
        }

        if(doOnce == true && time < 60)
        {
            time += Time.deltaTime;
        }
        else if(time >= 58 && !doTime)
        {
            mat.SetFloat("SecondPhaseValue", 1);

            doTime = true;
        }

    } */

}
