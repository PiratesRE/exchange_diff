using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DelegateExceptionCannotAddOwner : ServicePermanentException
	{
		public DelegateExceptionCannotAddOwner(Exception innerException) : base(CoreResources.IDs.ErrorDelegateCannotAddOwner, innerException)
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
