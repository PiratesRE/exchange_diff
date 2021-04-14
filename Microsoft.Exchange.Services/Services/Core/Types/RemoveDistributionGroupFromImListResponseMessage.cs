using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("RemoveDistributionGroupFromImListResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class RemoveDistributionGroupFromImListResponseMessage : ResponseMessage
	{
		public RemoveDistributionGroupFromImListResponseMessage()
		{
		}

		internal RemoveDistributionGroupFromImListResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.RemoveDistributionGroupFromImListResponseMessage;
		}
	}
}
