#nullable enable
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Color = UnityEngine.Color;

#if MONO
using ScheduleOne;
using ScheduleOne.DevUtilities;
using ScheduleOne.ItemFramework;
using ScheduleOne.PlayerScripts;
using FishNet;
#else
using Il2CppInterop.Runtime;
using Il2CppScheduleOne;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppFishNet;
using Object = Il2CppSystem.Object;
#endif

namespace DomsExpandedIngredientsAndEffects.Helpers;

/// <summary>
/// Provides extension methods for converting between C# and Il2Cpp lists.
/// </summary>
public static class Il2CppListExtensions
{
    /// <summary>
    /// Converts a C# <see cref="List{T}"/> to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static IEnumerable<T> AsEnumerable<T>(this List<T> list)
    {
        return list ?? Enumerable.Empty<T>();
    }

    /// <summary>
    /// Converts the provided list to a backend-native list.
    /// </summary>
    public static object ToNativeList<T>(this List<T> source)
    {
#if !MONO
        return source.ToIl2CppList();
#else
        return source ?? new List<T>();
#endif
    }

#if !MONO
    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> to an Il2Cpp list.
    /// </summary>
    public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(
        this IEnumerable<T> source)
    {
        var il2CppList = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var item in source)
            il2CppList.Add(item);
        return il2CppList;
    }

    /// <summary>
    /// Converts an Il2Cpp list to a C# list.
    /// </summary>
    public static List<T> ConvertToList<T>(Il2CppSystem.Collections.Generic.List<T> il2CppList)
    {
        List<T> csharpList = new List<T>();
        T[] array = il2CppList.ToArray();
        csharpList.AddRange(array);
        return csharpList;
    }

    /// <summary>
    /// Converts an Il2Cpp list to a C# IEnumerable.
    /// </summary>
    public static IEnumerable<T> AsEnumerable<T>(this Il2CppSystem.Collections.Generic.List<T> list)
    {
        return list == null ? Enumerable.Empty<T>() : list._items.Take(list._size);
    }

    /// <summary>
    /// Returns the Il2Cpp list as-is (already native).
    /// </summary>
    public static object ToNativeList<T>(this Il2CppSystem.Collections.Generic.List<T> source)
    {
        return source;
    }
#endif
}

/// <summary>
/// Common utility functions for the mod.
/// </summary>
public static class Utils
{
    private static readonly MelonLogger.Instance Logger = new MelonLogger.Instance(
        "DomsExpandedIngredientsAndEffects-Utils"
    );

    /// <summary>
    /// Searches all loaded objects of type T and returns the first one matching the given name.
    /// </summary>
    public static T? FindObjectByName<T>(string objectName)
        where T : UnityEngine.Object
    {
        try
        {
            foreach (var obj in Resources.FindObjectsOfTypeAll<T>())
            {
                if (obj.name != objectName)
                    continue;
                Logger.Msg($"Found {typeof(T).Name} '{objectName}' directly in loaded objects");
                return obj;
            }
            return null;
        }
        catch (Exception ex)
        {
            Logger.Error($"Error finding {typeof(T).Name} '{objectName}': {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets all components of type T in the given GameObject and its children recursively.
    /// </summary>
    public static List<T> GetAllComponentsInChildrenRecursive<T>(GameObject obj)
        where T : Component
    {
        var results = new List<T>();
        if (obj == null) return results;

        T[] components = obj.GetComponents<T>();
        if (components.Length > 0)
            results.AddRange(components);

        for (var i = 0; i < obj.transform.childCount; i++)
        {
            var child = obj.transform.GetChild(i);
            results.AddRange(GetAllComponentsInChildrenRecursive<T>(child.gameObject));
        }

        return results;
    }

    /// <summary>
    /// Checks if the given object is of type T and casts it to that type.
    /// </summary>
    public static bool Is<T>(object obj, out T? result)
#if !MONO
        where T : Object
#else
        where T : class
#endif
    {
#if !MONO
        if (obj is Object il2CppObj)
        {
            var targetType = Il2CppType.Of<T>();
            var objType = il2CppObj.GetIl2CppType();
            if (targetType.IsAssignableFrom(objType))
            {
                result = il2CppObj.TryCast<T>()!;
                return result != null;
            }
        }
#else
        if (obj is T t)
        {
            result = t;
            return true;
        }
#endif
        result = null;
        return false;
    }

    /// <summary>
    /// Waits for the player to be ready before starting the given coroutine.
    /// </summary>
    public static IEnumerator WaitForPlayer(IEnumerator routine)
    {
        while (Player.Local == null || Player.Local.gameObject == null)
            yield return null;
        MelonCoroutines.Start(routine);
    }

    /// <summary>
    /// Waits for the network to be ready before starting the given coroutine.
    /// </summary>
    public static IEnumerator WaitForNetwork(IEnumerator routine)
    {
        while (InstanceFinder.IsServer == false && InstanceFinder.IsClient == false)
            yield return null;
        MelonCoroutines.Start(routine);
    }

    /// <summary>
    /// Waits until the given condition is true, with optional timeout and callbacks.
    /// </summary>
    public static IEnumerator WaitForCondition(
        System.Func<bool> condition,
        float timeout = Single.NaN,
        Action? onTimeout = null,
        Action? onFinish = null)
    {
        var startTime = Time.time;
        while (!condition())
        {
            if (!float.IsNaN(timeout) && Time.time - startTime > timeout)
            {
                onTimeout?.Invoke();
                yield break;
            }
            yield return null;
        }
        onFinish?.Invoke();
    }

    /// <summary>
    /// Gets the full hierarchy path of a Transform.
    /// </summary>
    public static string GetHierarchyPath(this Transform transform)
    {
        if (transform == null) return "null";
        var path = transform.name;
        var current = transform.parent;
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        return path;
    }

    /// <summary>
    /// Gets a component or adds it if it doesn't exist.
    /// </summary>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();
        if (component != null) return component;
        component = gameObject.AddComponent<T>();
        Logger.Msg($"Added component {typeof(T).Name} to GameObject {gameObject.name}");
        return component;
    }

    /// <summary>
    /// Draws debug visuals on the given GameObject.
    /// </summary>
    public static Material? DrawDebugVisuals(this GameObject gameObject, Color? color = null)
    {
        var renderer = gameObject.GetComponent<Renderer>();
        if (renderer == null)
        {
            Logger.Error($"GameObject {gameObject.name} has no Renderer component");
            return null;
        }

        color ??= new Color(1f, 0f, 1f, 0.5f);
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) return null;

        var mat = new Material(shader);
        if (mat.HasProperty("_Surface"))
            mat.SetFloat("_Surface", 1f);

        var baseColor = color.Value;
        if (baseColor.a <= 0f) baseColor.a = 0.2f;

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", baseColor);

        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", new Color(baseColor.r, baseColor.g, baseColor.b) * 1.5f);
        }

        mat.SetInt("_ZWrite", 0);
        mat.renderQueue = 3000;

        var originalMaterial = renderer.material;
        renderer.material = mat;
        return originalMaterial;
    }
}