using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateResponseFromModernGroupRequest : CreateItemRequest
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new CreateResponseFromModernGroup(callContext, this);
		}
	}
}
