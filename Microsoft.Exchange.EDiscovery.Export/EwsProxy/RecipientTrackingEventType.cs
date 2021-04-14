using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class RecipientTrackingEventType
	{
		public DateTime Date
		{
			get
			{
				return this.dateField;
			}
			set
			{
				this.dateField = value;
			}
		}

		public EmailAddressType Recipient
		{
			get
			{
				return this.recipientField;
			}
			set
			{
				this.recipientField = value;
			}
		}

		public string DeliveryStatus
		{
			get
			{
				return this.deliveryStatusField;
			}
			set
			{
				this.deliveryStatusField = value;
			}
		}

		public string EventDescription
		{
			get
			{
				return this.eventDescriptionField;
			}
			set
			{
				this.eventDescriptionField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] EventData
		{
			get
			{
				return this.eventDataField;
			}
			set
			{
				this.eventDataField = value;
			}
		}

		public string Server
		{
			get
			{
				return this.serverField;
			}
			set
			{
				this.serverField = value;
			}
		}

		[XmlElement(DataType = "nonNegativeInteger")]
		public string InternalId
		{
			get
			{
				return this.internalIdField;
			}
			set
			{
				this.internalIdField = value;
			}
		}

		public bool BccRecipient
		{
			get
			{
				return this.bccRecipientField;
			}
			set
			{
				this.bccRecipientField = value;
			}
		}

		[XmlIgnore]
		public bool BccRecipientSpecified
		{
			get
			{
				return this.bccRecipientFieldSpecified;
			}
			set
			{
				this.bccRecipientFieldSpecified = value;
			}
		}

		public bool HiddenRecipient
		{
			get
			{
				return this.hiddenRecipientField;
			}
			set
			{
				this.hiddenRecipientField = value;
			}
		}

		[XmlIgnore]
		public bool HiddenRecipientSpecified
		{
			get
			{
				return this.hiddenRecipientFieldSpecified;
			}
			set
			{
				this.hiddenRecipientFieldSpecified = value;
			}
		}

		public string UniquePathId
		{
			get
			{
				return this.uniquePathIdField;
			}
			set
			{
				this.uniquePathIdField = value;
			}
		}

		public string RootAddress
		{
			get
			{
				return this.rootAddressField;
			}
			set
			{
				this.rootAddressField = value;
			}
		}

		[XmlArrayItem(IsNullable = false)]
		public TrackingPropertyType[] Properties
		{
			get
			{
				return this.propertiesField;
			}
			set
			{
				this.propertiesField = value;
			}
		}

		private DateTime dateField;

		private EmailAddressType recipientField;

		private string deliveryStatusField;

		private string eventDescriptionField;

		private string[] eventDataField;

		private string serverField;

		private string internalIdField;

		private bool bccRecipientField;

		private bool bccRecipientFieldSpecified;

		private bool hiddenRecipientField;

		private bool hiddenRecipientFieldSpecified;

		private string uniquePathIdField;

		private string rootAddressField;

		private TrackingPropertyType[] propertiesField;
	}
}
