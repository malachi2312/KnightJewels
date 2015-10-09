using UnityEngine;
using System.Collections;

public class Resolutioin : MonoBehaviour {


    float targetAspect = 10.0f / 16.0f;
    float windowAspect;
    float scaleHeight;
    float scaledWidth;
//    Camera camera;
    void Start()
    {

        Camera cam = GetComponent<Camera>();
        cam.aspect = targetAspect;

        //float TARGET_WIDTH = 800.0f;
        //float TARGET_HEIGHT = 1280.0f;
        //int PIXELS_TO_UNITS =64; // 1:1 ratio of pixels to units

        //float desiredRatio = TARGET_WIDTH / TARGET_HEIGHT;
        //float currentRatio = (float)Screen.width / (float)Screen.height;

        //if (currentRatio >= desiredRatio)
        //{
        //    // Our resolution has plenty of width, so we just need to use the height to determine the camera size
        //    Camera.main.orthographicSize = TARGET_HEIGHT / 4 / PIXELS_TO_UNITS;
        //}
        //else
        //{
        //    // Our camera needs to zoom out further than just fitting in the height of the image.
        //    // Determine how much bigger it needs to be, then apply that to our original algorithm.
        //    float differenceInSize = desiredRatio / currentRatio;
        //    Camera.main.orthographicSize = TARGET_HEIGHT / 4 / PIXELS_TO_UNITS * differenceInSize;
        //}

    }
}
