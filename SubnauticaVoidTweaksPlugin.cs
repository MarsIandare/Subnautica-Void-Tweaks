using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UWE;

using Nautilus.Options.Attributes;
using Nautilus.Options;
using Nautilus.Handlers;
using Nautilus.Json;

namespace SubnuaticaVoidTweaksMod
{
    // TODO Review this file and update to your own requirements.

    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class SubnauticaVoidTweaksPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "marslandare.voidtweaks";
        private const string PluginName = "Void Tweaks Mod";
        private const string VersionString = "1.0.0";
        
        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);
        internal static Options options { get; } = OptionsPanelHandler.RegisterModOptions<Options>();

        /// <summary>
        /// Initialise the configuration settings and patch methods
        /// </summary>
        private void Awake()
        {
            // Apply all of our patches and say everything loaded.
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            
            Log = Logger;
        }

        [Menu("Void Tweaks")]
        public class Options : Nautilus.Json.ConfigFile
        {
            [Toggle("Clear the Void", Tooltip = "Clears the Void of all Creatures")]
            public bool RemoveVoidCreatures = false;

            [Toggle("Retreats when Leaving", Tooltip = "Clears the Void of all Creatures when you leave the it")]
            public bool RetreatWhenLeave = true;

            [Slider("Creature Spawn Rate", DefaultValue = 20f, Min = 2f, Max = 200f, Step = 0.5f, Tooltip = "Change how fast creatures spawn (default 20 seconds)")]
            public float CreatureSpawnRate = 20;

            [Slider("Maximum Creature Capacity", DefaultValue = 3, Min = 0, Max = 150, Step = 1, Tooltip = "Change the limit of how many creatures that can simultaneously be in the void (default 3)")]
            public int MaxCreatureCapacity = 3;

            [Slider("Initial Grace Period", DefaultValue = 3, Min = 0, Max = 15, Step = 0.1f, Tooltip = "The initial time before the first creature spawns (default 3, which corresponds to 30 seconds)")]
            public float InitialGracePeriod = 3;

            [Slider("Creature Spawn Distance", DefaultValue = 50, Min = 30, Max = 100, Step = 0.5f, Tooltip = "The distance at which creatures spawn from you (default 50)")]
            public float CreatureSpawnDistance = 50;

            [Choice("Void Creature Prefab", Tooltip = "The type of creature that spawns when entering the void, only choose things that could work, as it might cause issues (default Ghost Leviathan)")]
            public TechType VoidCreaturePrefab = TechType.GhostLeviathan;

            [Slider("Void Creature Size (x)", Format = "{0:F1}x", DefaultValue = 1, Min = 0.1f, Max = 10, Step = 0.1f, Tooltip = "Multiplies the size of the Creatures (default 1)")]
            public float VoidCreatureSize = 1f;
        }
    }
}
