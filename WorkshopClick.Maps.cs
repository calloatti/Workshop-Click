using HarmonyLib;
using Steamworks;
using Timberborn.MapItemsUI;
using Timberborn.MapRepositorySystemUI;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

namespace Calloatti.WorkshopClick
{
  // Patching the Bind method because MapItem is a plain data class
  [HarmonyPatch(typeof(MapItemElementFactory), nameof(MapItemElementFactory.Bind))]
  internal static class MapItemElementFactoryPatch
  {
    private static void Postfix(VisualElement item, MapItem mapItem)
    {
      Image mapIcon = item.Q<Image>("Icon"); // Found in MapItemElement.uxml 
      if (mapIcon == null) return;

      // Parse the path to see if it resides in a Steam Workshop folder
      string path = mapItem.MapFileReference.Path;
      if (string.IsNullOrEmpty(path)) return;

      string folderName = Path.GetFileName(Path.GetDirectoryName(path));

      if (ulong.TryParse(folderName, out ulong steamId))
      {
        mapIcon.pickingMode = PickingMode.Position;
        Sprite hoverSprite = null;

        mapIcon.RegisterCallback<PointerEnterEvent>(evt =>
        {
          if (hoverSprite == null && WorkshopClickAssetCapturer.AssetLoader != null)
            hoverSprite = WorkshopClickAssetCapturer.AssetLoader.Load<Sprite>("Resources/UI/Images/Core/cloud-file-icon-hover");

          if (hoverSprite != null) mapIcon.style.backgroundImage = new StyleBackground(hoverSprite);
        });

        mapIcon.RegisterCallback<PointerLeaveEvent>(evt =>
        {
          mapIcon.style.backgroundImage = new StyleBackground(StyleKeyword.Null);
        });

        mapIcon.RegisterCallback<PointerDownEvent>(evt =>
        {
          if (evt.button == 0)
          {
            string url = $"https://steamcommunity.com/sharedfiles/filedetails/?id={steamId}";
            if (WorkshopClickStarter.Config.GetBool("OpenInDefaultBrowser", false)) Application.OpenURL(url);
            else SteamFriends.ActivateGameOverlayToWebPage(url);
            evt.StopPropagation();
          }
        }, TrickleDown.TrickleDown);
      }
    }
  }

  // Inject the Hint Label into the Map Selection screen
  [HarmonyPatch(typeof(MapSelection), "Initialize")]
  internal static class MapSelectionHintPatch
  {
    private static void Postfix(VisualElement root)
    {
      ListView mapList = root.Q<ListView>("MapList");
      if (mapList == null) return;

      Label infoLabel = new Label("💡 Hint: Left-click a Workshop map's icon to open its Steam page.");
      infoLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
      infoLabel.style.color = new StyleColor(new Color(0.85f, 0.75f, 0.55f));
      infoLabel.style.marginBottom = 10;
      infoLabel.style.marginTop = 5;
      infoLabel.style.fontSize = 14;

      mapList.parent.Insert(mapList.parent.IndexOf(mapList), infoLabel);
    }
  }
}