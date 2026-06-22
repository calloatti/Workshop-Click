using Bindito.Core;
using Calloatti.Config;
using HarmonyLib;
using Timberborn.AssetSystem;
using Timberborn.ModManagerScene;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Calloatti.WorkshopClick
{
  public partial class WorkshopClickStarter : IModStarter
  {
    public static SimpleConfig Config { get; private set; }

    public void StartMod(IModEnvironment modEnvironment)
    {
      Config = new SimpleConfig(modEnvironment.ModPath);

      var harmony = new Harmony("com.calloatti.workshopclick");
      harmony.PatchAll();

      Debug.Log("[Calloatti.WorkshopClick] Core Initialized.");
    }
  }

  [Context("MainMenu")]
  public class WorkshopClickConfigurator : Configurator
  {
    protected override void Configure()
    {
      Bind<WorkshopClickAssetCapturer>().AsSingleton();
    }
  }

  public class WorkshopClickAssetCapturer : ILoadableSingleton
  {
    public static IAssetLoader AssetLoader { get; private set; }
    public WorkshopClickAssetCapturer(IAssetLoader assetLoader) { AssetLoader = assetLoader; }
    public void Load() { }
  }
}