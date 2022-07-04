# Boid-Algorithmus Implementierunng mit Optimierungen und Anwendung auf Hasse-Diagramme
Szenen sind im Ordner Boids\Assets\Scenes vorhanden. Dieses Projekt wurde mit **Unity 2020.3.30f1** erstellt.

## Wichtige gemeinsame Klassen
Im Order Boids\Assets\Scripts\Common befindet sich:
- SpatialHasher.cs
- HelperMethods.cs
- BoidSettingsWithBehaviourBase.cs
- BoidsManagerBase.cs


## Wichtige Methoden der Boid-Klasse OOP
In der Datei Boids\Assets\Scripts\Conventional\Boid.cs ist die Boid-Klasse:
- LimitBoidInScreen()
- CalculateAllBehaviour(...)
- ApplyAndResetForce()
- MoveAndRotateBoid()
- BoidWillCollide()
- TryGoToPheromone()
- BoidHasAccessToFood()
- SpreadPheromone()


## Wichtige Klassen OOP
- Boids\Assets\Scripts\Conventional\BoidCollisionHelper.cs (Kollisionsdetektion mit Golden Spiral Method)
- Boids\Assets\Scripts\Conventional\BoidsManager.cs (Instanziierung und Aktualisierung von Boid-GameObjects)
Die verwendete k-d-Baum-Implementierung befindet sich in Boids\Assets\Scripts\Conventional\KD_Tree_Stuff und die benutzte Octree-Implementierung befindet sich in Boids\Assets\Scripts\Conventional\OC_Tree_Stuff
### Klassen OOP bezüglich Futter- und Pheromonmodellierung befinden sich in:
- Boids\Assets\Scripts\Conventional\FoodHandling

## Klassen bezüglich Hasse-Diagramme OOP:
- Boids\Assets\Scripts\HasseTesting\HasseDiagramBoidSpawner.cs (Methoden zur Abbildung und Anwendung der Graphvisualisierung)
- Boids\Assets\Scripts\HasseTesting\NodeBoid.cs (Modellierung eines Boids auf einen Graphknoten)

## Wichtige Klassen/Strukturen bezüglich Datenorientierung und Multithreading (Unity DOTS)
- Boids\Assets\Scripts\ECS\System\BoidMovementSystemSpatialHashing.cs
- Boids\Assets\Scripts\ECS\System\BoidRotationSystem.cs
- Boids\Assets\Scripts\ECS\System\BoidConfigurationSystem.cs
- Boids\Assets\Scripts\ECS\System\BoidLimitInSpaceSystem.cs
- Strukturen/Daten befinden sich in: Boids\Assets\Scripts\ECS\Data
- Boids\Assets\Scripts\ECS\BoidsManagerDOTS.cs (Instanziierung und Aktualisierung von Boid-Entitäten)