using UnityEngine;
using Playniax.Pyro;

public class CameraFlashTest : MonoBehaviour
{
    public void Flash()
    {
        CameraFlash.Flash(Camera.main, 3, 1, 1, 1);
    }
}
