using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class InvalidServerVersionException : ServicePermanentException
	{
		public InvalidServerVersionException() : base(CoreResources.IDs.ErrorInvalidServerVersion)
		{
		}

		public InvalidServerVersionException(Enum messageId) : base(ResponseCodeType.ErrorInvalidServerVersion, messageId)
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
