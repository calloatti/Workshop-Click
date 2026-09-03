using HarmonyLib;
using Newtonsoft.Json.Linq;
using Steamworks;
using System.IO;
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
      if (modIcon == null) return;

      string url = null;
      bool isLocal = __instance.Mod.ModDirectory.IsUserMod; //

      if (!isLocal)
      {
        // Workshop Mod Logic
        string steamId = __instance.Mod.ModDirectory.OriginName; //
        if (ulong.TryParse(steamId, out _))
        {
          url = $"https://steamcommunity.com/sharedfiles/filedetails/?id={steamId}";
        }
      }
      else
      {
        // Local Mod Logic: search for mod.io ResourceId
        string path = __instance.Mod.ModDirectory.OriginPath; //
        string manifestPath = Path.Combine(path, "mod_manager_manifest.json");
        if (File.Exists(manifestPath))
        {
          try
          {
            string content = File.ReadAllText(manifestPath);
            JObject json = JObject.Parse(content);
            if (json.TryGetValue("ResourceId", out JToken token) && token.Type != JTokenType.Null)
            {
              string resourceId = token.ToString();
              if (!string.IsNullOrEmpty(resourceId))
              {
                url = "https://mod.io/search/mods/" + resourceId;
              }
            }
          }
          catch { }
        }
      }

      // If no valid URL was found (Steam ID or mod.io ResourceId), we do nothing
      if (string.IsNullOrEmpty(url)) return;

      // Use specific classes and hover sprites based on mod type
      string iconClass = isLocal ? "mod-item__icon--local" : "mod-item__icon--cloud";
      string hoverSpritePath = isLocal ? "Resources/UI/Images/Core/local-file-icon-hover" : "Resources/UI/Images/Core/cloud-file-icon-hover";

      modIcon.pickingMode = PickingMode.Position;
      Sprite hoverSprite = null;

      modIcon.RegisterCallback<PointerEnterEvent>(evt =>
      {
        modIcon.RemoveFromClassList(iconClass);
        if (hoverSprite == null && WorkshopClickAssetCapturer.AssetLoader != null)
          hoverSprite = WorkshopClickAssetCapturer.AssetLoader.Load<Sprite>(hoverSpritePath);

        if (hoverSprite != null) modIcon.style.backgroundImage = new StyleBackground(hoverSprite);
      });

      modIcon.RegisterCallback<PointerLeaveEvent>(evt =>
      {
        modIcon.style.backgroundImage = new StyleBackground(StyleKeyword.Null);
        modIcon.AddToClassList(iconClass);
      });

      modIcon.RegisterCallback<PointerDownEvent>(evt =>
      {
        if (evt.button == 0)
        {
          if (WorkshopClickStarter.Config.GetBool("OpenInDefaultBrowser"))
          {
            Application.OpenURL(url);
          }
          else
          {
            SteamFriends.ActivateGameOverlayToWebPage(url);
          }
          evt.StopPropagation();
        }
      }, TrickleDown.TrickleDown);
    }
  }
}