using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using Microsoft.Xna.Framework;

namespace CustomFestivalFramework.Framework
{
	internal class Manager
	{
		public static bool Active => activeHandler.Value is not null;
		public static IFestivalAPI.IFestivalHandler? Handler => activeHandler.Value;

		internal static readonly PerScreen<IFestivalAPI.IFestivalHandler?> activeHandler = new();

		internal static void InitHandler(Event source, Dictionary<string, string> data, ref NPC festivalHost, ref string hostMessage)
		{
			activeHandler.Value = null;
			if (!data.TryGetValue("ID", out var id))
				return;

			string? host;

			if (!API.TryGetHandler(id, source, data, out var handler))
			{
				if (!data.TryGetValue("Host", out host))
				{
					ModEntry.monitor.Log($"No handler registered for festival ID '{id}', this festival will be ignored by CFF.", LogLevel.Info);
					return;
				}
				ModEntry.monitor.Log($"No handler registered for festival ID '{id}', using default behavior.", LogLevel.Debug);
			} 
			else
			{
				data.TryGetValue("Host", out host);

				ModEntry.monitor.Log($"Found handler registered for festival ID '{id}'.");
				ModEntry.monitor.Log($"Instantiated handler of type '{handler.GetType().AssemblyQualifiedName}'.");
			}

			activeHandler.Value = handler;

			AddHost(source, data, host, ref festivalHost, ref hostMessage);
		}

		internal static void DoControlSequence(string id)
			=> Handler?.SetupControlSequence(id);

		private static void AddHost(Event source, Dictionary<string, string> data, string? hostName, ref NPC host, ref string hostMessage)
		{
			if (!data.TryGetValue("HostQuestionKey", out var hostq))
			{
				ModEntry.monitor.Log("No host prompt provided! There's no way to trigger the main event!", LogLevel.Warn);
				return;
			}

			if (source.getActorByName(hostName) is NPC npc)
				host = npc;

			hostMessage = hostq;
		}

		internal static bool HandleCutscene(string? which)
			=> which != null && Handler is IFestivalAPI.IFestivalHandler handler && handler.TryHandleCutscene(which);

		internal static bool CheckAction(Point tile)
			=> Handler is IFestivalAPI.IFestivalHandler handler && handler.CheckAction(tile);

		internal static bool AnswerQuestion(string answer, NPC npc)
			=> Handler is IFestivalAPI.IFestivalHandler handler && handler.TryHandleQuestionAnswer(npc, answer);

		internal static bool ForceContinue()
		{
			if (Handler is null)
				return true;

			if (!Handler.MainEventSetup())
			{
				if (Game1.IsServer)
					Game1.Multiplayer.sendServerToClientsMessage("festivalEvent");

				return false;
			}

			return true;
		}
			

		internal static void End()
		{
			Handler?.Cleanup();
			activeHandler.Value = null;
		}

		internal static void DoRewards(string[] args)
			=> Handler?.GiveRewards(args);
	}
}
