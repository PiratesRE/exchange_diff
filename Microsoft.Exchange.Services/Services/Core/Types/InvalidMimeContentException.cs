using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidMimeContentException : ServicePermanentException
	{
		public InvalidMimeContentException(Enum messageId) : base(messageId)
		{
		}

		public InvalidMimeContentException(Enum messageId, Exception innerException) : base(messageId, innerException)
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
