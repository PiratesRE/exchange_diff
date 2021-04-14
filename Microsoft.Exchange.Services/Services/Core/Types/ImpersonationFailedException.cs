using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ImpersonationFailedException : ServicePermanentException
	{
		internal ImpersonationFailedException(Exception innerException) : base(CoreResources.IDs.ErrorImpersonationFailed, innerException)
		{
		}

		internal ImpersonationFailedException() : base(CoreResources.IDs.ErrorImpersonationFailed)
		{
		}

		internal ImpersonationFailedException(Enum messageId, Exception innerException) : base(messageId, innerException)
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
