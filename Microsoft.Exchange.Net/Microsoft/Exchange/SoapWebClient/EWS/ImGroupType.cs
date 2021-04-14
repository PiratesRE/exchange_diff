using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class ImGroupType
	{
		public string DisplayName;

		public string GroupType;

		public ItemIdType ExchangeStoreId;

		[XmlArrayItem("ItemId", IsNullable = false)]
		public ItemIdType[] MemberCorrelationKey;

		public NonEmptyArrayOfExtendedPropertyType ExtendedProperties;

		public string SmtpAddress;
	}
}
