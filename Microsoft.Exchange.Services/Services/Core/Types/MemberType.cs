using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class MemberType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public EmailAddressWrapper Mailbox { get; set; }

		[XmlElement]
		[IgnoreDataMember]
		public MemberStatusType Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.StatusSpecified = true;
				this.status = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "Status", EmitDefaultValue = false, Order = 2)]
		public string StatusString
		{
			get
			{
				if (!this.StatusSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<MemberStatusType>(this.Status);
			}
			set
			{
				this.Status = EnumUtilities.Parse<MemberStatusType>(value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool StatusSpecified { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 0)]
		[XmlAttribute]
		public string Key { get; set; }

		private MemberStatusType status;
	}
}
