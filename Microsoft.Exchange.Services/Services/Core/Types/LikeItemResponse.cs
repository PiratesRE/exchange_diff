using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class LikeItemResponse : ResponseMessage
	{
		internal LikeItemResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public LikeItemResponse()
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.LikeItemResponseMessage;
		}
	}
}
