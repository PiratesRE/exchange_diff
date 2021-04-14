using System;
using System.ServiceModel;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	public class PerformInstantSearchSoapRequest : BaseSoapRequest
	{
		[MessageBodyMember(Name = "PerformInstantSearch", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Order = 0)]
		public PerformInstantSearchRequest Body;
	}
}
