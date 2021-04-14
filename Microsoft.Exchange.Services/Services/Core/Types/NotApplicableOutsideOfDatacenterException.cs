using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class NotApplicableOutsideOfDatacenterException : ServicePermanentException
	{
		public NotApplicableOutsideOfDatacenterException(Enum messageId) : base(ResponseCodeType.ErrorNotApplicableOutsideOfDatacenter, messageId)
		{
		}

		public NotApplicableOutsideOfDatacenterException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorNotApplicableOutsideOfDatacenter, messageId, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2013;
			}
		}
	}
}
