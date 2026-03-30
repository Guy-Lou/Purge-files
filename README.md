# README – Programme de purge de fichiers

## Description

Ce programme, développé en C# dans le cadre d’un stage de seconde, a pour objectif de simuler et tester des mécanismes de purge de fichiers dans des environnements professionnels tels que la Gestion Électronique de Documents (GED) et les Échanges de Données Informatisés (EDI).

Il permet de générer une arborescence de fichiers de test organisée par date (année / mois / jour) et d’appliquer des opérations de suppression selon une logique configurable.

Ce projet vise à reproduire des problématiques réelles liées à la gestion du cycle de vie des données en entreprise.

## Fonctionnalités

### Génération de données
- Création automatique d’une structure de dossiers hiérarchique (année / mois / jour)
- Génération de fichiers de test pour simuler un volume de données

### Mécanisme de purge
- Suppression de fichiers (actuellement basée en partie sur un comportement aléatoire)
- Gestion de deux environnements :
  - GED
  - EDI
- Paramétrage de règles de purge :
  - taille de traitement (clusters GED)
  - durée de conservation des fichiers EDI

### Modes d’exécution
- Exécution ponctuelle
- Exécution planifiée :
  - par cycles
  - ou selon des plages horaires définies

## Configuration

Le programme est entièrement configurable via un fichier JSON.

### Exemple de configuration

```json
{
    "GestionPurgeOptions": {
        "CheminGed": "",
        "CheminEDI": "",
        "TailleClusterGED": 0,
        "PéremptionEDIJours": 0
    },
    "GestionPlannificationOptions": {
        "ModeGestion": "Cycles"
    },
    "GestionPlannificationHorairesOptions": {
        "JourSemaine": 0,
        "HeureDébut": "00:00:00",
        "DuréeMax": "00:00:00"
    },
    "GestionPlannificationCyclesOptions": {
        "DelaiPurge": "00:00:00",
        "ComportementDélaiDépassé": "ContinuerAprèsProchainCycle"
    },
    "ConnectionStrings": {
        "powerhermesEntities": ""
    },
    "Logging": {
        "LogLevel": {
            "Default": "Information"
        },
        "GELF": {
            "Host": "",
            "LogSource": "GestionPurge"
        }
    }
}```

## Description des paramètres

### GestionPurgeOptions
- CheminGed : chemin vers les données GED
- CheminEDI : chemin vers les données EDI
- TailleClusterGED : taille maximale des ensembles de fichiers à traiter
- PéremptionEDIJours : durée de conservation des fichiers EDI (en jours)

### GestionPlannificationOptions
- ModeGestion : mode d’exécution (Cycles ou planification horaire)

### GestionPlannificationHorairesOptions
- JourSemaine : jour d’exécution (0 = dimanche)
- HeureDébut : heure de lancement
- DuréeMax : durée maximale d’exécution

### GestionPlannificationCyclesOptions
- DelaiPurge : intervalle entre deux purges
- ComportementDélaiDépassé : comportement si une purge dépasse le délai prévu

### ConnectionStrings
- Paramètres de connexion à une base de données

### Logging
- Configuration des journaux d’exécution
- Possibilité d’export vers un serveur GELF

## Cas d’utilisation

- Test de systèmes de gestion documentaire (GED)
- Simulation de flux de données EDI
- Validation de stratégies de purge et de rétention
- Génération de données de test pour environnement de développement

## État du projet

- Génération de structure de fichiers fonctionnelle
- Mécanisme de purge implémenté
- Système de planification opérationnel
- Configuration via JSON complète
- Améliorations possibles :
  - règles de purge plus avancées
  - optimisation des performances
  - amélioration du système de logs

## Contexte de réalisation

Projet réalisé dans le cadre d’un stage de seconde.

Objectifs :
- découvrir le développement en C#
- manipuler le système de fichiers
- concevoir une application configurable
- comprendre les enjeux liés à la gestion et à la suppression de données en environnement professionnel