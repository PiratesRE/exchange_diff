using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "PhoneNumberType")]
	[XmlType(TypeName = "PersonaPhoneNumber", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PhoneNumber
	{
		[IgnoreDataMember]
		[XmlIgnore]
		public PersonPhoneNumberType Type { get; set; }

		[XmlElement]
		[DataMember(IsRequired = true, Order = 1)]
		public string Number { get; set; }

		[DataMember(Name = "Type", IsRequired = true, Order = 2)]
		[XmlElement(ElementName = "Type")]
		public string TypeString
		{
			get
			{
				return this.Type.ToString();
			}
			set
			{
				this.Type = (PersonPhoneNumberType)Enum.Parse(typeof(PersonPhoneNumberType), value);
			}
		}

		public PhoneNumber()
		{
		}

		public PhoneNumber(string number, PersonPhoneNumberType type)
		{
			this.Number = number;
			this.Type = type;
		}

		[XmlIgnore]
		[DataMember(Name = "NormalizedNumber", EmitDefaultValue = false, Order = 3)]
		public string NormalizedNumber
		{
			get
			{
				if (this.normalizedNumberSet)
				{
					return this.normalizedNumber;
				}
				return DtmfString.SanitizePhoneNumber(this.Number);
			}
			set
			{
				this.normalizedNumberSet = true;
				this.normalizedNumber = value;
			}
		}

		private string normalizedNumber;

		private bool normalizedNumberSet;
	}
}
