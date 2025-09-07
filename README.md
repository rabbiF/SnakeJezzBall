# SnakeJezzBall

🐍 Un jeu innovant qui combine les mécaniques classiques de Snake avec les éléments de conquête de territoire de JezzBall !

## 📋 Table des matières

- [Description](#description)
- [Fonctionnalités](#fonctionnalités)
- [Prérequis](#prérequis)
- [Installation](#installation)
- [Comment jouer](#comment-jouer)
- [Architecture](#architecture)
- [Technologies utilisées](#technologies-utilisées)
- [Contribution](#contribution)
- [Licence](#licence)

## 🎮 Description

SnakeJezzBall est un jeu d'arcade qui réinvente les classiques Snake et JezzBall en combinant :
- Le mouvement et la croissance du serpent traditionnel
- La construction de murs pour créer des territoires
- Un système de conquête de zones pour gagner

L'objectif est de capturer **75% du territoire** en construisant des murs stratégiques tout en évitant les collisions et en collectant des pommes spéciales.

## ✨ Fonctionnalités

### Gameplay Core
- **Serpent évolutif** : Grandit en mangeant des pommes
- **Construction de territoires** : Mode spécial pour créer des murs
- **Système de conquête** : Fermez des zones pour les capturer
- **Objectif progressif** : Atteignez 75% de territoire conquis

### Types de pommes
- 🔴 **Normale** : +1 segment, 10 points
- 🟡 **Dorée** : +2 segments, 50 points + invincibilité temporaire
- 🔵 **Réduction** : Pas de croissance, 5 points

### États du serpent
- **Normal** : Mouvement classique
- **Construction** : Mode de création de murs
- **Étourdi** : Temporairement immobilisé
- **Invincible** : Immunité temporaire aux collisions

### Interface utilisateur
- Score en temps réel
- Barre de progression du territoire
- Indicateurs d'état du serpent
- Instructions contextuelles

## 🔧 Prérequis

- **.NET 8.0** ou supérieur
- **Windows, macOS, ou Linux**
- **Raylib-cs 7.0.1** (automatiquement installé via NuGet)

## 📥 Installation

### Méthode 1 : Clonage et compilation

```bash
# Cloner le repository
git clone [URL_DU_REPOSITORY]
cd SnakeJezzBall

# Restaurer les dépendances
dotnet restore

# Compiler et exécuter
dotnet run --project SnakeJezzBall
```

### Méthode 2 : Visual Studio

1. Ouvrir `SnakeJezzBall.sln` dans Visual Studio
2. Appuyer sur F5 pour compiler et exécuter

## 🎯 Comment jouer

### Contrôles de base
- **Z/W** : Aller vers le haut
- **S** : Aller vers le bas  
- **Q/A** : Aller vers la gauche
- **D** : Aller vers la droite
- **ESPACE** : Entrer/Sortir du mode construction
- **R** : Redémarrer la partie
- **N** : Niveau suivant (quand terminé)

### Objectifs
1. **Collectez des pommes** pour grandir et gagner des points
2. **Utilisez le mode construction** (ESPACE) pour créer des murs
3. **Fermez des zones** en construisant des murs stratégiques
4. **Atteignez 75%** de territoire conquis pour gagner
5. **Évitez** les collisions avec les murs et vous-même

### Stratégies
- Planifiez vos murs pour fermer de grandes zones
- Utilisez les pommes dorées pour l'invincibilité temporaire
- Créez des couloirs sûrs avant de vous agrandir
- Surveillez votre pourcentage de territoire

## 🏗️ Architecture

Le projet suit une architecture modulaire claire :

### Structure des dossiers
```
SnakeJezzBall/
├── GameObjects/          # Entités du jeu
│   ├── Snake.cs         # Logique du serpent
│   ├── Apple.cs         # Système des pommes
│   ├── Wall.cs          # Gestion des murs
│   └── Territory.cs     # Territoires conquis
├── Scenes/              # Gestion des écrans
│   ├── MenuScene.cs     # Menu principal
│   ├── GameScene.cs     # Jeu principal
│   └── GameOverScene.cs # Écran de fin
├── Services/            # Services centralisés
│   ├── GridManager.cs   # Gestion de la grille
│   └── TerritoryManager.cs # Logique des territoires
└── Utils/              # Utilitaires
    ├── Coordinates.cs   # Système de coordonnées
    └── Enums.cs        # Énumérations du jeu
```

### Patterns utilisés
- **Service Locator** : Injection de dépendances simple
- **State Machine** : États du serpent et du jeu
- **Observer Pattern** : Événements de territoire
- **Component Pattern** : Objets de jeu modulaires

### Services principaux
- **IGridManager** : Gestion de la grille et collisions
- **ITerritoryManager** : Calcul et gestion des territoires
- **CollisionManager** : Détection avancée des collisions

## 🛠️ Technologies utilisées

- **Langage** : C# 8.0
- **Framework** : .NET 8.0
- **Moteur graphique** : Raylib-cs 7.0.1
- **Plateforme** : Multiplateforme (Windows/macOS/Linux)

## 🎨 Fonctionnalités techniques

### Système de grille intelligent
- Conversion automatique coordonnées ↔ monde
- Validation de positions avec types de cellules
- Gestion optimisée des objets sur la grille

### Algorithmes de territoire
- **Flood Fill** pour détecter les zones fermées
- Calcul en temps réel du pourcentage conquis
- Système d'événements pour les changements

### Gestion d'état avancée
- Machine à états pour le serpent
- États de jeu avec transitions fluides
- Sauvegarde d'état pour restart/continue

## 🔮 Fonctionnalités futures

- [ ] Niveaux avec obstacles prédéfinis
- [ ] Ennemis mobiles dans les zones libres
- [ ] Power-ups temporaires supplémentaires
- [ ] Système de classement et records
- [ ] Mode multijoueur local
- [ ] Éditeur de niveaux personnalisés

## 🤝 Contribution

Les contributions sont les bienvenues ! Pour contribuer :

1. Forkez le projet
2. Créez une branche pour votre fonctionnalité (`git checkout -b feature/amazing-feature`)
3. Committez vos changements (`git commit -m 'Add amazing feature'`)
4. Pushez vers la branche (`git push origin feature/amazing-feature`)
5. Ouvrez une Pull Request

### Guidelines de développement
- Respectez les conventions de nommage C#
- Ajoutez des commentaires pour les logiques complexes
- Testez vos modifications sur différentes plateformes
- Maintenez la compatibilité avec .NET 8.0

## 📝 Licence

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de détails.

## 👨‍💻 Auteur

Développé avec ❤️ par Frédéric LAURENZI
## 🆘 Support

Si vous rencontrez des problèmes :
1. Vérifiez que .NET 8.0 est installé
2. Assurez-vous que Raylib-cs est correctement restauré
3. Consultez les Issues GitHub pour les problèmes connus
4. Créez une nouvelle Issue si le problème persiste

---

**Amusez-vous bien en jouant à SnakeJezzBall ! 🎮**
