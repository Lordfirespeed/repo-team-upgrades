using System;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.Device;

namespace RepoPlayground;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    static Plugin()
    {
        RootCommand rootCommand = new RootCommand {
            TreatUnmatchedTokensAsErrors = false,
        };
        Option<FileInfo> versionDumpFileOption = new Option<FileInfo>(
            aliases: ["--version-dump-file"]
        );
        rootCommand.AddOption(versionDumpFileOption);

        var args = Environment.GetCommandLineArgs();
        var result = rootCommand.Parse(args);
        VersionDumpFile = result.GetValueForOption(versionDumpFileOption);
    }

    internal static FileInfo? VersionDumpFile { get; }
    internal static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;

    private CancellationTokenSource? _cts;

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

    private void OnDestroy()
    {
        if (_cts is { IsCancellationRequested: false }) _cts.Cancel();
    }

    private void DumpVersionAndExit(string versionString)
    {
        if (_cts is not null) {
            throw new InvalidOperationException("DumpVersionAndExit has already been invoked");
        }

        _cts = new();
        Task.Run(async () => await DumpVersionAndExit(versionString, _cts.Token), _cts.Token);
    }

    private async Task DumpVersionAndExit(string versionString, CancellationToken ct)
    {
        await DumpVersion(versionString, ct);
        if (ct.IsCancellationRequested) return;
        Application.Quit();
    }

    private async Task DumpVersion(string versionString, CancellationToken ct)
    {
        var versionDumpFile = VersionDumpFile;
        if (versionDumpFile is null) {
            var pluginDirectory = new DirectoryInfo(Path.GetDirectoryName(Info.Location)!);
            versionDumpFile = new FileInfo(Path.Combine(pluginDirectory.FullName, "version.txt"));
        }

        await using var versionDumpStream = new StreamWriter(versionDumpFile.FullName);
        await versionDumpStream.WriteLineAsync(versionString);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class Patches
    {
        [HarmonyPatch(typeof(BuildManager), nameof(BuildManager.Awake))]
        [HarmonyPostfix]

        static void AfterBuildManagerWakes(BuildManager __instance)
        {
            if (!ReferenceEquals(__instance, BuildManager.instance)) {
                return;
            }

            var versionString = BuildManager.instance.version.title;
            Plugin.Logger.LogInfo($"Version information: {versionString}");
            Plugin.Instance.DumpVersionAndExit(versionString);
        }
    }
}
