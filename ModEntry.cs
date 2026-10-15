using CustomFestivalFramework.Framework;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CustomFestivalFramework;

public class ModEntry : Mod
{
	internal static IMonitor monitor = null!;

	public override void Entry(IModHelper helper)
	{
		monitor = Monitor;
		helper.Events.GameLoop.GameLaunched += Launched;
	}

	private void Launched(object? sender, GameLaunchedEventArgs e)
	{
		var harmony = new Harmony(ModManifest.UniqueID);
		EventPatcher.Patch(new(harmony, Monitor));
	}

	public override object GetApi()
	{
		return API.api;
	}
}
