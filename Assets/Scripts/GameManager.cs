using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    // singleton simple pour accéder au GameManager depuis les autres scripts
    public static GameManager Instance;

    [Header("Prefabs")]
    // prefab du sol
    public GameObject floorPrefab;

    // prefab du mur
    public GameObject wallPrefab;

    // prefab du joueur
    public GameObject playerPrefab;

    // prefab du bouton A
    public GameObject buttonAPrefab;

    // prefab du bouton B
    public GameObject buttonBPrefab;

    // prefab de la porte
    public GameObject doorPrefab;

    [Header("Scene References")]
    // parent qui contient les objets du niveau
    public Transform levelParent;

    // trigger de sortie déjà placé dans la scène
    public Transform exitTriggerTransform;

    [Header("Settings")]
    // taille d'une case du labyrinthe
    public float cellSize = 2f;


    [Header("UI")]
    // script qui gère l'affichage logique à l'écran
    public LogicUI logicUI;

    // script qui gère les vies et le niveau en haut à gauche
    public StatusUI statusUI;

    // script qui gère la mini-map
    public MiniMapUI miniMapUI;

    // script qui gère le timer affiché
    public Timer timerUI;

    // texte d'instruction affiché en haut au centre
    public TMP_Text levelInstructionText;

    // panel du haut qui contient les instructions
    public GameObject instructionsPanel;

    public GameObject tutorialMessagePanel;
    public TextMeshProUGUI tutorialMessageText;

    public GameObject tutorialCompletePanel;
    public int tutorialLevelCount = 4;


    [Header("Logic State")]
    
    // état du bouton A
    public bool inputA = false;

    // nombre de vies du joueur
    public int lives = 3;

    // score du joueur
    public int score = 0;
    
    // état du bouton B
    public bool inputB = false;

    // référence vers le joueur créé dans la scène
    private GameObject currentPlayer;

    // référence vers la porte créée dans la scène
    private GameObject currentDoor;

    // maze actuellement utilisé
    private int[,] currentMaze;    

    // liste des niveaux
    public List<LogicLevelRuntime> levels = new List<LogicLevelRuntime>();

    // index du niveau actuel
    private int currentLevelIndex = 0;

    // dit si on a dépassé le dernier niveau
    private bool gameFinished = false;


    [Header("Adaptive Difficulty")]
    // niveau de difficulté actuel
    public int difficultyLevel = 1;

    // nombre de réussites rapides d'affilée
    private int consecutiveFastSuccesses = 0;

    // temps de départ du niveau actuel
    private float levelStartTime = 0f;

    // temps mis pour finir le dernier niveau
    public float lastCompletionTime = 0f;

    private void Awake()
    {
        // singleton basique
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // on crée les niveaux
        CreateLevels();

        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
        }

        if (tutorialMessagePanel != null)
        {
            tutorialMessagePanel.SetActive(true);
        }

        if (tutorialCompletePanel != null)
        {
            tutorialCompletePanel.SetActive(false);
        }

        // on charge le premier niveau
        LoadLevel(0);
    }
    private void Update()
    {
        UpdateMiniMapPlayer();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R détecté");
            SceneManager.LoadScene("MainMenu");
        }
    }

    void CreateLevels()
    {
        // niveau 1 : porte OR
        levels.Add(new LogicLevelRuntime(
            GateType.OR,
            "Porte OR : Active A OU B pour ouvrir la porte !",
            new int[,]
            {
                {1,1,1,1,1,1,1,1,1},
                {1,5,0,0,1,0,0,0,1},
                {1,0,1,0,1,0,1,0,1},
                {1,0,1,0,0,0,1,0,1},
                {1,0,1,1,1,1,1,0,1},
                {1,0,0,0,2,0,0,0,1},
                {1,1,1,0,1,0,1,1,1},
                {1,0,0,0,0,0,0,3,1},
                {1,0,1,1,4,1,1,0,1},
                {1,1,1,1,1,1,1,1,1}
            }
        ));

        // niveau 2 : porte AND
        levels.Add(new LogicLevelRuntime(
            GateType.AND,
            "Porte AND : Active A ET B pour ouvrir la porte !",
            new int[,]
            {
                {1,1,1,1,1,1,1,1,1},
                {1,5,0,1,0,0,0,0,1},
                {1,0,0,1,0,1,1,0,1},
                {1,0,1,1,0,0,1,0,1},
                {1,0,0,0,0,1,1,0,1},
                {1,1,1,0,1,1,0,0,1},
                {1,2,0,0,0,0,0,1,1},
                {1,1,1,0,0,1,0,3,1},
                {1,0,0,0,4,0,0,1,1},
                {1,1,1,1,1,1,1,1,1}
            }
        ));

        // niveau 3 : porte NOT
        levels.Add(new LogicLevelRuntime(
            GateType.NOT,
            "Porte NOT : La sortie s'ouvre quand A est désactivé !",
            new int[,]
            {
                {1,1,1,1,1,1,1,1,1},
                {1,5,0,0,0,1,0,0,1},
                {1,1,1,1,0,1,0,1,1},
                {1,0,0,0,0,0,0,0,1},
                {1,0,1,1,1,1,1,0,1},
                {1,0,0,2,1,0,0,0,1},
                {1,1,1,0,1,0,1,1,1},
                {1,0,0,0,0,0,0,0,1},
                {1,0,1,1,4,1,1,0,1},
                {1,1,1,1,1,1,1,1,1}
            }
        ));

        // niveau 4 : porte XOR
        levels.Add(new LogicLevelRuntime(
            GateType.XOR,
            "Porte XOR : La porte s'ouvre quand UNE SEULE entrée est activée !",
            new int[,]
            {
                {1,1,1,1,1,1,1,1,1},
                {1,5,0,0,1,0,0,0,1},
                {1,0,1,0,1,0,1,0,1},
                {1,0,1,0,0,0,1,0,1},
                {1,0,1,1,1,1,1,0,1},
                {1,0,0,0,2,0,0,0,1},
                {1,1,1,0,1,0,1,1,1},
                {1,0,0,0,0,0,0,3,1},
                {1,0,1,1,4,1,1,0,1},
                {1,1,1,1,1,1,1,1,1}
            }
        ));
    }

    public void LoadLevel(int index)
    {
        // on garde en mémoire quel niveau est chargé
        currentLevelIndex = index;

        // comme on charge un vrai niveau, le jeu n'est pas fini
        gameFinished = false;

        // on démarre le chrono du niveau
        levelStartTime = Time.time;

        // on remet aussi le timer visuel à zéro
        if (timerUI == null)
        {
            timerUI = FindFirstObjectByType<Timer>();
        }

        if (timerUI != null)
        {
            timerUI.ResetTimer();
        }

        // on supprime les anciens objets du niveau
        foreach (Transform child in levelParent)
        {
            Destroy(child.gameObject);
        }

        // on remet les états logiques à zéro
        inputA = false;
        inputB = false;

        // pendant un niveau normal, le bouton A n'est pas activé
        // exception pour NOT : on le met activé au départ pour forcer le joueur à le désactiver
        if (levels[index].gateType == GateType.NOT)
        {
            inputA = true;
        }

        // on vide l'ancienne référence de porte
        currentDoor = null;

        // on récupère le niveau pour garder sa logique (OR / AND / NOT)
        LogicLevelRuntime level = levels[index];

        // on génère un nouveau maze selon la difficulté actuelle
        int[,] generatedMaze = MazeGenerator.GenerateMaze(
            GetMazeSizeForCurrentDifficulty(),
            level.gateType
        );

        // on garde ce maze en mémoire pour la mini-map
        currentMaze = generatedMaze;

        Debug.Log("Maze généré : " + generatedMaze.GetLength(0) + " x " + generatedMaze.GetLength(1));
        
        // debug pour voir la taille cible du maze selon la difficulté actuelle
        Debug.Log("Difficulté actuelle : " + difficultyLevel);
        Debug.Log("Taille cible du maze : " + GetMazeSizeForCurrentDifficulty());

        int rows = generatedMaze.GetLength(0);
        int cols = generatedMaze.GetLength(1);

        // on parcourt toutes les cases du tableau
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int cell = generatedMaze[r, c];
                Vector3 pos = new Vector3(c * cellSize, 0, r * cellSize);

                // on met du sol partout
                Instantiate(floorPrefab, pos, Quaternion.identity, levelParent);

                // si la case vaut 1, on place un mur
                if (cell == 1)
                {
                    Instantiate(wallPrefab, pos + new Vector3(0, 0.9f, 0), Quaternion.identity, levelParent);
                }

                // si la case vaut 2, on place le bouton A
                if (cell == 2)
                {
                    Instantiate(buttonAPrefab, pos, Quaternion.identity, levelParent);
                }

                // si la case vaut 3, on place le bouton B
                if (cell == 3)
                {
                    Instantiate(buttonBPrefab, pos, Quaternion.identity, levelParent);
                }

                // si la case vaut 4, on place la porte
                if (cell == 4)
                {
                    Vector3 doorWorldPos = pos + new Vector3(0, 1f, 0);

                    currentDoor = Instantiate(
                        doorPrefab,
                        doorWorldPos,
                        Quaternion.identity,
                        levelParent
                    );

                    // on déplace aussi le trigger de sortie sur la porte générée
                    if (exitTriggerTransform != null)
                    {
                        exitTriggerTransform.position = doorWorldPos;
                    }
                }

                // si la case vaut 5, on place le joueur au départ
                if (cell == 5)
                {
                    if (currentPlayer != null)
                        Destroy(currentPlayer);

                    currentPlayer = Instantiate(
                        playerPrefab,
                        new Vector3(c * cellSize, 1f, r * cellSize),
                        Quaternion.identity
                    );

                    // la caméra suit le joueur
                    CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
                    if (cam != null)
                    {
                        cam.target = currentPlayer.transform;
                    }
                }
            }
        }

        // une fois la porte créée, on met son état visuel à jour
        UpdateDoorState();

        // on met aussi l'interface logique à jour
        UpdateLogicUI();

        // on met à jour les vies et le niveau affichés
        UpdateStatusUI();
        
        // on met à jour la phrase d'instruction du niveau
        UpdateLevelInstructionUI();

        // on remet le panel d'instructions visible pendant les niveaux
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
        }

        // on remet aussi le panel de tuto visible pendant les niveaux tuto
        if (tutorialMessagePanel != null)
        {
            tutorialMessagePanel.SetActive(true);
        }

        // le panel de fin de tuto doit rester caché pendant un niveau
        if (tutorialCompletePanel != null)
        {
            tutorialCompletePanel.SetActive(false);
        }

        // on met à jour le message du tuto selon le niveau actuel
        UpdateTutorialMessage();

        // on construit la mini-map avec le maze généré
        if (miniMapUI != null)
        {
            miniMapUI.BuildMiniMap(generatedMaze);
        }


    }

    // cette fonction inverse l'état de A
    public void ToggleA()
    {
        inputA = !inputA;
        Debug.Log("Etat de A : " + inputA);

        // à chaque changement, on met à jour la porte
        UpdateDoorState();

        // on met aussi l'interface à jour
        UpdateLogicUI();
    }

    // cette fonction inverse l'état de B
    public void ToggleB()
    {
        inputB = !inputB;
        Debug.Log("Etat de B : " + inputB);

        // à chaque changement, on met à jour la porte
        UpdateDoorState();

        // on met aussi l'interface à jour
        UpdateLogicUI();
    }

    // cette fonction calcule le résultat de la porte logique du niveau
    public bool EvaluateGate(GateType gate, bool a, bool b)
    {
        // si la porte est AND, il faut A et B
        if (gate == GateType.AND)
        {
            return a && b;
        }

        // si la porte est OR, il faut A ou B
        if (gate == GateType.OR)
        {
            return a || b;
        }

        // si la porte est NOT, on inverse A
        if (gate == GateType.NOT)
        {
            return !a;
        }

        // si la porte est XOR, une seule entrée doit être active
        if (gate == GateType.XOR)
        {
            return a != b;
        }

        // sécurité au cas où
        return false;
    }

    // renvoie le type de porte logique du niveau actuel
    public GateType GetCurrentGateType()
    {
        // sécurité si l'index sort de la liste
        if (currentLevelIndex < 0 || currentLevelIndex >= levels.Count)
        {
            return GateType.OR;
        }

        return levels[currentLevelIndex].gateType;
    }

    // cette fonction met à jour l'interface logique
    public void UpdateLogicUI()
    {
        // si on a dépassé le dernier niveau, on arrête ici
        if (gameFinished) return;

        // sécurité si aucune UI n'est reliée
        if (logicUI == null) return;

        // on récupère le type de porte logique du niveau actuel
        GateType gateType = GetCurrentGateType();

        // on calcule la sortie logique
        bool output = EvaluateGate(gateType, inputA, inputB);

        // on met à jour le panneau
        logicUI.RefreshUI(gateType, inputA, inputB, output);
    }
    
    // cette fonction met à jour l'affichage des vies, du score, du niveau et de la difficulté
    public void UpdateStatusUI()
    {
        // sécurité si aucune UI n'est reliée
        if (statusUI == null) return;

        // +1 parce que l'index commence à 0 mais le joueur doit voir 1, 2, 3...
        statusUI.RefreshStatus(lives, score, currentLevelIndex + 1, difficultyLevel);
    }

    // cette fonction met à jour la phrase d'instruction du niveau
    public void UpdateLevelInstructionUI()
    {
        if (levelInstructionText == null) return;

        switch (currentLevelIndex)
        {
            case 0:
                levelInstructionText.text = "Porte OR : Active A OU B pour ouvrir la porte !";
                break;

            case 1:
                levelInstructionText.text = "Niveau réussi ! Porte AND : Active A ET B pour ouvrir la porte !";
                break;

            case 2:
                levelInstructionText.text = "Niveau réussi ! Porte NOT : La sortie s'ouvre quand A est DÉSACTIVÉ !";
                break;

            case 3:
                levelInstructionText.text = "Niveau réussi ! Porte XOR : La porte s'ouvre quand UNE SEULE entrée est activée !";
                break;

            default:
                levelInstructionText.text = "";
                break;
        }
    }

    // cette fonction met à jour le message du tuto selon le niveau actuel
    public void UpdateTutorialMessage()
    {
        if (tutorialMessageText == null) return;

        switch (currentLevelIndex)
        {
            case 0:
                tutorialMessageText.text = "Porte OR : Active A OU B pour ouvrir la porte !";
                break;

            case 1:
                tutorialMessageText.text = "Porte AND : Active A ET B pour ouvrir la porte !";
                break;

            case 2:
                tutorialMessageText.text = "Porte NOT : La sortie s'ouvre quand A est DÉSACTIVÉ !";
                break;

            case 3:
                tutorialMessageText.text = "Porte XOR : La porte s'ouvre quand UNE SEULE entrée est activée !";
                break;

            default:
                tutorialMessageText.text = "";
                break;
        }
    }

    // cette fonction affiche le panel de fin de tuto
    public void ShowTutorialCompletePanel()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }

        if (tutorialMessagePanel != null)
        {
            tutorialMessagePanel.SetActive(false);
        }

        if (tutorialCompletePanel != null)
        {
            tutorialCompletePanel.SetActive(true);
        }
    }

    // cette fonction met à jour le marqueur joueur sur la mini-map
    public void UpdateMiniMapPlayer()
    {
        if (miniMapUI == null) return;
        if (currentPlayer == null) return;
        if (currentMaze == null) return;

        miniMapUI.UpdatePlayerMarker(currentPlayer.transform.position, currentMaze.GetLength(0));
    }
    
    // cette fonction met à jour l'état visuel de la porte
    public void UpdateDoorState()
    {
        // si on a dépassé le dernier niveau, on arrête ici
        if (gameFinished) return;

        // sécurité : si aucune porte n'est créée, on arrête
        if (currentDoor == null) return;

        // on récupère le type de porte logique du niveau actuel
        GateType gateType = GetCurrentGateType();

        // on calcule si la porte doit être ouverte ou fermée
        bool result = EvaluateGate(gateType, inputA, inputB);

        // on récupère le renderer de la porte
        Renderer doorRenderer = currentDoor.GetComponent<Renderer>();

        // on récupère aussi le collider de la porte
        Collider doorCollider = currentDoor.GetComponent<Collider>();

        // sécurité si jamais il manque un composant
        if (doorRenderer == null) return;

        // si la logique est vraie, la porte est ouverte
        if (result)
        {
            // vert translucide
            doorRenderer.material.color = new Color(0f, 166f / 255f, 0f, 0.6f);

            // la porte ouverte ne bloque plus le joueur
            if (doorCollider != null)
            {
                doorCollider.enabled = false;
            }

            Debug.Log("Porte ouverte ✅");
        }
        else
        {
            // rouge translucide
            doorRenderer.material.color = new Color(0.55f, 0.05f, 0.05f, 0.6f);

            // la porte fermée rebloque le passage
            if (doorCollider != null)
            {
                doorCollider.enabled = true;
            }

            Debug.Log("Porte fermée 🔒");
        }
    }
    
    // cette fonction donne le temps cible selon la difficulté
    public float GetTargetTimeForCurrentDifficulty()
    {
        if (difficultyLevel == 1)
        {
            return 35f;
        }

        if (difficultyLevel == 2)
        {
            return 50f;
        }

        if (difficultyLevel == 3)
        {
            return 70f;
        }

        // difficulté 4 ou plus
        return 90f;
    }
    
        // cette fonction donne la taille du maze selon la difficulté
    public int GetMazeSizeForCurrentDifficulty()
    {
        if (difficultyLevel == 1)
        {
            return 9;
        }

        if (difficultyLevel == 2)
        {
            return 11;
        }

        if (difficultyLevel == 3)
        {
            return 13;
        }

        // difficulté 4 ou plus
        return 15;
    }


        // cette fonction ajuste la difficulté selon le temps du joueur
    public void AdjustDifficulty()
    {
        // temps mis pour finir le niveau
        lastCompletionTime = Time.time - levelStartTime;

        // temps cible pour la difficulté actuelle
        float targetTime = GetTargetTimeForCurrentDifficulty();

        // si le joueur a été rapide
        if (lastCompletionTime <= targetTime)
        {
            consecutiveFastSuccesses += 1;

            Debug.Log("Niveau fini rapidement");
            Debug.Log("Temps : " + lastCompletionTime + " / cible : " + targetTime);
        }
        else
        {
            // si le joueur a été lent, on casse la série de réussites rapides
            consecutiveFastSuccesses = 0;

            Debug.Log("Niveau réussi, mais trop lent");
            Debug.Log("Temps : " + lastCompletionTime + " / cible : " + targetTime);
        }

        // si le joueur réussit 2 niveaux rapides d'affilée, on monte
        if (consecutiveFastSuccesses >= 2)
        {
            difficultyLevel = Mathf.Min(difficultyLevel + 1, 4);
            consecutiveFastSuccesses = 0;

            Debug.Log("Difficulté augmentée ⬆");
            Debug.Log("Nouvelle difficulté : " + difficultyLevel);
        }

        // si le joueur est vraiment trop lent, on baisse
        if (lastCompletionTime > targetTime * 1.5f)
        {
            difficultyLevel = Mathf.Max(difficultyLevel - 1, 1);
            consecutiveFastSuccesses = 0;

            Debug.Log("Difficulté baissée ⬇");
            Debug.Log("Nouvelle difficulté : " + difficultyLevel);
        }
    }

    // cette fonction charge le niveau suivant
    public void LoadNextLevel()
    {
        // avant de changer de niveau, on ajuste la difficulté
        AdjustDifficulty();

        // le joueur gagne des points quand il termine un niveau
        score += 20;

        // on met à jour le HUD tout de suite
        UpdateStatusUI();

        // on passe au niveau suivant
        currentLevelIndex++;

        // si on a fini les niveaux du tuto, on affiche le panel final
        if (currentLevelIndex >= tutorialLevelCount)
        {
            gameFinished = true;
            ShowTutorialCompletePanel();
            Debug.Log("Tutoriel terminé !");
            return;
        }

        // sécurité si jamais l'index dépasse la liste
        if (currentLevelIndex >= levels.Count)
        {
            gameFinished = true;
            Debug.Log("Tous les niveaux sont terminés hihi");
            return;
        }

        // sinon on charge le niveau suivant
        LoadLevel(currentLevelIndex);
    }
}