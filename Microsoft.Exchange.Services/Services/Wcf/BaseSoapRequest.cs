using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	[Serializable]
	public abstract class BaseSoapRequest
	{
		[MessageHeader(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public MessageHeader<ExchangeImpersonationType> ExchangeImpersonation;
	}
}
