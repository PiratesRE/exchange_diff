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
	public class NotificationType
	{
		public string SubscriptionId;

		public string PreviousWatermark;

		public bool MoreEvents;

		[XmlIgnore]
		public bool MoreEventsSpecified;

		[XmlElement("StatusEvent", typeof(BaseNotificationEventType))]
		[XmlElement("CopiedEvent", typeof(MovedCopiedEventType))]
		[XmlElement("DeletedEvent", typeof(BaseObjectChangedEventType))]
		[XmlElement("MovedEvent", typeof(MovedCopiedEventType))]
		[XmlElement("FreeBusyChangedEvent", typeof(BaseObjectChangedEventType))]
		[XmlElement("NewMailEvent", typeof(BaseObjectChangedEventType))]
		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("CreatedEvent", typeof(BaseObjectChangedEventType))]
		[XmlElement("ModifiedEvent", typeof(ModifiedEventType))]
		public BaseNotificationEventType[] Items;

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ItemsChoiceType[] ItemsElementName;
	}
}
