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

    // Workstation module indices from TileEntityWorkstation.Module enum:
    //   0 = Tools, 1 = Input, 2 = Output, 3 = Fuel, 4 = Material_Input (forge-style smelting)
    // Only fueled workstations (module 3) actually burn resources when idle,
    // so the patch short-circuits for anything else.
    private const int ModuleFuel = 3;

    [HarmonyPatch(typeof(TileEntityWorkstation), nameof(TileEntityWorkstation.UpdateTick))]
    private static class Patch_TileEntityWorkstation_UpdateTick
    {
        private static void Postfix(TileEntityWorkstation __instance)
        {
            if (!__instance.isBurning) return;
            if (__instance.isModuleUsed == null) return;
            if (!__instance.isModuleUsed[ModuleFuel]) return;

            // Don't touch state while the player has the UI open. When a recipe is
            // queued, fuelWindow.TurnOn() flips IsBurning BEFORE syncTEfromUI() copies
            // the UI queue into TE.queue, so there's a window where isBurning=true
            // and hasRecipeInQueue()=false. syncTEfromUI doesn't re-sync isBurning,
            // so a false-flip here sticks past the UI close and stalls the recipe.
            if (__instance.IsUserAccessing()) return;

            if (__instance.hasRecipeInQueue()) return;

            // input[0..2] are the user-facing input slots (campfire cooking slots /
            // forge smelt input). Material stockpile slots come after at
            // [3..3+materialNames.Length-1]. If anything is staged in the input
            // slots, leave the fire on — applies to campfire and forge alike.
            if (__instance.input != null)
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
