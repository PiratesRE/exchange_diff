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
	public class WorkingPeriod
	{
		public string DayOfWeek
		{
			get
			{
				return this.dayOfWeekField;
			}
			set
			{
				this.dayOfWeekField = value;
			}
		}

		public int StartTimeInMinutes
		{
			get
			{
				return this.startTimeInMinutesField;
			}
			set
			{
				this.startTimeInMinutesField = value;
			}
		}

		public int EndTimeInMinutes
		{
			get
			{
				return this.endTimeInMinutesField;
			}
			set
			{
				this.endTimeInMinutesField = value;
			}
		}

		private string dayOfWeekField;

		private int startTimeInMinutesField;

		private int endTimeInMinutesField;
	}
}
