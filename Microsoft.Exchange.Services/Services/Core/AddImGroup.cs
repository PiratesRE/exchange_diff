using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AddImGroup
	{
		public AddImGroup(IMailboxSession session, string displayName, IXSOFactory xsoFactory, IUnifiedContactStoreConfiguration configuration)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			if (displayName.Length == 0)
			{
				throw new ArgumentException("displayName that was passed in was empty.", "displayName");
			}
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(session, xsoFactory, configuration);
			this.displayName = displayName;
		}

		public RawImGroup Execute()
		{
			return this.unifiedContactStoreUtilities.GetUserImGroupWith(this.displayName);
		}

		private readonly UnifiedContactStoreUtilities unifiedContactStoreUtilities;

		private readonly string displayName;
	}
}
