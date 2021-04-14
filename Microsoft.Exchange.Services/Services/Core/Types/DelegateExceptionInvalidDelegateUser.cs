using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DelegateExceptionInvalidDelegateUser : ServicePermanentException
	{
		public DelegateExceptionInvalidDelegateUser(CoreResources.IDs errorCode) : base(errorCode)
		{
		}

		public DelegateExceptionInvalidDelegateUser(CoreResources.IDs errorCode, Exception innerException) : base(errorCode, innerException)
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
