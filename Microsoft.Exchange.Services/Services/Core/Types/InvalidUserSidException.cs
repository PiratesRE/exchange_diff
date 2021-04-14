using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidUserSidException : ServicePermanentException
	{
		public InvalidUserSidException() : base(CoreResources.IDs.ErrorInvalidUserSid)
		{
		}

		public InvalidUserSidException(Exception innerException) : base(CoreResources.IDs.ErrorInvalidUserSid, innerException)
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
