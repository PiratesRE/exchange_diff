using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidRestrictionException : ServicePermanentException
	{
		public InvalidRestrictionException(Enum messageId) : base(messageId)
		{
		}

		public InvalidRestrictionException(Enum messageId, Exception innerException) : base(messageId, innerException)
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
