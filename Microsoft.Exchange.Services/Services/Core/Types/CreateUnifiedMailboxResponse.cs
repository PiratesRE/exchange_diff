using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("CreateUnifiedMailboxResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class CreateUnifiedMailboxResponse : ResponseMessage
	{
		public CreateUnifiedMailboxResponse()
		{
		}

		internal CreateUnifiedMailboxResponse(ServiceResultCode code, ServiceError error, string userPrincipalName) : base(code, error)
		{
			this.UserPrincipalName = userPrincipalName;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.CreateUnifiedMailboxResponseMessage;
		}

		[XmlElement("UserPrincipalName", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string UserPrincipalName { get; set; }
	}
}
