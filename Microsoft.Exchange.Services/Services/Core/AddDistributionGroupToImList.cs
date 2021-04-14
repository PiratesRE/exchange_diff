using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AddDistributionGroupToImList
	{
		public AddDistributionGroupToImList(IMailboxSession session, string smtpAddress, string displayName, IXSOFactory xsoFactory, IUnifiedContactStoreConfiguration configuration)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			this.ThrowIfNullOrEmpty(smtpAddress, "smtpAddress");
			this.ThrowIfNullOrEmpty(displayName, "displayName");
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(session, xsoFactory, configuration);
			this.smtpAddress = smtpAddress;
			this.displayName = displayName;
		}

		public RawImGroup Execute()
		{
			return this.unifiedContactStoreUtilities.GetUserImDGWith(this.smtpAddress, this.displayName);
		}

		private void ThrowIfNullOrEmpty(string parameterValue, string parameterName)
		{
			if (parameterValue == null)
			{
				throw new ArgumentNullException(parameterName);
			}
			if (parameterValue.Length == 0)
			{
				throw new ArgumentException(parameterName + " was empty", parameterName);
			}
		}

		private readonly UnifiedContactStoreUtilities unifiedContactStoreUtilities;

		private readonly string smtpAddress;

		private readonly string displayName;
	}
}
