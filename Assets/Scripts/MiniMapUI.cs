using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// ce script dessine la mini-map à partir du tableau du maze
public class MiniMapUI : MonoBehaviour
{
    // parent qui contient toutes les petites cases
    public RectTransform miniMapGrid;

    // prefab du petit carré UI
    public GameObject miniMapCellPrefab;

    // taille d'une case sur la mini-map
    public float cellSize = 12f;

    // marqueur du joueur sur la mini-map
    public RectTransform playerMarker;

    // taille d'une case du vrai maze dans le monde
    public float worldCellSize = 2f;

    // liste des cases créées pour pouvoir les supprimer quand on recharge
    private List<GameObject> spawnedCells = new List<GameObject>();

    // cette fonction construit la mini-map complète
    public void BuildMiniMap(int[,] maze)
    {
        // sécurité
        if (miniMapGrid == null || miniMapCellPrefab == null || maze == null)
        {
            return;
        }

        // on efface l'ancienne mini-map
        ClearMiniMap();

        int rows = maze.GetLength(0);
        int cols = maze.GetLength(1);

        // on ajuste la taille du conteneur
        miniMapGrid.sizeDelta = new Vector2(cols * cellSize, rows * cellSize);

        // on crée les cases
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject cell = Instantiate(miniMapCellPrefab, miniMapGrid);
                spawnedCells.Add(cell);

                RectTransform rect = cell.GetComponent<RectTransform>();
                Image image = cell.GetComponent<Image>();

                if (rect != null)
                {
                    rect.sizeDelta = new Vector2(cellSize, cellSize);

                    // position dans la grille
                    rect.anchorMin = new Vector2(0, 1);
                    rect.anchorMax = new Vector2(0, 1);
                    rect.pivot = new Vector2(0, 1);

                    rect.anchoredPosition = new Vector2(
                        c * cellSize,
                        -(rows - 1 - r) * cellSize
                    );
                }

                if (image != null)
                {
                    image.color = GetColorForCell(maze[r, c]);
                }
            }
        }

        // on garde le marqueur joueur au-dessus des cases générées
        if (playerMarker != null)
        {
            playerMarker.SetParent(miniMapGrid, false);
            playerMarker.SetAsLastSibling();
        }
    }

    // cette fonction déplace le marqueur du joueur sur la mini-map
    public void UpdatePlayerMarker(Vector3 playerWorldPosition, int rows)
    {
        if (playerMarker == null) return;

        // convertir la position monde en position de grille plus fluide
        float col = playerWorldPosition.x / worldCellSize;
        float row = playerWorldPosition.z / worldCellSize;

        // placer le marqueur dans l'UI
        playerMarker.anchorMin = new Vector2(0, 1);
        playerMarker.anchorMax = new Vector2(0, 1);
        playerMarker.pivot = new Vector2(0, 1);
        playerMarker.sizeDelta = new Vector2(cellSize, cellSize);
        playerMarker.SetAsLastSibling();

        playerMarker.anchoredPosition = new Vector2(
            col * cellSize,
            -(rows - 1 - row) * cellSize
        );
    }

    // cette fonction supprime toutes les anciennes cases
    private void ClearMiniMap()
    {
        for (int i = 0; i < spawnedCells.Count; i++)
        {
            if (spawnedCells[i] != null)
            {
                Destroy(spawnedCells[i]);
            }
        }

        spawnedCells.Clear();
    }

    // cette fonction choisit la couleur selon le type de case
    private Color GetColorForCell(int cellValue)
    {
        // 1 = mur
        if (cellValue == 1)
        {
            return new Color(0.25f, 0.28f, 0.33f, 1f);
        }

        // 0 = chemin
        if (cellValue == 0)
        {
            return new Color(0.55f, 0.58f, 0.62f, 1f);
        }

        // 2 = bouton A
        if (cellValue == 2)
        {
            return new Color(1f, 0.6f, 0.1f, 1f);
        }

        // 3 = bouton B
        if (cellValue == 3)
        {
            return new Color(1f, 0.6f, 0.1f, 1f);
        }

        // 4 = porte
        if (cellValue == 4)
        {
            return new Color(1f, 0.2f, 0.2f, 1f);
        }

        // 5 = départ
        if (cellValue == 5)
        {
            return new Color(0.55f, 0.58f, 0.62f, 1f);
        }

        // sécurité si la valeur n'est pas reconnue
        return Color.white;
    }
}