using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DelegateUserResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class DelegateUserResponseMessageType : ResponseMessage
	{
		public DelegateUserResponseMessageType()
		{
		}

		internal DelegateUserResponseMessageType(ServiceResultCode code, ServiceError error, DelegateUserType delegateUser) : base(code, error)
		{
			this.delegateUser = delegateUser;
		}

		[XmlElement("DelegateUser")]
		public DelegateUserType DelegateUser
		{
			get
			{
				return this.delegateUser;
			}
			set
			{
				this.delegateUser = value;
			}
		}

		private DelegateUserType delegateUser;
	}
}
