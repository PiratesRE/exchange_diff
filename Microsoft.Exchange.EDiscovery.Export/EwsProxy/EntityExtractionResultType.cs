using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class EntityExtractionResultType
	{
		[XmlArrayItem("AddressEntity", IsNullable = false)]
		public AddressEntityType[] Addresses
		{
			get
			{
				return this.addressesField;
			}
			set
			{
				this.addressesField = value;
			}
		}

		[XmlArrayItem("MeetingSuggestion", IsNullable = false)]
		public MeetingSuggestionType[] MeetingSuggestions
		{
			get
			{
				return this.meetingSuggestionsField;
			}
			set
			{
				this.meetingSuggestionsField = value;
			}
		}

		[XmlArrayItem("TaskSuggestion", IsNullable = false)]
		public TaskSuggestionType[] TaskSuggestions
		{
			get
			{
				return this.taskSuggestionsField;
			}
			set
			{
				this.taskSuggestionsField = value;
			}
		}

		[XmlArrayItem("EmailAddressEntity", IsNullable = false)]
		public EmailAddressEntityType[] EmailAddresses
		{
			get
			{
				return this.emailAddressesField;
			}
			set
			{
				this.emailAddressesField = value;
			}
		}

		[XmlArrayItem("Contact", IsNullable = false)]
		public ContactType[] Contacts
		{
			get
			{
				return this.contactsField;
			}
			set
			{
				this.contactsField = value;
			}
		}

		[XmlArrayItem("UrlEntity", IsNullable = false)]
		public UrlEntityType[] Urls
		{
			get
			{
				return this.urlsField;
			}
			set
			{
				this.urlsField = value;
			}
		}

		[XmlArrayItem("Phone", IsNullable = false)]
		public PhoneEntityType[] PhoneNumbers
		{
			get
			{
				return this.phoneNumbersField;
			}
			set
			{
				this.phoneNumbersField = value;
			}
		}

		private AddressEntityType[] addressesField;

		private MeetingSuggestionType[] meetingSuggestionsField;

		private TaskSuggestionType[] taskSuggestionsField;

		private EmailAddressEntityType[] emailAddressesField;

		private ContactType[] contactsField;

		private UrlEntityType[] urlsField;

		private PhoneEntityType[] phoneNumbersField;
	}
}
