using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class RecipientTrackingEventType
	{
		public DateTime Date;

		public EmailAddressType Recipient;

		public string DeliveryStatus;

		public string EventDescription;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] EventData;

		public string Server;

		[XmlElement(DataType = "nonNegativeInteger")]
		public string InternalId;

		public bool BccRecipient;

		[XmlIgnore]
		public bool BccRecipientSpecified;

		public bool HiddenRecipient;

		[XmlIgnore]
		public bool HiddenRecipientSpecified;

		public string UniquePathId;

		public string RootAddress;

		[XmlArrayItem(IsNullable = false)]
		public TrackingPropertyType[] Properties;
	}
}
