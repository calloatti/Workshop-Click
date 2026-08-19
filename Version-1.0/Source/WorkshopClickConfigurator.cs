using Bindito.Core;
using Timberborn.AssetSystem;
using Timberborn.SingletonSystem;

namespace Calloatti.WorkshopClick
{
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
