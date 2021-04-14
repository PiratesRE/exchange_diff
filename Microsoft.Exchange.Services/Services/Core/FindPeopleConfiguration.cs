using System;
using System.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class FindPeopleConfiguration
	{
		private static int ReadFastSearchTimeoutConfiguration()
		{
			string s = ConfigurationManager.AppSettings["FindPeopleFastSearchTimeoutInMilliseconds"];
			int fastSearchTimeoutConfiguration;
			if (!int.TryParse(s, out fastSearchTimeoutConfiguration))
			{
				fastSearchTimeoutConfiguration = 30000;
			}
			return FindPeopleConfiguration.EnsureFastSearchTimeoutWithinLogicalRange(fastSearchTimeoutConfiguration);
		}

		private static int EnsureFastSearchTimeoutWithinLogicalRange(int fastSearchTimeoutConfiguration)
		{
			if (fastSearchTimeoutConfiguration > Global.SearchTimeoutInMilliseconds)
			{
				return Global.SearchTimeoutInMilliseconds;
			}
			if (fastSearchTimeoutConfiguration < 100)
			{
				return 100;
			}
			return fastSearchTimeoutConfiguration;
		}

		private const int DefaultFastSearchTimeoutInMilliseconds = 30000;

		private const int GraceTimeout = 100;

		public static readonly int MaxRowsDefault = 20;

		public static readonly int MaxQueryStringLength = 255;

		public static readonly LazyMember<int> FastSearchTimeoutInMilliseconds = new LazyMember<int>(new InitializeLazyMember<int>(FindPeopleConfiguration.ReadFastSearchTimeoutConfiguration));
	}
}
