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
	public class ArrayOfTimeZoneDefinitionType
	{
		[XmlElement("TimeZoneDefinition")]
		public TimeZoneDefinitionType[] TimeZoneDefinition
		{
			get
			{
				return this.timeZoneDefinitionField;
			}
			set
			{
				this.timeZoneDefinitionField = value;
			}
		}

		private TimeZoneDefinitionType[] timeZoneDefinitionField;
	}
}
