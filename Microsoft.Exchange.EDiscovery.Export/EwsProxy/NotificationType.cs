using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class NotificationType
	{
		public string SubscriptionId
		{
			get
			{
				return this.subscriptionIdField;
			}
			set
			{
				this.subscriptionIdField = value;
			}
		}

		public string PreviousWatermark
		{
			get
			{
				return this.previousWatermarkField;
			}
			set
			{
				this.previousWatermarkField = value;
			}
		}

		public bool MoreEvents
		{
			get
			{
				return this.moreEventsField;
			}
			set
			{
				this.moreEventsField = value;
			}
		}

		[XmlIgnore]
		public bool MoreEventsSpecified
		{
			get
			{
				return this.moreEventsFieldSpecified;
			}
			set
			{
				this.moreEventsFieldSpecified = value;
			}
		}

		[XmlElement("DeletedEvent", typeof(BaseObjectChangedEventType))]
		[XmlElement("CreatedEvent", typeof(BaseObjectChangedEventType))]
		[XmlElement("FreeBusyChangedEvent", typeof(BaseObjectChangedEventType))]
		[XmlElement("ModifiedEvent", typeof(ModifiedEventType))]
		[XmlElement("MovedEvent", typeof(MovedCopiedEventType))]
		[XmlElement("NewMailEvent", typeof(BaseObjectChangedEventType))]
		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("CopiedEvent", typeof(MovedCopiedEventType))]
		[XmlElement("StatusEvent", typeof(BaseNotificationEventType))]
		public BaseNotificationEventType[] Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ItemsChoiceType[] ItemsElementName
		{
			get
			{
				return this.itemsElementNameField;
			}
			set
			{
				this.itemsElementNameField = value;
			}
		}

		private string subscriptionIdField;

		private string previousWatermarkField;

		private bool moreEventsField;

		private bool moreEventsFieldSpecified;

		private BaseNotificationEventType[] itemsField;

		private ItemsChoiceType[] itemsElementNameField;
	}
}
