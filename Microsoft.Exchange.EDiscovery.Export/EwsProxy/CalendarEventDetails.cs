using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class CalendarEventDetails
	{
		public string ID
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

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

		public bool IsMeeting
		{
			get
			{
				return this.isMeetingField;
			}
			set
			{
				this.isMeetingField = value;
			}
		}

		public bool IsRecurring
		{
			get
			{
				return this.isRecurringField;
			}
			set
			{
				this.isRecurringField = value;
			}
		}

		public bool IsException
		{
			get
			{
				return this.isExceptionField;
			}
			set
			{
				this.isExceptionField = value;
			}
		}

		public bool IsReminderSet
		{
			get
			{
				return this.isReminderSetField;
			}
			set
			{
				this.isReminderSetField = value;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return this.isPrivateField;
			}
			set
			{
				this.isPrivateField = value;
			}
		}

		private string idField;

		private string subjectField;

		private string locationField;

		private bool isMeetingField;

		private bool isRecurringField;

		private bool isExceptionField;

		private bool isReminderSetField;

		private bool isPrivateField;
	}
}
