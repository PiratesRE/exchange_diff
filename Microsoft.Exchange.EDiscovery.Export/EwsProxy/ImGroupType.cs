using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ImGroupType
	{
		public string DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		public string GroupType
		{
			get
			{
				return this.groupTypeField;
			}
			set
			{
				this.groupTypeField = value;
			}
		}

		public ItemIdType ExchangeStoreId
		{
			get
			{
				return this.exchangeStoreIdField;
			}
			set
			{
				this.exchangeStoreIdField = value;
			}
		}

		[XmlArrayItem("ItemId", IsNullable = false)]
		public ItemIdType[] MemberCorrelationKey
		{
			get
			{
				return this.memberCorrelationKeyField;
			}
			set
			{
				this.memberCorrelationKeyField = value;
			}
		}

		public NonEmptyArrayOfExtendedPropertyType ExtendedProperties
		{
			get
			{
				return this.extendedPropertiesField;
			}
			set
			{
				this.extendedPropertiesField = value;
			}
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddressField;
			}
			set
			{
				this.smtpAddressField = value;
			}
		}

		private string displayNameField;

		private string groupTypeField;

		private ItemIdType exchangeStoreIdField;

		private ItemIdType[] memberCorrelationKeyField;

		private NonEmptyArrayOfExtendedPropertyType extendedPropertiesField;

		private string smtpAddressField;
	}
}
