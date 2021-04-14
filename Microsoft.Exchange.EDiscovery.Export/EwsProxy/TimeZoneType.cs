using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class TimeZoneType
	{
		[XmlElement(DataType = "duration")]
		public string BaseOffset
		{
			get
			{
				return this.baseOffsetField;
			}
			set
			{
				this.baseOffsetField = value;
			}
		}

		public TimeChangeType Standard
		{
			get
			{
				return this.standardField;
			}
			set
			{
				this.standardField = value;
			}
		}

		public TimeChangeType Daylight
		{
			get
			{
				return this.daylightField;
			}
			set
			{
				this.daylightField = value;
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

		private string baseOffsetField;

		private TimeChangeType standardField;

		private TimeChangeType daylightField;

		private string timeZoneNameField;
	}
}
