using System;
using System.ServiceModel;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	public class AddImGroupSoapResponse : BaseSoapResponse
	{
		[MessageBodyMember(Name = "AddImGroupResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Order = 0)]
		public AddImGroupResponseMessage Body;
	}
}
