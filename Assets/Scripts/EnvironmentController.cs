using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public List<Material> skyboxMaterials = new List<Material>();

    void Start()
    {
        if(skyboxMaterials.Count > 0)
        {
            RenderSettings.skybox = skyboxMaterials[Random.Range(0, skyboxMaterials.Count)];

            DynamicGI.UpdateEnvironment();
        }
    }
}
