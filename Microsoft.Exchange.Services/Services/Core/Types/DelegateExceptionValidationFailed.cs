using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DelegateExceptionValidationFailed : ServicePermanentException
	{
		public DelegateExceptionValidationFailed(Exception innerException) : base((CoreResources.IDs)4097108255U, innerException)
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
