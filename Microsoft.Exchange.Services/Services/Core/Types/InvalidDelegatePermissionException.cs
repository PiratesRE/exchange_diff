using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class InvalidDelegatePermissionException : ServicePermanentException
	{
		public InvalidDelegatePermissionException() : base((CoreResources.IDs)3537364541U)
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
