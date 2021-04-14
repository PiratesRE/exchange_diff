using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ComputerNameCharacterConstraint : CharacterRegexConstraint
	{
		public ComputerNameCharacterConstraint() : base("[a-z0-9A-Z\\-]")
		{
		}

		public static ComputerNameCharacterConstraint DefaultConstraint = new ComputerNameCharacterConstraint();
	}
}
