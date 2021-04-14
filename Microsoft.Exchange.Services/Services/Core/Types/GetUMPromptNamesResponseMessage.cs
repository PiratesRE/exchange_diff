using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetUMPromptNamesResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUMPromptNamesResponseMessage : ResponseMessage
	{
		public GetUMPromptNamesResponseMessage()
		{
		}

		internal GetUMPromptNamesResponseMessage(ServiceResultCode code, ServiceError error, GetUMPromptNamesResponseMessage response) : base(code, error)
		{
			if (response != null)
			{
				this.PromptNames = response.PromptNames;
			}
		}

		[XmlArray(ElementName = "PromptNames", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlArrayItem(ElementName = "String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		public string[] PromptNames { get; set; }

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetUMPromptNamesResponseMessage;
		}
	}
}
