using System;
using System.ServiceModel;
using Microsoft.Exchange.InfoWorker.Availability;

namespace Microsoft.Exchange.Services.Wcf
{
	[MessageContract(IsWrapped = false)]
	public class GetUserOofSettingsSoapResponse : BaseSoapResponse
	{
		[MessageBodyMember(Name = "GetUserOofSettingsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", Order = 0)]
		public GetUserOofSettingsResponse Body;
	}
}
