using UnityEngine;

public class BillboardManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    
    void Start()
    {
        
    }

  
    void LateUpdate()
    {
        transform.rotation = mainCamera.transform.rotation;

        Vector3 newRotation = mainCamera.transform.eulerAngles;

        newRotation.x = 0;
        newRotation.z = 0;

        transform.eulerAngles = newRotation;
    }
}
