using UnityEngine;
using System.Collections.Generic;

// ce script sert à générer un maze sous forme de tableau int[,]
// notes :
// 1 = mur
// 0 = chemin
// 2 = bouton A
// 3 = bouton B
// 4 = porte
// 5 = départ

public static class MazeGenerator
{
    // cette fonction génère un nouveau maze carré
    public static int[,] GenerateMaze(int size, GateType gateType)
    {
        // sécurité : on force une taille impaire
        // notes : un maze DFS marche mieux avec 9, 11, 13, 15...
        if (size % 2 == 0)
        {
            size += 1;
        }

        // on crée la grille
        int[,] maze = new int[size, size];

        // au début, tout est mur
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                maze[r, c] = 1;
            }
        }

        // point de départ du creusage
        // notes : on commence en 1,1 pour éviter la bordure
        CarveDFS(maze, 1, 1);

        // position de départ du joueur
        Vector2Int startPos = new Vector2Int(1, 1);

        // on cherche une case lointaine pour la porte
        Vector2Int doorPos = FindFarthestPathCell(maze, startPos);

        // on récupère toutes les cases chemin libres
        List<Vector2Int> freePathCells = GetAllFreePathCells(maze, startPos, doorPos);

        // on place le départ du joueur
        maze[startPos.x, startPos.y] = 5;

        // on place la porte
        maze[doorPos.x, doorPos.y] = 4;

        // s'il n'y a plus de cases libres, on s'arrête là
        if (freePathCells.Count == 0)
        {
            return maze;
        }

        // on choisit une case pour A
        Vector2Int aPos = ChooseFarCellFromStart(freePathCells, startPos);
        maze[aPos.x, aPos.y] = 2;
        freePathCells.Remove(aPos);

        // si le type logique a besoin de B, on place aussi B
        if (gateType == GateType.OR || gateType == GateType.AND)
        {
            // sécurité si une case libre reste
            if (freePathCells.Count > 0)
            {
                Vector2Int bPos;

                // pour AND, on essaie de placer B loin de A
                if (gateType == GateType.AND)
                {
                    bPos = ChooseFarCellFromReference(freePathCells, aPos);
                }
                else
                {
                    // pour OR, on peut juste prendre une autre case assez loin du départ
                    bPos = ChooseFarCellFromStart(freePathCells, startPos);
                }

                maze[bPos.x, bPos.y] = 3;
                freePathCells.Remove(bPos);
            }
        }

        return maze;
    }

    // cette fonction creuse le labyrinthe avec une version simple de DFS récursif
    private static void CarveDFS(int[,] maze, int row, int col)
    {
        // la case actuelle devient un chemin
        maze[row, col] = 0;

        // on prend les 4 directions possibles
        List<Vector2Int> directions = new List<Vector2Int>()
        {
            new Vector2Int(-2, 0), // haut
            new Vector2Int(2, 0),  // bas
            new Vector2Int(0, -2), // gauche
            new Vector2Int(0, 2)   // droite
        };

        // on mélange les directions pour rendre le maze aléatoire
        Shuffle(directions);

        // on teste chaque direction
        foreach (Vector2Int dir in directions)
        {
            int newRow = row + dir.x;
            int newCol = col + dir.y;

            // si la nouvelle case est bien dans la zone intérieure
            if (IsInsideBounds(maze, newRow, newCol))
            {
                // si la case n'a pas encore été creusée
                if (maze[newRow, newCol] == 1)
                {
                    // on ouvre le mur entre les deux cases
                    int wallRow = row + dir.x / 2;
                    int wallCol = col + dir.y / 2;
                    maze[wallRow, wallCol] = 0;

                    // on continue à creuser
                    CarveDFS(maze, newRow, newCol);
                }
            }
        }
    }

    // cette fonction vérifie qu'on reste bien dans la zone intérieure du maze
    private static bool IsInsideBounds(int[,] maze, int row, int col)
    {
        int rows = maze.GetLength(0);
        int cols = maze.GetLength(1);

        // notes : on évite la bordure extérieure
        return row > 0 && row < rows - 1 && col > 0 && col < cols - 1;
    }

    // cette fonction mélange une liste
    private static void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            Vector2Int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // cette fonction cherche la case chemin la plus loin du départ
    private static Vector2Int FindFarthestPathCell(int[,] maze, Vector2Int start)
    {
        int rows = maze.GetLength(0);
        int cols = maze.GetLength(1);

        bool[,] visited = new bool[rows, cols];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        Vector2Int farthest = start;

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1)
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            farthest = current;

            foreach (Vector2Int dir in directions)
            {
                int newRow = current.x + dir.x;
                int newCol = current.y + dir.y;

                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                {
                    if (!visited[newRow, newCol] && maze[newRow, newCol] == 0)
                    {
                        visited[newRow, newCol] = true;
                        queue.Enqueue(new Vector2Int(newRow, newCol));
                    }
                }
            }
        }

        return farthest;
    }

    // cette fonction récupère toutes les cases chemin libres sauf départ et porte
    private static List<Vector2Int> GetAllFreePathCells(int[,] maze, Vector2Int startPos, Vector2Int doorPos)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        int rows = maze.GetLength(0);
        int cols = maze.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (maze[r, c] == 0)
                {
                    Vector2Int current = new Vector2Int(r, c);

                    if (current != startPos && current != doorPos)
                    {
                        cells.Add(current);
                    }
                }
            }
        }

        return cells;
    }

    // cette fonction choisit une case assez loin du départ
    private static Vector2Int ChooseFarCellFromStart(List<Vector2Int> cells, Vector2Int startPos)
    {
        return ChooseFarCellFromReference(cells, startPos);
    }

    // cette fonction choisit la case la plus loin d'une case de référence
    private static Vector2Int ChooseFarCellFromReference(List<Vector2Int> cells, Vector2Int reference)
    {
        Vector2Int bestCell = cells[0];
        int bestDistance = -1;

        for (int i = 0; i < cells.Count; i++)
        {
            int distance = Mathf.Abs(cells[i].x - reference.x) + Mathf.Abs(cells[i].y - reference.y);

            if (distance > bestDistance)
            {
                bestDistance = distance;
                bestCell = cells[i];
            }
        }

        return bestCell;
    }
}