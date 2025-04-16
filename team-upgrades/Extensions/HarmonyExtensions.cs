using System;
using System.Reflection;
using HarmonyLib;

namespace TeamUpgrades.Extensions;

public static class HarmonyExtensions
{
    private const BindingFlags SearchNestedTypeBindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;

    private static Type[] AllNestedTypesOf(Type type)
    {
        return type.GetNestedTypes(SearchNestedTypeBindingFlags);
    }

    public static void PatchAllNestedTypes(this Harmony harmony, Type type)
    {
        foreach (var nestedType in AllNestedTypesOf(type)) {
            harmony.PatchAll(nestedType);
        }
    }

    public static void PatchAllNestedTypesRecursively(this Harmony harmony, Type type)
    {
        foreach (var nestedType in AllNestedTypesOf(type))
        {
            harmony.PatchAll(nestedType);
            PatchAllNestedTypesRecursively(harmony, nestedType);
        }
    }
}
