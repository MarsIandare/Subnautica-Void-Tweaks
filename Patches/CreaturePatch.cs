using HarmonyLib;
using System;
using UnityEngine;

namespace SubnuaticaVoidTweaksMod.Patches
{
    [HarmonyPatch(typeof(Creature))]
    internal class CreaturePatch
    {
        /// <param name="__instance"></param>
        
        [HarmonyPatch(nameof(Creature.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(Creature __instance)
        {
            float distance = (__instance.transform.position - Player.main.transform.position).magnitude;

            if (distance <= 152f && IsVoidBiome(Player.main.biomeString) && !__instance.GetComponent<VoidCreature>())
            {
                UnityEngine.Object.Destroy(__instance.gameObject);

                return;
            }
        }
        private static bool IsVoidBiome(string biomeName) // Returns true or false depending if the input biome is the void.
        {
            return string.Equals(biomeName, "void", StringComparison.OrdinalIgnoreCase);
        }
    }
}