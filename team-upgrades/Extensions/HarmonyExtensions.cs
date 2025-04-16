using System;
using System.Collections;
using System.Collections.Generic;
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

    // https://gist.github.com/pardeike/c873b95e983e4814a8f6eb522329aee5
    public class SimpleEnumerator<T> : IEnumerable<T>
    {
        public required IEnumerator<T> Enumerator { get; init; }
        public Action PrefixAction { get; init; } = () => { };
        public Action PostfixAction { get; init; } = () => { };
        public Action<T> PreItemAction { get; init; } = item => { };
        public Action<T> PostItemAction { get; init; } = item => { };
        public Func<T, T> ItemAction { get; init; } = item => item;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() { return _GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return _GetEnumerator(); }

        private IEnumerator<T> _GetEnumerator()
        {
            PrefixAction();
            while (Enumerator.MoveNext())
            {
                var item = Enumerator.Current;
                PreItemAction(item);
                yield return ItemAction(item);
                PostItemAction(item);
            }
            PostfixAction();
        }
    }
}
