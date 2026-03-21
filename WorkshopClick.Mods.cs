using HarmonyLib;
using Steamworks;
using Timberborn.ModdingUI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calloatti.WorkshopClick
{
  [HarmonyPatch(typeof(ModItem), nameof(ModItem.Initialize))]
  internal static class ModItemInitializePatch
  {
    private static void Postfix(ModItem __instance)
    {
      Image modIcon = __instance.Root.Q<Image>("ModIcon");
      if (modIcon == null || __instance.Mod.ModDirectory.IsUserMod) return;

      modIcon.pickingMode = PickingMode.Position;
      Sprite hoverSprite = null;

      modIcon.RegisterCallback<PointerEnterEvent>(evt =>
      {
        modIcon.RemoveFromClassList("mod-item__icon--cloud");
        if (hoverSprite == null && WorkshopClickAssetCapturer.AssetLoader != null)
          hoverSprite = WorkshopClickAssetCapturer.AssetLoader.Load<Sprite>("Resources/UI/Images/Core/cloud-file-icon-hover");

        if (hoverSprite != null) modIcon.style.backgroundImage = new StyleBackground(hoverSprite);
      });

      modIcon.RegisterCallback<PointerLeaveEvent>(evt =>
      {
        modIcon.style.backgroundImage = new StyleBackground(StyleKeyword.Null);
        modIcon.AddToClassList("mod-item__icon--cloud");
      });

      modIcon.RegisterCallback<PointerDownEvent>(evt =>
      {
        if (evt.button == 0)
        {
          string steamId = __instance.Mod.ModDirectory.OriginName;
          if (ulong.TryParse(steamId, out _))
          {
            string url = $"https://steamcommunity.com/sharedfiles/filedetails/?id={steamId}";
            if (WorkshopClickStarter.Config.GetBool("OpenInDefaultBrowser", false)) Application.OpenURL(url);
            else SteamFriends.ActivateGameOverlayToWebPage(url);
            evt.StopPropagation();
          }
        }
      }, TrickleDown.TrickleDown);
    }
  }
}