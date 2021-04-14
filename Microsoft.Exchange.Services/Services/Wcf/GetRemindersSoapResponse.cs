using System;
using System.ServiceModel;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	public class GetRemindersSoapResponse : BaseSoapResponse
	{
		[MessageBodyMember(Name = "GetRemindersResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Order = 0)]
		public GetRemindersResponse Body;
	}
}
