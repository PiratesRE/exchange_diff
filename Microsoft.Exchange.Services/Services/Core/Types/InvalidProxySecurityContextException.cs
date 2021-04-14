using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class InvalidProxySecurityContextException : ServicePermanentException
	{
		public InvalidProxySecurityContextException(Exception innerException) : base((CoreResources.IDs)3616451054U, innerException)
		{
		}

		public InvalidProxySecurityContextException() : base((CoreResources.IDs)3616451054U)
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
