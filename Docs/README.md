# Depra.Ecs.Baking - Unity Conversion Workflow for [Depra.Ecs](https://github.com/Leopotam/ecslite)

<div>
    <strong><a href="README.md">English</a> | <a href="README.RU.md">Русский</a></strong>
</div>

<details>
<summary>Table of Contents</summary>

- [Introduction](#-introduction)
    - [Features](#-features)
- [Installation](#-installation)
- [Contents](#-contents)
- [Usage Examples](#-usage-examples)
- [Dependencies](#-dependencies)
- [Collaboration](#-collaboration)
- [Support](#-support)
- [License](#-license)

</details>

## 🧾 Introduction

Important! This repository is extension to [Depra.Ecs](https://github.com/Depra-Inc/Ecs) -
Engine independent ECS that works with any Game Engine.
But Unity Engineers often ask how to integrate Leo with Unity Inspector and deal with Prefabs.
This lightweight repository is intended to help with this.

## 📥 Installation

**First** you need to install [Depra.Ecs](https://github.com/Depra-Inc/Ecs),
you can do it with Unity Package Manager

Add new line to `Packages/manifest.json`

```
"com.leopotam.ecslite": "https://github.com/voody2506/ecslite.git",
```

**Second** install this repository

```
"com.voody.unileo.lite": "https://github.com/voody2506/UniLeo-Lite.git",
```

### 📦 Using **UPM**:

1. Open the **Unity Package Manager** window.
2. Click the **+** button in the upper right corner of the window.
3. Select **Add package from git URL...**.
4. Enter the base [repository link](https://github.com/Depra-Inc/Ecs.Unity.git).
5. Click **Add**.
6. Repeat steps 2-5 for this [repository](https://github.com/Depra-Inc/Ecs.Baking.git).

### ⚙️ Manual:

Add the following lines to `Packages/manifest.json` in the `dependencies` section:

```
"com.depra.ecs": "https://github.com/Depra-Inc/Ecs.Unity.git"
"com.depra.ecs.baking": "https://github.com/Depra-Inc/Ecs.Baking.git"
```

## 📋 Usage Examples

### Create your component

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

Create new script with `ComponentBaker`.

```csharp
public sealed class HealthComponentBaker : ComponentBaker<HealthComponent> { }
```

Add `HealthComponentBaker` into **Inspector**
<details>
  <summary>Inspector Preview</summary>

![Health Component Baker](https://i.postimg.cc/RVNG1K36/health-component.jpg)
</details>

Now you can control component values within the **Inspector**. Congratulations!

> ⚠️ At this moment you can not control values from **Inspector** at **Runtime**

<details>
  <summary>Choose conversion mode</summary>

![Convert Mode](https://i.postimg.cc/J04qyBXq/convert-method.jpg)

| Mode                | Description                                                   |
|---------------------|---------------------------------------------------------------|
| Convert And Inject  | Just creates entities with components based on GameObject     |
| Convert And Destroy | Deletes GameObject after conversion                           |
| Convert And Save    | Stores associated GameObject entity in ConvertToEntity Script |

```csharp
// Than just get value from ConvertToEntity MonoBehavior
if (ConvertToEntity.TryGetEntity().HasValue) {
    ConvertToEntity.TryGetEntity().Value
}
```

</details>

### Convert your GameObjects to Entity

You cant create `MonoBehaviour` for Startup ECS. 
To Automatically convert **GameObjects** to **Entity** add `ConvertScene()` method.

```csharp
void Start() 
{
    _world = new World();    
    _systems = new WorldSystems(_world);
    _systems
        .ConvertScene() // <- Need to add this method
        .Add(new ExampleSystem());
    
    _systems.Initilize();
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

- [Depra.Ecs](https://github.com/Depra-Inc/Ecs.Unity.git) - the base library for working with assets (provided
  with this **UPM** package).

## 🤝 Collaboration

I welcome feature requests and bug reports in
the [issues section](https://github.com/Depra-Inc/Ecs.Unity/issues), and I also
accept [pull requests](https://github.com/Depra-Inc/Ecs.Unity/pulls).

## 🫂 Support

I am an independent developer, and most of the development of this project is done in my free time. If you are
interested in collaborating or hiring me for a project, please check out
my [portfolio](https://github.com/Depra-Inc) and [contact me](mailto:g0dzZz1lla@yandex.ru)!

## 🔐 License

This project is distributed under the
**[Apache-2.0 license](https://github.com/Depra-Inc/Ecs.Unity/blob/main/LICENSE.md)**

Copyright (c) 2023 Nikolay Melnikov
[n.melnikov@depra.org](mailto:n.melnikov@depra.org)