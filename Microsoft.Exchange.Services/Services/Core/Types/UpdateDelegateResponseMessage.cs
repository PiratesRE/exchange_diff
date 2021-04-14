using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UpdateDelegateResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class UpdateDelegateResponseMessage : DelegateResponseMessage
	{
		public UpdateDelegateResponseMessage()
		{
		}

		internal UpdateDelegateResponseMessage(ServiceResultCode code, ServiceError error, DelegateUserResponseMessageType[] delegateUsers) : base(code, error, delegateUsers, ResponseType.UpdateDelegateResponseMessage)
		{
		}
	}
}
