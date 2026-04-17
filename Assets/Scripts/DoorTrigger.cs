using UnityEngine;

// script pour tester si le joueur peut passer la porte
public class DoorTrigger : MonoBehaviour
{
    // évite de spammer la console tant que le joueur reste dans la zone
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        // on vérifie que c'est bien le joueur
        if (!other.CompareTag("Player")) return;

        // évite les déclenchements répétés
        if (playerInside) return;

        playerInside = true;

        // on récupère le type de porte du niveau actuel
        GateType gateType = GameManager.Instance.GetCurrentGateType();

        // on calcule si la porte doit être ouverte ou fermée
        bool result = GameManager.Instance.EvaluateGate(
            gateType,
            GameManager.Instance.inputA,
            GameManager.Instance.inputB
        );

        // message selon le résultat
        if (result)
        {
            Debug.Log("Porte ouverte ✅");
        }
        else
        {
            Debug.Log("Porte fermée 🔒");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // quand le joueur sort, on autorise un futur nouveau test
        if (!other.CompareTag("Player")) return;

        playerInside = false;
    }
}