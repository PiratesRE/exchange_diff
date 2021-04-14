using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class EntityExtractionResultType
	{
		[XmlArrayItem(ElementName = "AddressEntity", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(AddressEntityType))]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public AddressEntityType[] Addresses { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlArrayItem(ElementName = "MeetingSuggestion", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(MeetingSuggestionType))]
		public MeetingSuggestionType[] MeetingSuggestions { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlArrayItem(ElementName = "TaskSuggestion", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(TaskSuggestionType))]
		public TaskSuggestionType[] TaskSuggestions { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlArrayItem(ElementName = "BusinessName", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		public string[] BusinessNames { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlArrayItem(ElementName = "PeopleName", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		public string[] PeopleNames { get; set; }

		[XmlArrayItem(ElementName = "EmailAddressEntity", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(EmailAddressEntityType))]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public EmailAddressEntityType[] EmailAddresses { get; set; }

		[XmlArrayItem(ElementName = "Contact", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(ContactType))]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public ContactType[] Contacts { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlArrayItem(ElementName = "UrlEntity", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(UrlEntityType))]
		public UrlEntityType[] Urls { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlArrayItem(ElementName = "Phone", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(PhoneEntityType))]
		public PhoneEntityType[] PhoneNumbers { get; set; }
	}
}
