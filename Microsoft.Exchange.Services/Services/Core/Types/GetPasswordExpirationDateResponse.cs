using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "GetPasswordExpirationDateResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetPasswordExpirationDateResponse : ResponseMessage
	{
		public GetPasswordExpirationDateResponse()
		{
		}

		internal GetPasswordExpirationDateResponse(ServiceResultCode code, ServiceError error, DateTime passwordExpirationDate) : base(code, error)
		{
			this.PasswordExpirationDate = passwordExpirationDate;
		}

		[IgnoreDataMember]
		public DateTime PasswordExpirationDate { get; set; }

		[DataMember(Name = "PasswordExpirationDate", IsRequired = true)]
		[XmlIgnore]
		public string PasswordExpirationDateString
		{
			get
			{
				return this.PasswordExpirationDate.ToString();
			}
			set
			{
				this.PasswordExpirationDate = DateTime.Parse(value);
			}
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetPasswordExpirationDateResponseMessage;
		}
	}
}
