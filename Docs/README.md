# Depra.Ecs.Baking - Unity Conversion Workflow for [Depra.Ecs](https://github.com/Leopotam/ecslite)

![License](https://img.shields.io/github/license/Depra-Inc/Ecs.Baking)
![Last Commit](https://img.shields.io/github/last-commit/Depra-Inc/Ecs.Baking)
![Code Size](https://img.shields.io/github/languages/code-size/Depra-Inc/Ecs.Baking)

<div>
    <strong><a href="README.md">English</a> | <a href="README.RU.md">Русский</a></strong>
</div>

<details>
<summary>Table of Contents</summary>

- [Introduction](#-introduction)
    - [Features](#-features)
- [Installation](#-installation)
- [Usage Examples](#-usage-examples)
    - [Create Your component](#create-your-component)
    - [Create a new ComponentBaker](#create-a-new-componentbaker)
    - [Choose conversion mode](#choose-conversion-mode)
    - [Convert Your GameObjects to Entity](#convert-your-gameobjects-to-entity)
    - [Spawn Prefabs](#spawn-prefabs)
    - [Working with Unity Editor Extension](#working-with-unity-editor-extension)
- [Dependencies](#-dependencies)
- [Collaboration](#-collaboration)
- [Support](#-support)
- [License](#-license)

</details>

## 🧾 Introduction

This repository is extension to [Depra.Ecs](https://github.com/Depra-Inc/Ecs) -
Engine independent ECS that works with any Game Engine.
But Unity Engineers often ask how to integrate **Depra.Ecs** with **Unity Inspector** and deal with Prefabs.
This lightweight repository is intended to help with this.

### 💡 Features

- **Open Source**: This library is open source and free to use.
- **Easy to use**: Just add `ComponentBaker` to your component and add `ConvertScene` method to your `WorldSystems`.
- **Convert Modes**: You can choose how to convert **GameObjects to **Entity**.
- **Prefab support**: You can spawn Prefabs with `ComponentBaker` and it will be converted to **Entity** after spawn.
- **Extensibility**: A flexible architecture for extending functionality according to your needs.
- **Entities-like**: This library is similar to **Entities** conversion workflow.
- **Lightweight**: This library is lightweight and has single dependency.
- **Declarative**: You can control your component values within the **Unity Inspector**.

## 📥 Installation

First of all you need to install [Depra.Ecs](https://github.com/Depra-Inc/Ecs.git). 
Just add .dll to your project.

### 📦 Using **UPM**:

1. Open the **Unity Package Manager** window.
2. Click the **+** button in the upper right corner of the window.
3. Select **Add package from git URL...**.
4. Enter the [repository link](https://github.com/Depra-Inc/Ecs.Baking.git).
5. Click **Add**.

### ⚙️ Manual:

Add the following line to `Packages/manifest.json` in the `dependencies` section:

```
"com.depra.ecs.baking": "https://github.com/Depra-Inc/Ecs.Baking.git"
```

## 📋 Usage Examples

### Create Your component

```csharp
[Serializable] // <- Important to add Serializable attribute
public struct HealthComponent 
{
    public float Value;
}
```

Now you need to control health value within the **Unity Inspector**,
but **Unity Engine** works only with `MonoBehaviour` classes.
That mean you need to create **Baker** for our component.

### Create a new ComponentBaker.

```csharp
public sealed class HealthComponentBaker : ComponentBaker<HealthComponent> { }
```

Add `HealthComponentBaker` into **Inspector**
<details>
  <summary>Inspector preview</summary>

![Health Component Baker](https://i.postimg.cc/RVNG1K36/health-component.jpg)
</details>

Now you can control component values within the **Inspector**. Congratulations!

> ⚠️ At this moment you can not control values from **Inspector** at **Runtime**

### Choose conversion mode

<details>
  <summary>Inspector preview</summary>

![Convert Mode](https://i.postimg.cc/J04qyBXq/convert-method.jpg)
</details>

| Mode                | Description                                                   |
|---------------------|---------------------------------------------------------------|
| Convert And Inject  | Just creates entities with components based on GameObject     |
| Convert And Destroy | Deletes GameObject after conversion                           |
| Convert And Save    | Stores associated GameObject entity in ConvertToEntity Script |

You can also retrieve a value from **ConvertibleEntity**:

```csharp
if (ConvertToEntity.TryGetEntity().HasValue) {
    ConvertToEntity.TryGetEntity().Value
}
```

### Convert Your GameObjects to Entity

You cant create `MonoBehaviour` for startup ECS.
To Automatically convert **GameObjects** to **Entity** add `WorldSystemsExtensions.ConvertScene()` method.

```csharp
void Start() 
{
    _world = new World();    
    _systems = new WorldSystems(_world);
    _systems
        .ConvertScene() // <- Need to add this method
        .Add(new ExampleSystem());
    
    _systems.Initialize();
 }
```

**ConvertScene** - method that automatically scan world,
finds GameObjects with `ComponentBaker`,
creates entity and adds initial Components to the Entity.

### Spawn Prefabs

Every Prefab initialize with new entity. Components will be added automatically

```csharp
GameObject.Instantiate(gameObject, position, rotation);
PhotonNetwork.Instantiate <- works in 3rd party Assets
```

### Working with Unity Editor Extension

Please, add `ConvertScene` method **after** UnityEditor extension

```csharp
#if UNITY_EDITOR
        // Add debug systems for custom worlds here, for example:
        .Add(new WorldDebugSystem())
#endif
        .ConvertScene() // <- Need to add this method
```

## 🖇 Dependencies

- [Depra.Ecs](https://github.com/Depra-Inc/Ecs.git) - the base ecs library.

## 🤝 Collaboration

I welcome feature requests and bug reports in
the [issues section](https://github.com/Depra-Inc/Ecs.Baking/issues), and I also
accept [pull requests](https://github.com/Depra-Inc/Ecs.Baking/pulls).

## 🫂 Support

I am an independent developer, and most of the development of this project is done in my free time. If you are
interested in collaborating or hiring me for a project, please check out
my [portfolio](https://github.com/Depra-Inc) and [contact me](mailto:g0dzZz1lla@yandex.ru)!

## 🔐 License

This project is distributed under the
**[Apache-2.0 license](https://github.com/Depra-Inc/Ecs.Baking/blob/main/LICENSE.md)**

Copyright (c) 2023 Nikolay Melnikov
[n.melnikov@depra.org](mailto:n.melnikov@depra.org)