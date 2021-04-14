using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TooManyMailboxesException : ServicePermanentException
	{
		public TooManyMailboxesException(int mailboxes, int maxAllowedMailboxes) : base(ResponseCodeType.ErrorSearchTooManyMailboxes, CoreResources.ErrorSearchTooManyMailboxes("EDiscoveryError:E010::", mailboxes, maxAllowedMailboxes))
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}
