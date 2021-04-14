using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class ReminderMessageDataType
	{
		public string ReminderText
		{
			get
			{
				return this.reminderTextField;
			}
			set
			{
				this.reminderTextField = value;
			}
		}

		public string Location
		{
			get
			{
				return this.locationField;
			}
			set
			{
				this.locationField = value;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.startTimeField;
			}
			set
			{
				this.startTimeField = value;
			}
		}

		[XmlIgnore]
		public bool StartTimeSpecified
		{
			get
			{
				return this.startTimeFieldSpecified;
			}
			set
			{
				this.startTimeFieldSpecified = value;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return this.endTimeField;
			}
			set
			{
				this.endTimeField = value;
			}
		}

		[XmlIgnore]
		public bool EndTimeSpecified
		{
			get
			{
				return this.endTimeFieldSpecified;
			}
			set
			{
				this.endTimeFieldSpecified = value;
			}
		}

		public ItemIdType AssociatedCalendarItemId
		{
			get
			{
				return this.associatedCalendarItemIdField;
			}
			set
			{
				this.associatedCalendarItemIdField = value;
			}
		}

		private string reminderTextField;

		private string locationField;

		private DateTime startTimeField;

		private bool startTimeFieldSpecified;

		private DateTime endTimeField;

		private bool endTimeFieldSpecified;

		private ItemIdType associatedCalendarItemIdField;
	}
}
