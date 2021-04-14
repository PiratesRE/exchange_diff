using System;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Ex12MultivalueEntryIdStrategyForSyncIssue : Ex12MultivalueEntryIdStrategy
	{
		internal Ex12MultivalueEntryIdStrategyForSyncIssue(StorePropertyDefinition property, LocationEntryIdStrategy.GetLocationPropertyBagDelegate getLocationPropertyBag) : base(property, getLocationPropertyBag, 1)
		{
		}

		internal override byte[] GetEntryId(DefaultFolderContext context)
		{
			if (!Utils.IsTeamMailbox(context.Session))
			{
				return base.GetEntryId(context);
			}
			return null;
		}

		internal override void SetEntryId(DefaultFolderContext context, byte[] entryId)
		{
			if (!Utils.IsTeamMailbox(context.Session))
			{
				base.SetEntryId(context, entryId);
			}
		}
	}
}
