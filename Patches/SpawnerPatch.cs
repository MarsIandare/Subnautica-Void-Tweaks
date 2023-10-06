using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using BepInEx.Logging;
using static VFXParticlesPool;
using BepInEx.Configuration;
using UnityEngine;
using System.Collections;
using SubnuaticaVoidTweaksMod;

namespace SubnuaticaVoidTweaksMod
{
    internal class SpawnerPatch : MonoBehaviour
    {
        public static GameObject VoidPrefab = null;
        public static GameObject GhostLevithanVoidPrefab = null;

        [HarmonyPatch(typeof(VoidGhostLeviathansSpawner))]
        public static class VoidGhostLeviathansSpawner_Start_Patch
        {
            [HarmonyPatch(nameof(VoidGhostLeviathansSpawner.Start))]
            [HarmonyPostfix]
            public static void Start_Postfix(VoidGhostLeviathansSpawner __instance)
            {
                GhostLevithanVoidPrefab = __instance.ghostLeviathanPrefab; // Couldn't find the GhostLevithanVoidPrefab in the tech types, luckly it's added to the spawner by default, and we can save that prefab and revert back to it if the player ever want's back the vanilla ghost levithans.
                Update_Values(__instance);
            }
        }

        [HarmonyPatch(typeof(VoidGhostLeviathansSpawner))]
        public static class VoidGhostLeviathansSpawner_Update_Patch
        {
            [HarmonyPatch(nameof(VoidGhostLeviathansSpawner.UpdateSpawn))]
            [HarmonyPostfix]
            public static void UpdateSpawn_Postfix(VoidGhostLeviathansSpawner __instance)
            {
                if (__instance.maxSpawns != SubnauticaVoidTweaksPlugin.options.MaxCreatureCapacity)
                {
                    __instance.playerIsInVoid = false;
                }

                Update_Values(__instance); // Calls the Update_Values function every 2 seconds that make the spawner match the player's settings.
            }
        }
        public static void Update_Values(VoidGhostLeviathansSpawner __instance)
        {
            // Config variables
            SubnauticaVoidTweaksPlugin.Options options = SubnauticaVoidTweaksPlugin.options;

            int MaxSpawnsValue = options.MaxCreatureCapacity;
            float SpawnIntervalValue = options.CreatureSpawnRate;
            float FirstSpawnDelayValue = options.InitialGracePeriod;
            float SpawnDistanceValue = options.CreatureSpawnDistance;
            TechType PrefabType = options.VoidCreaturePrefab;
            float CreatureSize = options.VoidCreatureSize;

            VoidGhostLeviathansSpawner Spawner = __instance as VoidGhostLeviathansSpawner;
            // Update the spawners values.
            Spawner.maxSpawns = MaxSpawnsValue;
            Spawner.spawnInterval = SpawnIntervalValue;
            Spawner.timeBeforeFirstSpawn = FirstSpawnDelayValue;
            Spawner.spawnDistance = SpawnDistanceValue;

            if (PrefabType == TechType.GhostLeviathan || PrefabType == null)
            {
                // The GhostLeviathan prefab is not the same as the one in the void, but for simplicity's sake, if the player chooses the GhostLeviathan prefab, we replace that with the VoidGhostLeviathan prefab from earlier.
                VoidPrefab = GhostLevithanVoidPrefab;
            }
            else
            {
                // Gets the prefab that the player chose.
                UWE.CoroutineHost.StartCoroutine(GetPrefab(PrefabType));
            }

            if (VoidPrefab != null) // When the game finally finds out what prefab we want, and it's not null, we update what the spawners spawn.
            {
                Spawner.ghostLeviathanPrefab = VoidPrefab;
            }

            foreach(GameObject Creature in Spawner.spawnedCreatures)
            {
                if (Creature != null)
                {
                    Creature.transform.localScale = new Vector3(CreatureSize, CreatureSize, CreatureSize); // Updates the Creature scale.

                    if (Creature.GetComponent("VoidCreature") == null) // If we haven't added the VoidCreature component, we add it.
                    {
                        Creature.AddComponent(typeof(VoidCreature));
                    }
                }
                else // If the creature is null, nothing, we remove it from the list of spawned Creatures.
                {
                    Spawner.spawnedCreatures.Remove(Creature);
                    break;
                }
            }
        }

        private static IEnumerator GetPrefab(TechType PrefabType)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(PrefabType);
            yield return task;
            VoidPrefab = task.GetResult();
        }
    }
}
