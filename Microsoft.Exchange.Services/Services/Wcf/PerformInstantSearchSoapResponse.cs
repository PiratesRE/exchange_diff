using System;
using System.ServiceModel;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	public class PerformInstantSearchSoapResponse : BaseSoapResponse
	{
		[MessageBodyMember(Name = "PerformInstantSearchResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Order = 0)]
		public PerformInstantSearchResponse Body;
	}
}
