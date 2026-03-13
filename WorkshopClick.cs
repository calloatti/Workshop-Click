using HarmonyLib;
using Steamworks;
using Timberborn.ModdingUI;
using Timberborn.ModManagerScene;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calloatti.WorkshopClick
{
  public class WorkshopClickStarter : IModStarter
  {
    public void StartMod(IModEnvironment modEnvironment)
    {
      var harmony = new Harmony("com.calloatti.workshopclick");
      harmony.PatchAll();
      Debug.Log("[Calloatti.WorkshopClick] Harmony patches applied successfully.");
    }
  }

  // --- PATCH 1: THE CLICK ACTION ---
  [HarmonyPatch(typeof(ModItem), nameof(ModItem.Initialize))]
  internal static class ModItemInitializePatch
  {
    private static void Postfix(ModItem __instance)
    {
      Label nameLabel = __instance.Root.Q<Label>("ModName");
      if (nameLabel == null) return;

      // Only apply the click event if it is a Cloud/Workshop mod 
      if (!__instance.Mod.ModDirectory.IsUserMod)
      {
        nameLabel.RegisterCallback<PointerDownEvent>(evt =>
        {
          // Trigger only on Left Click (button 0)
          if (evt.button == 0)
          {
            string steamId = __instance.Mod.ModDirectory.OriginName;

            if (ulong.TryParse(steamId, out _))
            {
              string workshopUrl = $"https://steamcommunity.com/sharedfiles/filedetails/?id={steamId}";
              SteamFriends.ActivateGameOverlayToWebPage(workshopUrl);

              // Stop the click from propagating
              evt.StopPropagation();
            }
          }
        }, TrickleDown.TrickleDown);
      }
    }
  }

  // --- PATCH 2: THE GLOBAL HINT LABEL ---
  [HarmonyPatch(typeof(ModListView), nameof(ModListView.Initialize))]
  internal static class ModListViewInitializePatch
  {
    private static void Postfix(VisualElement root)
    {
      // Find the scroll view so we know where to inject our text 
      ScrollView scrollView = root.Q<ScrollView>();
      if (scrollView == null) return;

      // Create our new info label
      Label infoLabel = new Label("💡 Hint: Left-click a Workshop mod's name to open its Steam page.");

      // Add some basic styling so it fits the Timberborn aesthetic
      infoLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
      infoLabel.style.color = new StyleColor(new Color(0.85f, 0.75f, 0.55f)); // A nice timber-friendly gold/tan
      infoLabel.style.marginBottom = 10;
      infoLabel.style.marginTop = 5;
      infoLabel.style.fontSize = 14;

      // Insert it into the parent container, exactly before the scroll view 
      scrollView.parent.Insert(scrollView.parent.IndexOf(scrollView), infoLabel);
    }
  }
}