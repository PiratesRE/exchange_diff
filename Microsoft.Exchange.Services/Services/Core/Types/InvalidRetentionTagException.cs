using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidRetentionTagException : ServicePermanentException
	{
		public InvalidRetentionTagException(Enum messageId) : base(messageId)
		{
		}

		public InvalidRetentionTagException(Enum messageId, Exception innerException) : base(messageId, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}
