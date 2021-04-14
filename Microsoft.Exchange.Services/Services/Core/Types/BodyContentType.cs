using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class BodyContentType
	{
		[IgnoreDataMember]
		[XmlAttribute("BodyType")]
		public BodyType BodyType { get; set; }

		[XmlIgnore]
		[DataMember(Name = "BodyType", IsRequired = true)]
		public string BodyTypeString
		{
			get
			{
				return EnumUtilities.ToString<BodyType>(this.BodyType);
			}
			set
			{
				this.BodyType = EnumUtilities.Parse<BodyType>(value);
			}
		}

		[DataMember(IsRequired = true, EmitDefaultValue = false)]
		[XmlText]
		public string Value { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlIgnore]
		public string QuotedText { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsTruncatedSpecified { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = true)]
		[XmlAttribute("IsTruncated")]
		public bool IsTruncated { get; set; }
	}
}
