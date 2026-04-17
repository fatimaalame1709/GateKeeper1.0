using UnityEngine;

// petit menu pour les types de portes logiques
public enum GateType
{
    AND,
    OR,
    NOT
}

// cette classe sert juste à stocker les données d’un niveau
public class LogicLevelRuntime
{
    // type de porte logique du niveau
    public GateType gateType;

    // petit texte explicatif du niveau
    public string description;

    // tableau du labyrinthe
    // 0 = chemin
    // 1 = mur
    // 2 = bouton A
    // 3 = bouton B
    // 4 = porte
    // 5 = départ joueur
    public int[,] maze;

    // constructeur = quand on crée un niveau, on lui donne ses infos
    public LogicLevelRuntime(GateType gateType, string description, int[,] maze)
    {
        this.gateType = gateType;
        this.description = description;
        this.maze = maze;
    }
}

//À quoi sert ce script ?

//Il sert à stocker : 
//le type de porte logique (AND, OR, NOT)
//la description du niveau
//la grille du labyrinthe