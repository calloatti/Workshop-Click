using Calloatti.Config;
using HarmonyLib;
using Timberborn.ModManagerScene;
using UnityEngine;

namespace Calloatti.WorkshopClick
{
  public class WorkshopClickStarter : IModStarter
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
}
