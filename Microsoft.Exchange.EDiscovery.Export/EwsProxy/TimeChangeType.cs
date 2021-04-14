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
	public class TimeChangeType
	{
		[XmlElement(DataType = "duration")]
		public string Offset
		{
			get
			{
				return this.offsetField;
			}
			set
			{
				this.offsetField = value;
			}
		}

		[XmlElement("RelativeYearlyRecurrence", typeof(RelativeYearlyRecurrencePatternType))]
		[XmlElement("AbsoluteDate", typeof(DateTime), DataType = "date")]
		public object Item
		{
			get
			{
				return this.itemField;
			}
			set
			{
				this.itemField = value;
			}
		}

		[XmlElement(DataType = "time")]
		public DateTime Time
		{
			get
			{
				return this.timeField;
			}
			set
			{
				this.timeField = value;
			}
		}

		[XmlAttribute]
		public string TimeZoneName
		{
			get
			{
				return this.timeZoneNameField;
			}
			set
			{
				this.timeZoneNameField = value;
			}
		}

		private string offsetField;

		private object itemField;

		private DateTime timeField;

		private string timeZoneNameField;
	}
}
