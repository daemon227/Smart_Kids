# Dot Puzzle Game - AI Coding Assistant Instructions

## Project Overview
**Dot Puzzle** is a Unity 2D puzzle game where players fill polygons by connecting dots. The architecture uses a **Manager-based singleton pattern** with event-driven communication between systems.

**Key Info**: C# codebase (Unity 2022+), organized under `Assets/00 Game/01Scripts/` with system folders numbered 00-08.

## Architecture & Component Layout

### Core Systems (Singleton Managers)
All major systems use **static singleton pattern** with `.Instance`:

| System | Location | Responsibility |
|--------|----------|-----------------|
| **GameManager** | `00GameManager/` | Game state (isGameOver, canInteract, levelData), level loading, level progression |
| **EventManager** | `00GameManager/` | Central event hub using C# `Action` delegates (100+ game events) |
| **PolygonFiller** | `01PolygonSystem/FillSystem/` | Fills completed polygons with flower sprites using animation system |
| **CameraController** | `04CamaraController/` | Handles camera dragging, zooming, centering on polygons using DOTween |
| **InsectSpawner** | `07InsectSystem/Manager/` | Spawns insects (ladybugs) that can damage/complete polygons |

### Data Structures
- **Polygon** (`01PolygonSystem/Entities/Polygon.cs`): Represents a single polygon with `vertices` list and `edges`. Key method: `GetCentroid()` for geometric center calculation.
- **Vertice** (`01PolygonSystem/Entities/Vertice.cs`): Individual dots/vertices that form polygon edges
- **LevelData** (`03LevelData/`): Contains `List<StepData>` where each step has multiple polygons. Loaded from `AllLevelDataSO` (ScriptableObject).
- **StepData**: Groups polygons into logical steps/stages

### Helper Utilities
- **PolygonManager** (`01PolygonSystem/Helper/`): Non-MonoBehaviour helper for polygon queries (`GetGlobalPolygonRange()`, `GetRandomCompletedPolygon()`, `GetGlobalCentroid()`)

## Critical Event Flow

```
Level Load → OnLevelLoadedEvent (GameManager)
    ↓
Player fills polygon → OnPolygonComplete (EventManager)
    ↓
PolygonFiller.Fill() + FilledOnePolygon event
    ↓
All polygons done → FillAllPolygons event
    ↓
Step complete → OnStepComplete event
    ↓
CameraController moves to next polygon, EventManager triggers ChangeToNewStep
```

**Key**: Most systems respond to `EventManager.Instance` events rather than direct calls. Event subscriptions happen in `Start()`, unsubscriptions in `OnDisable()`.

## Development Patterns & Conventions

### Namespace Organization
- `Inwave.DongA.DotPuzzle.Manager` - Game managers (GameManager, CameraController, HintManager)
- `Inwave.DongA.DotPuzzle.Entity` - Core data (Polygon, Vertice)
- `Inwave.DongA.DotPuzzle.Event` - Event system (EventManager)
- `Inwave.DongA.DotPuzzle.UI*` - UI components (popups, step UI)
- `Inwave.DongA.DotPuzzle.InsectSystem` - Insect mechanics

### Common Patterns
1. **Coroutine-based animations**: Use `IEnumerator` + `StartCoroutine()` for timed effects (fill animation, camera movement)
2. **DOTween for smooth transitions**: `camera.DOOrthoSize()`, `transform.DOMove()` with easing curves
3. **Conditional state checks**: All Update() loops check `GameManager.Instance.canInteract` and `isGameOver` before processing input
4. **Physics2D for hit detection**: `Physics2D.OverlapCircle()` to detect clicks on interactive elements

### Important Gotchas
- **LevelData is instantiated per level**: `GameManager.LoadLevelData()` destroys old `levelData.gameObject` and loads new one
- **PolygonManager is NOT a MonoBehaviour**: Instantiate with `new PolygonManager()`, doesn't use `.Instance`
- **currentStepIndex tracking**: Used for calculating visible polygons in camera bounds and filling logic
- **Z-position layering**: PolygonFiller calculates Z offset from global centroid distance to create depth: `distToGlobal * theme.globalZMultiplier`
- **Sprite assignment randomization**: Fill data can use `fillRandom` to randomly assign flower sprites from `FlowerSO`

## Building & Testing

### Unity Build Setup
- **Solution**: `dot-puzzle.sln` contains 8 projects (Assembly-CSharp, spine-unity, MyExtension, etc.)
- **Main assembly**: `Assembly-CSharp.csproj` compiles all game scripts
- **Editor tools**: `Assembly-CSharp-Editor.csproj` for custom editor scripts
- **Target platform**: 2D, runs at 60 FPS in builds (see `Application.targetFrameRate = 60` in GameManager)

### Editor Testing
- **Test Mode**: PlayerPrefs flag `TEST=1` enables custom level loading from text (see GameManager.LoadLevelData())
- **Custom levels**: Load via `SaveLoadData.LoadLevelFromContent()` instead of `AllLevelDataSO`

## External Dependencies
- **DoTween**: Animation library for smooth transitions (`DG.Tweening`)
- **TextMesh Pro**: Text rendering (TextMeshProUGUI in UI components)
- **Spine Animation**: Animation framework (spine-unity, spine-csharp packages)
- **Unity Addressables**: Asset loading system (1.22.3)
- **Visual Scripting**: Unity's node-based scripting (1.9.4)

## Quick Navigation
- **Polygon filling logic**: `PolygonFiller.FillPolygon()` (coroutine-based, spawns sprites based on FillData)
- **Camera behavior**: `CameraController.DragCamera()`, `ZoomCamera()`, handles touch input
- **UI management**: `02UIManager/` - popups (LosePopup, WinPopup, SettingPopup) inherit from PopupBase
- **Insect AI**: `07InsectSystem/Entities/LaydyBug.cs` - moves between completed polygons, damages on contact
- **Level progression**: `GameManager.NextLevel()`, `ReplayLevel()` wraps around to level 1 after final level
