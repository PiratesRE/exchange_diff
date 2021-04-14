using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Nspi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class NspiOnlyProperties
	{
		public static readonly NspiPropertyDefinition DisplayType = new NspiPropertyDefinition(PropTag.DisplayType, typeof(LegacyRecipientDisplayType?), "nspiDisplayType", ADPropertyDefinitionFlags.ReadOnly, null, true);

		public static readonly NspiPropertyDefinition DisplayTypeEx = new NspiPropertyDefinition(PropTag.DisplayTypeEx, typeof(RecipientDisplayType?), "nspiDisplayTypeEx", ADPropertyDefinitionFlags.ReadOnly, null, true);
	}
}
