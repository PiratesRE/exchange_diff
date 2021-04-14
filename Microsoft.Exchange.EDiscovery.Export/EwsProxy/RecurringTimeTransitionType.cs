using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[XmlInclude(typeof(RecurringDayTransitionType))]
	[XmlInclude(typeof(RecurringDateTransitionType))]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public abstract class RecurringTimeTransitionType : TransitionType
	{
		[XmlElement(DataType = "duration")]
		public string TimeOffset
		{
			get
			{
				return this.timeOffsetField;
			}
			set
			{
				this.timeOffsetField = value;
			}
		}

		public int Month
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

		private string timeOffsetField;

		private int monthField;
	}
}
