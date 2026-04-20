using HarmonyLib;
using System.Reflection;

public class KitsuneFuelSaver : IModApi
{
    public void InitMod(Mod mod)
    {
        Log.Out("[KitsuneFuelSaver] Loading Harmony patches");
        Harmony harmony = new Harmony(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    // Workstation module indices from TileEntityWorkstation.
    //   0 = Tools, 1 = ?, 2 = ?, 3 = Fuel, 4 = MaterialInput (forge-style smelting)
    // Only fueled workstations (module 3) actually burn resources when idle,
    // so the patch short-circuits for anything else.
    private const int ModuleFuel = 3;
    private const int ModuleMaterialInput = 4;

    [HarmonyPatch(typeof(TileEntityWorkstation), nameof(TileEntityWorkstation.UpdateTick))]
    private static class Patch_TileEntityWorkstation_UpdateTick
    {
        private static void Postfix(TileEntityWorkstation __instance)
        {
            if (!__instance.isBurning) return;
            if (__instance.isModuleUsed == null) return;
            if (!__instance.isModuleUsed[ModuleFuel]) return;

            if (__instance.hasRecipeInQueue()) return;

            if (__instance.isModuleUsed[ModuleMaterialInput])
            {
                int materialCount = __instance.materialNames != null ? __instance.materialNames.Length : 0;
                int rawSlotEnd = __instance.input.Length - materialCount;
                for (int i = 0; i < rawSlotEnd; i++)
                {
                    if (!__instance.input[i].IsEmpty()) return;
                }
            }

            __instance.IsBurning = false;
        }
    }
}
