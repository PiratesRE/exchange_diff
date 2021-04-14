using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class RequestDeviceRegistrationChallengeResponseMessage : ResponseMessage
	{
		public RequestDeviceRegistrationChallengeResponseMessage()
		{
		}

		internal RequestDeviceRegistrationChallengeResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.RequestDeviceRegistrationChallengeResponseMessage;
		}
	}
}
