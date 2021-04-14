using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ServiceArgumentException : ServicePermanentException
	{
		public ServiceArgumentException(Enum messageId) : base(ResponseCodeType.ErrorInvalidArgument, messageId)
		{
		}

		public ServiceArgumentException(Enum messageId, LocalizedString message) : base(ResponseCodeType.ErrorInvalidArgument, message)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}
	}
}
