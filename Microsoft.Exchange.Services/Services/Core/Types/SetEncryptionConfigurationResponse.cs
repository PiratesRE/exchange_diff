using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SetEncryptionConfigurationResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SetEncryptionConfigurationResponse : ResponseMessage
	{
		public SetEncryptionConfigurationResponse()
		{
		}

		internal SetEncryptionConfigurationResponse(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SetEncryptionConfigurationResponseMessage;
		}
	}
}
