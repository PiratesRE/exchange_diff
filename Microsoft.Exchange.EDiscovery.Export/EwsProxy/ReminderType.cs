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
	public class ReminderType
	{
		public string Subject
		{
			get
			{
				return this.subjectField;
			}
			set
			{
				this.subjectField = value;
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

		public DateTime ReminderTime
		{
			get
			{
				return this.reminderTimeField;
			}
			set
			{
				this.reminderTimeField = value;
			}
		}

		public DateTime StartDate
		{
			get
			{
				return this.startDateField;
			}
			set
			{
				this.startDateField = value;
			}
		}

		public DateTime EndDate
		{
			get
			{
				return this.endDateField;
			}
			set
			{
				this.endDateField = value;
			}
		}

		public ItemIdType ItemId
		{
			get
			{
				return this.itemIdField;
			}
			set
			{
				this.itemIdField = value;
			}
		}

		public ItemIdType RecurringMasterItemId
		{
			get
			{
				return this.recurringMasterItemIdField;
			}
			set
			{
				this.recurringMasterItemIdField = value;
			}
		}

		public ReminderGroupType ReminderGroup
		{
			get
			{
				return this.reminderGroupField;
			}
			set
			{
				this.reminderGroupField = value;
			}
		}

		[XmlIgnore]
		public bool ReminderGroupSpecified
		{
			get
			{
				return this.reminderGroupFieldSpecified;
			}
			set
			{
				this.reminderGroupFieldSpecified = value;
			}
		}

		public string UID
		{
			get
			{
				return this.uIDField;
			}
			set
			{
				this.uIDField = value;
			}
		}

		private string subjectField;

		private string locationField;

		private DateTime reminderTimeField;

		private DateTime startDateField;

		private DateTime endDateField;

		private ItemIdType itemIdField;

		private ItemIdType recurringMasterItemIdField;

		private ReminderGroupType reminderGroupField;

		private bool reminderGroupFieldSpecified;

		private string uIDField;
	}
}
