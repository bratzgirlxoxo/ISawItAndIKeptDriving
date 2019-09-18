using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script controls the materials of the three blank cars at the end of the second scene
public class CrashManager : MonoBehaviour
{
    [SerializeField] private Material crashMat1, crashMat2, crashMat3;
    [SerializeField] private Texture2D[] crash1, crash2, crash3;
    private int idx1, idx2, idx3 = 0;

    [HideInInspector] public bool mat1crash, mat2crash, mat3crash;

    private int frameidx = 0;

    void Awake()
    {
        // set shader values
        crashMat1.SetTexture("_MainTex", null);
        crashMat2.SetTexture("_MainTex", null);
        crashMat3.SetTexture("_MainTex", null);
    }

    private int screenshotIdx = 0;
    void Update()
    {
        // every 3 frames update each clip
        if (frameidx % 3 == 0)
        {

            // if these materials have been activated, loop through all of their individual frames
            if (mat1crash)
            {
                crashMat1.SetTexture("_MainTex", crash1[idx1 % crash1.Length]);
                idx1++;
            }

            if (mat2crash)
            {
                crashMat2.SetTexture("_MainTex", crash2[idx2 % crash2.Length]);
                idx2++;
            }

            if (mat3crash)
            {
                crashMat3.SetTexture("_MainTex", crash3[idx3 % crash3.Length]);
                idx3++;
            }
        }
        frameidx++;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            screenshotIdx++;
            ScreenCapture.CaptureScreenshot("Screenshot" + screenshotIdx+".png");
        }
    }
}
