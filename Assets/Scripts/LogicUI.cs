using UnityEngine;
using TMPro;

// ce script sert à afficher l'état logique du niveau à l'écran
public class LogicUI : MonoBehaviour
{
    // texte du type de porte logique
    public TMP_Text gateText;

    // texte de l'état de A
    public TMP_Text aText;

    // texte de l'état de B
    public TMP_Text bText;

    // texte de l'état de la sortie
    public TMP_Text outputText;

    // cette fonction met à jour tout le panneau
    public void RefreshUI(GateType gateType, bool inputA, bool inputB, bool output)
    {
        // affichage du type de porte
        gateText.text = "Porte " + gateType;

        // affichage de A
        if (inputA)
        {
            aText.text = "1 (ON)";
            aText.color = Color.green;
        }
        else
        {
            aText.text = "0 (OFF)";
            aText.color = Color.red;
        }

        // affichage de B
        if (inputB)
        {
            bText.text = "1 (ON)";
            bText.color = Color.green;
        }
        else
        {
            bText.text = "0 (OFF)";
            bText.color = Color.red;
        }

        // affichage de la sortie
        if (output)
        {
            outputText.text = "1 OUVERT";
            outputText.color = Color.green;
        }
        else
        {
            outputText.text = "0 FERMÉ";
            outputText.color = Color.red;
        }
    }
}