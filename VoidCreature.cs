using SubnuaticaVoidTweaksMod;
using System;
using UnityEngine;

namespace SubnuaticaVoidTweaksMod
{
    // TODO Review this file and update to your own requirements, or remove it altogether if not required
    /// <summary>
    /// Template MonoBehaviour class. Use this to add new functionality and behaviours to
    /// the game.
    /// </summary>
    public class VoidCreature : MonoBehaviour
    {
        private void Awake() {

            bool VoidCreatureDeletion = SubnauticaVoidTweaksPlugin.options.RemoveVoidCreatures;

            if (VoidCreatureDeletion)
            {
                KillCrature(gameObject);
                UnityEngine.Object.Destroy(gameObject);
            }

            LargeWorldEntity entity = gameObject.GetComponent<LargeWorldEntity>();
            entity.cellLevel = LargeWorldEntity.CellLevel.Global;
            entity.initialCellLevel = LargeWorldEntity.CellLevel.Global;
        }
        private void KillCrature(GameObject gameObject)
        {
            Creature creature = gameObject.GetComponent<Creature>();
            if (creature != null)
            {
                LiveMixin liveMixin = creature.liveMixin;
                if (liveMixin != null)
                {
                    liveMixin.Kill();
                }
            }
        }
        public void Update()
        {
            bool flag = IsVoidBiome(Player.main.GetBiomeString());
            if (!flag) // Checks if we're outside the void.
            {
                bool VoidCreatureDeletion = SubnauticaVoidTweaksPlugin.options.RetreatWhenLeave;
                if (VoidCreatureDeletion) // Checks if we have enabled the option that deletes creatures if we leave the void.
                {
                    KillCrature(gameObject);
                    UnityEngine.Object.Destroy(gameObject);

                    return;
                }
            }

            if ((transform.position - Player.main.transform.position).magnitude >= 151f || SubnauticaVoidTweaksPlugin.options.RemoveVoidCreatures) // Remove Creatures that are really far away, as that causes issues.
            {
                KillCrature(gameObject);
                UnityEngine.Object.Destroy(gameObject);

                return;
            }
        }

        private bool IsVoidBiome(string biomeName) // Returns true or false depending if the input biome is the void.
        {
            return string.Equals(biomeName, "void", StringComparison.OrdinalIgnoreCase);
        }
    }
}
