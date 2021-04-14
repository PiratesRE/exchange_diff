using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class EntityExtractionResultType
	{
		[XmlArrayItem("AddressEntity", IsNullable = false)]
		public AddressEntityType[] Addresses;

		[XmlArrayItem("MeetingSuggestion", IsNullable = false)]
		public MeetingSuggestionType[] MeetingSuggestions;

		[XmlArrayItem("TaskSuggestion", IsNullable = false)]
		public TaskSuggestionType[] TaskSuggestions;

		[XmlArrayItem("EmailAddressEntity", IsNullable = false)]
		public EmailAddressEntityType[] EmailAddresses;

		[XmlArrayItem("Contact", IsNullable = false)]
		public ContactType[] Contacts;

		[XmlArrayItem("UrlEntity", IsNullable = false)]
		public UrlEntityType[] Urls;

		[XmlArrayItem("Phone", IsNullable = false)]
		public PhoneEntityType[] PhoneNumbers;
	}
}
