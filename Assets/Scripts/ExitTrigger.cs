using UnityEngine;

// script pour valider la fin du niveau
public class ExitTrigger : MonoBehaviour
{
    // évite que le niveau se charge 20 fois d'un coup
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        // on vérifie que c'est bien le joueur
        if (!other.CompareTag("Player")) return;

        // évite les déclenchements répétés
        if (playerInside) return;

        playerInside = true;

        // on récupère le type de porte logique du niveau actuel
        GateType gateType = GameManager.Instance.GetCurrentGateType();

        // on calcule si la porte est ouverte
        bool result = GameManager.Instance.EvaluateGate(
            gateType,
            GameManager.Instance.inputA,
            GameManager.Instance.inputB
        );

        // si la porte est ouverte, on passe au niveau suivant
        if (result)
        {
            Debug.Log("Niveau réussi ✅");
            GameManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.Log("La sortie est encore bloquée 🔒");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // quand le joueur sort, on pourra redétecter plus tard
        if (!other.CompareTag("Player")) return;

        playerInside = false;
    }
}