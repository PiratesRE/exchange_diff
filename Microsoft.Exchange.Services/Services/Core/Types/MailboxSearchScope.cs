using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "MailboxSearchScope", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "MailboxSearchScopeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MailboxSearchScope
	{
		public MailboxSearchScope()
		{
		}

		public MailboxSearchScope(string mailbox, MailboxSearchLocation searchScope)
		{
			this.mailbox = mailbox;
			this.searchScope = searchScope;
		}

		[DataMember(Name = "Mailbox", IsRequired = true, Order = 0)]
		[XmlElement("Mailbox")]
		public string Mailbox
		{
			get
			{
				return this.mailbox;
			}
			set
			{
				this.mailbox = value;
			}
		}

		[XmlElement("SearchScope")]
		[IgnoreDataMember]
		public MailboxSearchLocation SearchScope
		{
			get
			{
				return this.searchScope;
			}
			set
			{
				this.searchScope = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "SearchScope", IsRequired = true, Order = 1)]
		public string SearchScopeString
		{
			get
			{
				return EnumUtilities.ToString<MailboxSearchLocation>(this.searchScope);
			}
			set
			{
				this.searchScope = EnumUtilities.Parse<MailboxSearchLocation>(value);
			}
		}

		[XmlArray(ElementName = "ExtendedAttributes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "ExtendedAttribute", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(ExtendedAttribute))]
		[DataMember(Name = "ExtendedAttributes", IsRequired = false, EmitDefaultValue = false, Order = 2)]
		public ExtendedAttribute[] ExtendedAttributes
		{
			get
			{
				return this.extendedAttributes;
			}
			set
			{
				this.extendedAttributes = value;
			}
		}

		[XmlIgnore]
		public bool ExtendedAttributesSpecified
		{
			get
			{
				return false;
			}
		}

		private string mailbox;

		private MailboxSearchLocation searchScope = MailboxSearchLocation.All;

		private ExtendedAttribute[] extendedAttributes;
	}
}
