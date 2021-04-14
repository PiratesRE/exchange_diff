using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "AttendeeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "AttendeeType", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class EwsAttendeeType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public EmailAddressWrapper Mailbox { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ResponseType", EmitDefaultValue = false, Order = 2)]
		public string ResponseTypeString
		{
			get
			{
				return this.responseTypeString;
			}
			set
			{
				this.ResponseTypeSpecified = true;
				this.responseTypeString = value;
			}
		}

		[XmlElement("ResponseType")]
		[IgnoreDataMember]
		public ResponseTypeType ResponseType
		{
			get
			{
				if (!this.ResponseTypeSpecified)
				{
					return ResponseTypeType.Unknown;
				}
				return EnumUtilities.Parse<ResponseTypeType>(this.ResponseTypeString);
			}
			set
			{
				this.ResponseTypeString = EnumUtilities.ToString<ResponseTypeType>(value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ResponseTypeSpecified { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public string LastResponseTime
		{
			get
			{
				return this.lastResponseTime;
			}
			set
			{
				this.LastResponseTimeSpecified = true;
				this.lastResponseTime = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool LastResponseTimeSpecified { get; set; }

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string ProposedStart { get; set; }

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string ProposedEnd { get; set; }

		private string responseTypeString;

		private string lastResponseTime;
	}
}
