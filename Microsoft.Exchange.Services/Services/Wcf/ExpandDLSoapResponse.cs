using System;
using System.ServiceModel;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	public class ExpandDLSoapResponse : BaseSoapResponse
	{
		[MessageBodyMember(Name = "ExpandDLResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Order = 0)]
		public ExpandDLResponse Body;
	}
}
