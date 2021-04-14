using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(ContactType))]
	[XmlInclude(typeof(AddressEntityType))]
	[XmlInclude(typeof(UrlEntityType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(TaskSuggestionType))]
	[KnownType(typeof(UrlEntityType))]
	[XmlInclude(typeof(MeetingSuggestionType))]
	[XmlInclude(typeof(PhoneEntityType))]
	[XmlInclude(typeof(TaskSuggestionType))]
	[KnownType(typeof(AddressEntityType))]
	[KnownType(typeof(ContactType))]
	[KnownType(typeof(PhoneEntityType))]
	[XmlInclude(typeof(EmailAddressEntityType))]
	[KnownType(typeof(EmailAddressEntityType))]
	[KnownType(typeof(MeetingSuggestionType))]
	[Serializable]
	public abstract class BaseEntityType
	{
		[XmlElement]
		[IgnoreDataMember]
		public EmailPositionType Position { get; set; }

		[XmlIgnore]
		[DataMember(Name = "Position", EmitDefaultValue = false)]
		public string PositionString
		{
			get
			{
				return EnumUtilities.ToString<EmailPositionType>(this.Position);
			}
			set
			{
				throw new NotImplementedException("Position setter");
			}
		}
	}
}
