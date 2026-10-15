using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;

namespace CustomFestivalFramework;

public interface IFestivalAPI
{
	/// <summary>Register a festival handler.</summary>
	/// <param name="id">A unique ID. Should NOT be the festival date.</param>
	/// <param name="handler">A delegate which produces handlers.</param>
	/// <returns>True if successfully registered, false if a handler with that ID already exists.</returns>
	public bool RegisterFestivalHandler(string id, IFestivalProducer producer);

	/// <summary>Unregister a festival handler.</summary>
	/// <param name="id">The unique ID used to register it. Should NOT be the festival date.</param>
	/// <returns>True if successfully removed, false if a handler with that ID does not exist.</returns>
	public bool UnregisterFestivalHandler(string id);

	public interface IFestivalProducer
	{
		/// <summary>Creates a new instance of a festival handler.</summary>
		/// <param name="source">The festival event for which the handler is being created.</param>
		/// <param name="data">The raw festival data.</param>
		public IFestivalHandler Create(Event source, Dictionary<string, string> data);
	}

	/// <summary>
	/// A screen-instanced handler for custom festival effects and mechanics.<br/>
	/// Only Draw, Tick, and Cleanup must be implemented. Other methods are optional.
	/// </summary>
	public interface IFestivalHandler
	{
		/// <summary>Called one time, when a festival is ended or destroyed. Use to cleanup state.</summary>
		public void Cleanup();

		/// <summary>Use for special drawing in world. Called every frame while the world is being drawn.</summary>
		public void Draw(SpriteBatch batch);

		/// <summary>Use for state updates. Called every game loop.</summary>
		public void Tick(GameTime time);

		/// <summary>Use for special drawing above world. Called every frame after the world has been drawn.</summary>
		public void DrawAfterMap(SpriteBatch batch) { }

		/// <summary>Called whenever the player regains control during the festival. Use if you have more than one.</summary>
		/// <param name="name">The ID of the control sequence</param>
		public void SetupControlSequence(string name) { }

		/// <summary>Try to perform an action at this tile</summary>
		/// <returns>True if an action was performed.</returns>
		public bool CheckAction(Point tile) => false;

		/// <summary>Use for setup before the main event begins. Called once, before the main event starts.</summary>
		/// <returns>True if the main event sequence from the festival data should be queued.</returns>
		public bool MainEventSetup() => true;

		/// <summary>Use to provide rewards to the current player. Called when the AwardFestivalPrize command is used.</summary>
		public bool GiveRewards(string[] args) => true;

		/// <summary>Use to handle custom (hardcoded) cutscenes. Called when the Cutscene command is used.</summary>
		/// <returns>True if the cutscene of the given name was handled.</returns>
		public bool TryHandleCutscene(string name) => false;

		/// <summary>Use to handle answers to questions asked during the festival. Called when a question is answered.</summary>
		/// <returns>True if the answer was handled.</returns>
		public bool TryHandleQuestionAnswer(NPC? npc, string answerKey) => false;
	}
}
