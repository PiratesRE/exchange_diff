using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SetImListMigrationCompletedResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SetImListMigrationCompletedResponseMessage : ResponseMessage
	{
		public SetImListMigrationCompletedResponseMessage()
		{
		}

		internal SetImListMigrationCompletedResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SetImListMigrationCompletedResponseMessage;
		}
	}
}
