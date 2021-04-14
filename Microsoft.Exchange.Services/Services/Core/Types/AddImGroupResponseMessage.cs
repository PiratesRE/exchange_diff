using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("AddImGroupResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class AddImGroupResponseMessage : ResponseMessage
	{
		public AddImGroupResponseMessage()
		{
		}

		internal AddImGroupResponseMessage(ServiceResultCode code, ServiceError error, ImGroup result) : base(code, error)
		{
			this.ImGroup = result;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.AddImGroupResponseMessage;
		}

		[DataMember]
		[XmlElement]
		public ImGroup ImGroup { get; set; }
	}
}
