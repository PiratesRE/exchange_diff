using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CommonParameterSets
	{
		private CommonParameterSets()
		{
		}

		public const string Identity = "Identity";

		public const string Default = "Default";
	}
}
