using UnityEngine;

// ce script fait tourner doucement la skybox
public class SkyboxRotator : MonoBehaviour
{
    // vitesse de rotation du fond
    public float rotationSpeed = 1f;

    void Update()
    {
        if (RenderSettings.skybox != null)
        {
            float currentRotation = RenderSettings.skybox.GetFloat("_Rotation");
            currentRotation += rotationSpeed * Time.deltaTime;
            RenderSettings.skybox.SetFloat("_Rotation", currentRotation);
        }
    }
}