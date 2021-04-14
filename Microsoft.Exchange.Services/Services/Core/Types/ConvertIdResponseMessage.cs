using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ConvertIdResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ConvertIdResponseMessage : ResponseMessage
	{
		public ConvertIdResponseMessage()
		{
		}

		internal ConvertIdResponseMessage(ServiceResultCode code, ServiceError error, AlternateIdBase alternateId) : base(code, error)
		{
			this.alternateIdField = alternateId;
		}

		[DataMember(EmitDefaultValue = false)]
		public AlternateIdBase AlternateId
		{
			get
			{
				return this.alternateIdField;
			}
			set
			{
				this.alternateIdField = value;
			}
		}

		private AlternateIdBase alternateIdField;
	}
}
