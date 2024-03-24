using UnityEngine;

public class LookToCamera : MonoBehaviour
{
    private Camera myCamera;

    private void Awake()
    {
        myCamera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(myCamera.transform.position);
        transform.Rotate(Vector3.up * 180);
    }

}
