using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public abstract class DelegateResponseMessage : ResponseMessage
	{
		public DelegateResponseMessage()
		{
		}

		internal DelegateResponseMessage(ServiceResultCode code, ServiceError error, DelegateUserResponseMessageType[] delegateUsers, ResponseType responseType) : base(code, error)
		{
			this.delegateUsers = delegateUsers;
			this.responseType = responseType;
		}

		[XmlArrayItem("DelegateUserResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", IsNullable = false)]
		public DelegateUserResponseMessageType[] ResponseMessages
		{
			get
			{
				return this.delegateUsers;
			}
			set
			{
				this.delegateUsers = value;
			}
		}

		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces Namespaces
		{
			get
			{
				return ResponseMessage.namespaces;
			}
			set
			{
			}
		}

		public override ResponseType GetResponseType()
		{
			return this.responseType;
		}

		private ResponseType responseType;

		private DelegateUserResponseMessageType[] delegateUsers;
	}
}
