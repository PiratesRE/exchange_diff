using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class WellKnownNetworkNames
	{
		static WellKnownNetworkNames()
		{
			WellKnownNetworkNames.WellKnownExternalNetworkName = new HashSet<string>
			{
				WellKnownNetworkNames.Facebook,
				WellKnownNetworkNames.LinkedIn,
				WellKnownNetworkNames.Sharepoint,
				WellKnownNetworkNames.GAL,
				WellKnownNetworkNames.QuickContacts,
				WellKnownNetworkNames.RecipientCache
			};
		}

		internal static bool IsWellKnownExternalNetworkName(string name)
		{
			return WellKnownNetworkNames.WellKnownExternalNetworkName.Contains(name);
		}

		internal static bool IsHiddenSourceNetworkName(string partnerNetworkId, string parentFolderName)
		{
			return StringComparer.OrdinalIgnoreCase.Equals(partnerNetworkId, WellKnownNetworkNames.RecipientCache) || (StringComparer.OrdinalIgnoreCase.Equals(partnerNetworkId, WellKnownNetworkNames.QuickContacts) && StringComparer.OrdinalIgnoreCase.Equals(parentFolderName, "{06967759-274D-40B2-A3EB-D7F9E73727D7}"));
		}

		private static readonly HashSet<string> WellKnownExternalNetworkName;

		public static readonly string Facebook = "Facebook";

		public static readonly string LinkedIn = "LinkedIn";

		public static readonly string Sharepoint = "Sharepoint";

		public static readonly string GAL = "GAL";

		public static readonly string QuickContacts = "QuickContacts";

		public static readonly string RecipientCache = "RecipientCache";

		public static readonly string Outlook = "Outlook";
	}
}
