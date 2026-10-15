namespace CustomFestivalFramework.Framework
{
	internal class CustomCommands
	{
		// TODO event commands
		// conditional switch
		// custom-textured prop

		private static bool registered = false;
		internal static void Register()
		{
			if (registered)
				return;

			registered = true;
		}
	}
}
