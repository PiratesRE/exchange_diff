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
	public class RecurringMasterItemIdRangesType : ItemIdType
	{
		[XmlArrayItem("Range", IsNullable = false)]
		public OccurrencesRangeType[] Ranges
		{
			get
			{
				return this.rangesField;
			}
			set
			{
				this.rangesField = value;
			}
		}

		private OccurrencesRangeType[] rangesField;
	}
}
