using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotSetPermissionUnknownEntriesException : ServicePermanentException
	{
		public CannotSetPermissionUnknownEntriesException() : base((CoreResources.IDs)2549623104U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}
	}
}
