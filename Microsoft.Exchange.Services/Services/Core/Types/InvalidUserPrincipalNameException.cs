using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidUserPrincipalNameException : ServicePermanentException
	{
		public InvalidUserPrincipalNameException() : base(CoreResources.IDs.ErrorInvalidUserPrincipalName)
		{
		}

		public InvalidUserPrincipalNameException(Exception innerException) : base(CoreResources.IDs.ErrorInvalidUserPrincipalName, innerException)
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
