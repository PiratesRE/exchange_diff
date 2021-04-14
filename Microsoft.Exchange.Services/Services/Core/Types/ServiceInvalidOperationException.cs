using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class ServiceInvalidOperationException : ServicePermanentException
	{
		public ServiceInvalidOperationException(Enum messageId) : base(ResponseCodeType.ErrorInvalidOperation, messageId)
		{
		}

		public ServiceInvalidOperationException(LocalizedString message, Exception innerException) : base(ResponseCodeType.ErrorInvalidOperation, message, innerException)
		{
		}

		public ServiceInvalidOperationException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorInvalidOperation, messageId, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}
	}
}
