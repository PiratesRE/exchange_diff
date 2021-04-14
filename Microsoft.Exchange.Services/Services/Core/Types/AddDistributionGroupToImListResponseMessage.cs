using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("AddDistributionGroupToImListResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class AddDistributionGroupToImListResponseMessage : ResponseMessage
	{
		public AddDistributionGroupToImListResponseMessage()
		{
		}

		internal AddDistributionGroupToImListResponseMessage(ServiceResultCode code, ServiceError error, ImGroup result) : base(code, error)
		{
			this.ImGroup = result;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.AddDistributionGroupToImListResponseMessage;
		}

		[DataMember]
		[XmlElement]
		public ImGroup ImGroup { get; set; }
	}
}
