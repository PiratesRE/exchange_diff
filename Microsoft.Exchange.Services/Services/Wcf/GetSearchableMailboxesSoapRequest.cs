using System;
using System.ServiceModel;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	public class GetSearchableMailboxesSoapRequest : BaseSoapRequest
	{
		[MessageBodyMember(Name = "GetSearchableMailboxes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Order = 0)]
		public GetSearchableMailboxesRequest Body;
	}
}
