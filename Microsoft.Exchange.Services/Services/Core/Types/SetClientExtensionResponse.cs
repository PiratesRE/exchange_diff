using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SetClientExtensionResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SetClientExtensionResponse : BaseResponseMessage
	{
		public SetClientExtensionResponse() : base(ResponseType.SetClientExtensionResponseMessage)
		{
		}
	}
}
