using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.InfoWorker.Availability;

namespace Microsoft.Exchange.Services.Wcf
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetUserOofSettingsJsonResponse : BaseJsonResponse
	{
		[DataMember(IsRequired = true, Order = 0)]
		public SetUserOofSettingsResponse Body;
	}
}
