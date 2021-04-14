using System;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class AuthZFailureException : ServicePermanentException
	{
		internal AuthZFailureException(AuthzException innerException) : base(CoreResources.IDs.ErrorImpersonationFailed, innerException)
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
