using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // vitesse de déplacement du joueur
    public float moveSpeed = 5f;

    // petite gravité pour que le joueur reste bien au sol
    public float gravity = -9.81f;

    // composant Unity qui gère le déplacement du personnage
    private CharacterController controller;

    // vitesse verticale (surtout utile pour la gravité)
    private Vector3 velocity;

    void Start()
    {
        // on récupère le Character Controller attaché au joueur
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // on lit les touches de déplacement
        // Horizontal = gauche/droite
        // Vertical = avant/arrière
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // on crée le vecteur de déplacement
        Vector3 move = new Vector3(moveX, 0f, moveZ);

        // on déplace le joueur horizontalement
        controller.Move(move * moveSpeed * Time.deltaTime);

        // si le joueur touche le sol et tombe encore, on remet une petite valeur
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // on applique la gravité
        velocity.y += gravity * Time.deltaTime;

        // on déplace le joueur verticalement
        controller.Move(velocity * Time.deltaTime);
    }
}