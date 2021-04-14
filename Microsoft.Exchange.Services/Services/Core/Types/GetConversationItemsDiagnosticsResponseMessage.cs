using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetConversationItemsDiagnosticsResponseMessage : ResponseMessage
	{
		public GetConversationItemsDiagnosticsResponseMessage()
		{
		}

		internal GetConversationItemsDiagnosticsResponseMessage(ServiceResultCode code, ServiceError error, GetConversationItemsDiagnosticsResponseType diagnostics) : base(code, error)
		{
			this.Diagnostics = diagnostics;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetConversationItemsDiagnosticsResponseMessage;
		}

		[DataMember(EmitDefaultValue = false)]
		public GetConversationItemsDiagnosticsResponseType Diagnostics { get; set; }
	}
}
