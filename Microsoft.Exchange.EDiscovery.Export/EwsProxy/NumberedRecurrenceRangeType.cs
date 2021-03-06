using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class NumberedRecurrenceRangeType : RecurrenceRangeBaseType
	{
		public int NumberOfOccurrences
		{
			get
			{
				return this.numberOfOccurrencesField;
			}
			set
			{
				this.numberOfOccurrencesField = value;
			}
		}

		private int numberOfOccurrencesField;
	}
}
