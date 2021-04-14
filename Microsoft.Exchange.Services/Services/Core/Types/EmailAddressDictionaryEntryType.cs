using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class EmailAddressDictionaryEntryType
	{
		[OnDeserializing]
		public void Initialize(StreamingContext context)
		{
			this.Initialize();
		}

		public static implicit operator EmailAddressWrapper(EmailAddressDictionaryEntryType entry)
		{
			return entry.emailAddress;
		}

		public EmailAddressDictionaryEntryType()
		{
			this.Initialize();
		}

		public EmailAddressDictionaryEntryType(EmailAddressKeyType key, string emailAddress)
		{
			this.Initialize();
			this.Key = key;
			this.emailAddress.EmailAddress = emailAddress;
		}

		private void Initialize()
		{
			this.emailAddress = new EmailAddressWrapper();
		}

		[IgnoreDataMember]
		[XmlAttribute]
		public EmailAddressKeyType Key { get; set; }

		[DataMember(Name = "Key", EmitDefaultValue = false, Order = 0)]
		[XmlIgnore]
		public string KeyString
		{
			get
			{
				return EnumUtilities.ToString<EmailAddressKeyType>(this.Key);
			}
			set
			{
				this.Key = EnumUtilities.Parse<EmailAddressKeyType>(value);
			}
		}

		[XmlAttribute]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string Name
		{
			get
			{
				return this.emailAddress.Name;
			}
			set
			{
				this.emailAddress.Name = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		[XmlAttribute]
		public string RoutingType
		{
			get
			{
				return this.emailAddress.RoutingType;
			}
			set
			{
				this.emailAddress.RoutingType = value;
			}
		}

		[DataMember(Name = "MailboxType", EmitDefaultValue = false, Order = 3)]
		[XmlAttribute]
		public string MailboxType
		{
			get
			{
				return this.emailAddress.MailboxType;
			}
			set
			{
				this.emailAddress.MailboxType = value;
				this.MailboxTypeSpecified = true;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool MailboxTypeSpecified { get; set; }

		[DataMember(Name = "EmailAddress", EmitDefaultValue = false, Order = 4)]
		[XmlText]
		public string Value
		{
			get
			{
				return this.emailAddress.EmailAddress;
			}
			set
			{
				this.emailAddress.EmailAddress = value;
			}
		}

		private EmailAddressWrapper emailAddress;
	}
}
