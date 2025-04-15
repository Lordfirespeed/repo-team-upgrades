using System;
using System.Diagnostics.CodeAnalysis;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace TeamUpgrades;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;

    public Plugin()
    {
        if (Instance is not null) {
            throw new InvalidOperationException($"{typeof(Plugin).AssemblyQualifiedName} was instantiated twice");
        }
        Instance = this;
    }

    private void Awake()
    {
        Logger = base.Logger;
        var harmony = new Harmony(Info.Metadata.GUID);
        harmony.PatchAll(typeof(Patches));
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class Patches
    {

    }
}
