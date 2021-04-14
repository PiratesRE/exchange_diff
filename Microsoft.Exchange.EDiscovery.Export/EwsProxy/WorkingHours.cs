using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class WorkingHours
	{
		public SerializableTimeZone TimeZone
		{
			get
			{
				return this.timeZoneField;
			}
			set
			{
				this.timeZoneField = value;
			}
		}

		[XmlArrayItem(IsNullable = false)]
		public WorkingPeriod[] WorkingPeriodArray
		{
			get
			{
				return this.workingPeriodArrayField;
			}
			set
			{
				this.workingPeriodArrayField = value;
			}
		}

		private SerializableTimeZone timeZoneField;

		private WorkingPeriod[] workingPeriodArrayField;
	}
}
