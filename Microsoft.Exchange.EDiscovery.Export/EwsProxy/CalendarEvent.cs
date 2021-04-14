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
	public class CalendarEvent
	{
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

		public LegacyFreeBusyType BusyType
		{
			get
			{
				return this.busyTypeField;
			}
			set
			{
				this.busyTypeField = value;
			}
		}

		public CalendarEventDetails CalendarEventDetails
		{
			get
			{
				return this.calendarEventDetailsField;
			}
			set
			{
				this.calendarEventDetailsField = value;
			}
		}

		private DateTime startTimeField;

		private DateTime endTimeField;

		private LegacyFreeBusyType busyTypeField;

		private CalendarEventDetails calendarEventDetailsField;
	}
}
