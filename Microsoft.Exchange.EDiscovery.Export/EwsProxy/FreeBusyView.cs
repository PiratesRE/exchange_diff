using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class FreeBusyView
	{
		public FreeBusyViewType FreeBusyViewType
		{
			get
			{
				return this.freeBusyViewTypeField;
			}
			set
			{
				this.freeBusyViewTypeField = value;
			}
		}

		public string MergedFreeBusy
		{
			get
			{
				return this.mergedFreeBusyField;
			}
			set
			{
				this.mergedFreeBusyField = value;
			}
		}

		[XmlArrayItem(IsNullable = false)]
		public CalendarEvent[] CalendarEventArray
		{
			get
			{
				return this.calendarEventArrayField;
			}
			set
			{
				this.calendarEventArrayField = value;
			}
		}

		public WorkingHours WorkingHours
		{
			get
			{
				return this.workingHoursField;
			}
			set
			{
				this.workingHoursField = value;
			}
		}

		private FreeBusyViewType freeBusyViewTypeField;

		private string mergedFreeBusyField;

		private CalendarEvent[] calendarEventArrayField;

		private WorkingHours workingHoursField;
	}
}
