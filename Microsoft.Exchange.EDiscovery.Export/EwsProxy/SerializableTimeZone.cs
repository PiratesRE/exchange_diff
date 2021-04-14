using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class SerializableTimeZone
	{
		public int Bias
		{
			get
			{
				return this.biasField;
			}
			set
			{
				this.biasField = value;
			}
		}

		public SerializableTimeZoneTime StandardTime
		{
			get
			{
				return this.standardTimeField;
			}
			set
			{
				this.standardTimeField = value;
			}
		}

		public SerializableTimeZoneTime DaylightTime
		{
			get
			{
				return this.daylightTimeField;
			}
			set
			{
				this.daylightTimeField = value;
			}
		}

		private int biasField;

		private SerializableTimeZoneTime standardTimeField;

		private SerializableTimeZoneTime daylightTimeField;
	}
}
