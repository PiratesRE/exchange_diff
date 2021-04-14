using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ImpersonateUserDeniedException : ServicePermanentException
	{
		public ImpersonateUserDeniedException() : base(CoreResources.IDs.ErrorImpersonateUserDenied)
		{
		}

		public ImpersonateUserDeniedException(Exception innerException) : base(CoreResources.IDs.ErrorImpersonateUserDenied, innerException)
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
