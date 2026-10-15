using StardewValley;
using System.Diagnostics.CodeAnalysis;

namespace CustomFestivalFramework.Framework;

internal class API : IFestivalAPI
{
	internal static API api = new();
	internal static bool TryGetHandler(string id, Event ev, Dictionary<string, string> data, [NotNullWhen(true)] out IFestivalAPI.IFestivalHandler? handler)
	{
		handler = null;
		if (!api.handlers.TryGetValue(id, out var factory))
			return false;

		ModEntry.monitor.Log("Initializing handler...");
		handler = factory.Create(ev, data);
		ModEntry.monitor.Log("Handler initialized!");
		return true;
	}

	private readonly Dictionary<string, IFestivalAPI.IFestivalProducer> handlers = new(StringComparer.OrdinalIgnoreCase);

	public bool RegisterFestivalHandler(string id, IFestivalAPI.IFestivalProducer producer)
	{
		if (handlers.ContainsKey(id))
			return false;
		handlers[id] = producer;
		return true;
	}

	public bool UnregisterFestivalHandler(string id)
	{
		return handlers.Remove(id);
	}
}
