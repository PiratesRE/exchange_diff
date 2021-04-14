﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class RelativeYearlyRecurrencePatternType : RecurrencePatternBaseType
	{
		public string DaysOfWeek
		{
			get
			{
				return this.daysOfWeekField;
			}
			set
			{
				this.daysOfWeekField = value;
			}
		}

		public DayOfWeekIndexType DayOfWeekIndex
		{
			get
			{
				return this.dayOfWeekIndexField;
			}
			set
			{
				this.dayOfWeekIndexField = value;
			}
		}

		public MonthNamesType Month
		{
			get
			{
				return this.monthField;
			}
			set
			{
				this.monthField = value;
			}
		}

		private string daysOfWeekField;

		private DayOfWeekIndexType dayOfWeekIndexField;

		private MonthNamesType monthField;
	}
}
