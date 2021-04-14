using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class GetRemindersType : BaseRequestType
	{
		public DateTime BeginTime
		{
			get
			{
				return this.beginTimeField;
			}
			set
			{
				this.beginTimeField = value;
			}
		}

		[XmlIgnore]
		public bool BeginTimeSpecified
		{
			get
			{
				return this.beginTimeFieldSpecified;
			}
			set
			{
				this.beginTimeFieldSpecified = value;
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

		public int MaxItems
		{
			get
			{
				return this.maxItemsField;
			}
			set
			{
				this.maxItemsField = value;
			}
		}

		[XmlIgnore]
		public bool MaxItemsSpecified
		{
			get
			{
				return this.maxItemsFieldSpecified;
			}
			set
			{
				this.maxItemsFieldSpecified = value;
			}
		}

		public GetRemindersTypeReminderType ReminderType
		{
			get
			{
				return this.reminderTypeField;
			}
			set
			{
				this.reminderTypeField = value;
			}
		}

		[XmlIgnore]
		public bool ReminderTypeSpecified
		{
			get
			{
				return this.reminderTypeFieldSpecified;
			}
			set
			{
				this.reminderTypeFieldSpecified = value;
			}
		}

		private DateTime beginTimeField;

		private bool beginTimeFieldSpecified;

		private DateTime endTimeField;

		private bool endTimeFieldSpecified;

		private int maxItemsField;

		private bool maxItemsFieldSpecified;

		private GetRemindersTypeReminderType reminderTypeField;

		private bool reminderTypeFieldSpecified;
	}
}
