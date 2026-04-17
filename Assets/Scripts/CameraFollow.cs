using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // cible que la caméra doit suivre
    public Transform target;

    // décalage entre la caméra et le joueur
    public Vector3 offset = new Vector3(0f, 12f, -10f);

    // vitesse à laquelle la caméra suit
    public float followSpeed = 5f;

    void LateUpdate()
    {
        // si aucune cible n’est définie, on ne fait rien
        if (target == null) return;

        // position voulue de la caméra
        Vector3 desiredPosition = target.position + offset;

        // déplacement progressif de la caméra
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // la caméra regarde toujours vers le joueur
        transform.LookAt(target);
    }
}