using HarmonyLib;
using StardewValley;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StarModGen.Utils;

namespace CustomFestivalFramework.Framework;

internal class EventPatcher
{
	// TODO patches
	// CMD loadactors transpiler: add fallback. explicit exclusion w/ -1000 -1000
	// CMD TemporaryAnimatedSprite transpiler: add underwater flag

	public static void Patch(HarmonyHelper harmony)
	{
		harmony
			.With(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.Cutscene)).Transpiler(InjectCutscene)
			.With(nameof(Event.DefaultCommands.AwardFestivalPrize)).Prefix(HandleReward)
			.With<Event>(nameof(Event.answerDialogueQuestion)).Prefix(HandleAnswer)
			.With(nameof(Event.forceFestivalContinue)).Prefix(HandleFestivalContinue)
			.With(nameof(Event.checkAction)).Postfix(HandleAction)
			.With(nameof(Event.setUpPlayerControlSequence)).Postfix(HandleSetup)
			.With(nameof(Event.draw)).Postfix(HandleDraw)
			.With(nameof(Event.drawAfterMap)).Postfix(HandleDrawAfter)
			.With(nameof(Event.festivalUpdate)).Postfix(HandleUpdate)
			.With(nameof(Event.exitEvent)).Postfix(HandleEnd);
	}

	private static IEnumerable<CodeInstruction> InjectCutscene(IEnumerable<CodeInstruction> source, ILGenerator gen)
	{
		var cursor = new CodeMatcher(source, gen);
		cursor
			.MatchEndForward(new CodeMatch(OpCodes.Ldloc_0))
			.Advance(1)
			.InsertAndAdvance(
					new(OpCodes.Call, typeof(Manager).GetMethod(nameof(Manager.HandleCutscene), BindingFlags.Static | BindingFlags.NonPublic)),
					new(OpCodes.Ldloc_0)
			)
			.Advance(-1)
			.InsertBranchAndAdvance(OpCodes.Brfalse, cursor.Pos)
			.InsertAndAdvance(new CodeInstruction(OpCodes.Ret));

		return cursor.InstructionEnumeration();
	}

	private static bool HandleAnswer(string answerKey, NPC who)
		=> !Manager.AnswerQuestion(answerKey, who);

	private static bool HandleAction(bool result, xTile.Dimensions.Location tileLocation)
		=> Manager.CheckAction(new(tileLocation.X, tileLocation.Y)) || result;

	private static void HandleSetup(Event __instance, string id, ref NPC ___festivalHost,
		ref string ___hostMessageKey, Dictionary<string, string> ___festivalData)
	{
		if (!__instance.isFestival)
			return;

		if (Manager.Handler is null)
			Manager.InitHandler(__instance, ___festivalData, ref ___festivalHost, ref ___hostMessageKey);
		Manager.DoControlSequence(id);
	}

	private static void HandleDraw(SpriteBatch b)
		=> Manager.Handler?.Draw(b);

	private static void HandleDrawAfter(SpriteBatch b)
		=> Manager.Handler?.DrawAfterMap(b);

	private static void HandleUpdate(GameTime time)
		=> Manager.Handler?.Tick(time);

	private static void HandleEnd()
		=> Manager.End();

	private static bool HandleReward(string[] args)
		=> Manager.DoRewards(args[1..]);

	private static bool HandleFestivalContinue()
		=> Manager.ForceContinue();
}
