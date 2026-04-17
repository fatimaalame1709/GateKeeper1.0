# GateKeeper 1.0

## C'est quoi ?

On doit faire un jeu pour le cours de STM. Et donc GateKeeper est un jeu créé sur Unity en 3D basé sur des portes logiques (love PFO).
Le but est de se déplacer dans un labyrinthe, activer les bons boutons logiques, puis ouvrir la sortie.

Pour l'instant, le jeu utilise surtout :
- OR
- AND
- NOT

Le projet a été fait avec des scripts en C#

## Idée du jeu

Le joueur apparaît dans un labyrinthe.

Dans le niveau, il peut trouver :
- un bouton A
- un bouton B
- une porte de sortie

Selon le type de porte logique du niveau, il faut activer les bons boutons pour ouvrir la porte.

Exemples :
- OR = A ou B
- AND = A et B
- NOT = la porte s'ouvre quand A est désactivé

## Ce qu'on a déjà fait

### Base du jeu
- création du labyrinthe
- déplacement du joueur
- caméra qui suit
- porte qui change d'état

### Logique
- niveau OR
- niveau AND
- niveau NOT

### Objets
- bouton A
- bouton B
- porte générée automatiquement
- bouton OpenSCAD importé dans Unity

### Interface
- panneau UI en bas à gauche
- affichage de :
  - la porte logique actuelle
  - l'état de A
  - l'état de B
  - l'état de la sortie

### Progression
- passage automatique au niveau suivant
- base d'un système de difficulté adaptative


## Adaptativité

L'idée finale du projet, c'est de rendre le jeu adaptatif. Le jeu doit observer la performance du joueur, puis ajuster la difficulté.

Pour l'instant, la base utilisée est surtout :
- le temps mis pour finir un niveau

Ensuite, plus tard, la difficulté pourra modifier :
- la taille du maze
- le type de logique
- la disposition des boutons et de la porte

## Structure actuelle du projet

### Scripts principaux
- `GameManager.cs` : gère le jeu, les niveaux, la logique, l'UI et la difficulté
- `LogicUI.cs` : met à jour  panneau logique à l'écran
- `LogicTrigger.cs` : gère boutons A et B
- `DoorTrigger.cs` : gère sortie / validation du niveau
- `PlayerMovement.cs` : déplacement du joueur
- `CameraFollow.cs` : caméra qui suit le joueur
- `LogicLevelRuntime.cs` : structure des niveaux

## Outils utilisés
- Unity 6
- C#
- OpenSCAD

## État actuel
Le jeu fonctionne déjà avec :
- plusieurs niveaux logiques
- une porte qui s'ouvre et se ferme
- une UI logique
- une progression entre niveaux
- une base pour l'adaptativité hihi

La prochaine étape est de créer un maze generator simple + brancher ce générateur sur la difficulté adaptative

## Remarques
Le projet est encore en évolution. Donc calm down sur les critiques 
