using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(NumberedRecurrenceRangeType))]
	[XmlInclude(typeof(NoEndRecurrenceRangeType))]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(EndDateRecurrenceRangeType))]
	[Serializable]
	public abstract class RecurrenceRangeBaseType
	{
		[XmlElement(DataType = "date")]
		public DateTime StartDate
		{
			get
			{
				return this.startDateField;
			}
			set
			{
				this.startDateField = value;
			}
		}

		private DateTime startDateField;
	}
}
