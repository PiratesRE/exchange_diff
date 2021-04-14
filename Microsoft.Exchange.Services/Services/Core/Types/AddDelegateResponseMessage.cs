using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "AddDelegateResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class AddDelegateResponseMessage : DelegateResponseMessage
	{
		public AddDelegateResponseMessage()
		{
		}

		internal AddDelegateResponseMessage(ServiceResultCode code, ServiceError error, DelegateUserResponseMessageType[] delegateUsers) : base(code, error, delegateUsers, ResponseType.AddDelegateResponseMessage)
		{
		}
	}
}
