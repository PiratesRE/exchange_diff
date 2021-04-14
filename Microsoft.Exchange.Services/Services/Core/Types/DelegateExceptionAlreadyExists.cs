using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DelegateExceptionAlreadyExists : ServicePermanentException
	{
		public DelegateExceptionAlreadyExists(Exception innerException) : base(CoreResources.IDs.ErrorDelegateAlreadyExists, innerException)
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
