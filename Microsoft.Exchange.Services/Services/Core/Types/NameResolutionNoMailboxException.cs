using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class NameResolutionNoMailboxException : ServicePermanentException
	{
		public NameResolutionNoMailboxException() : base(CoreResources.IDs.ErrorNameResolutionNoMailbox)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}
