using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetClientExtensionResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetClientExtensionResponse : ResponseMessage
	{
		public GetClientExtensionResponse()
		{
		}

		internal GetClientExtensionResponse(ServiceResultCode code, ServiceError error, GetClientExtensionResponse getClientExtensionResponse) : base(code, error)
		{
			if (getClientExtensionResponse != null)
			{
				this.ClientExtensions = getClientExtensionResponse.ClientExtensions;
				if (!string.IsNullOrEmpty(getClientExtensionResponse.RawMasterTableXml))
				{
					this.RawMasterTableXml = getClientExtensionResponse.RawMasterTableXml;
				}
			}
		}

		[XmlArrayItem("ClientExtension", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ClientExtension[] ClientExtensions { get; set; }

		[DataMember(Name = "RawMasterTableXml", IsRequired = false)]
		[XmlElement("RawMasterTableXml")]
		public string RawMasterTableXml { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetClientExtensionResponseMessage;
		}
	}
}
