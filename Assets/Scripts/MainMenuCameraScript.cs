using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraScript : MonoBehaviour
{
    
    void Update()
    {
        this.gameObject.transform.localEulerAngles += 10 * Time.deltaTime *  Vector3.up;
    }
}
