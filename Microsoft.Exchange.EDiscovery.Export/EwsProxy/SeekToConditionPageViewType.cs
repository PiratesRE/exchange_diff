using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class SeekToConditionPageViewType : BasePagingType
	{
		public RestrictionType Condition
		{
			get
			{
				return this.conditionField;
			}
			set
			{
				this.conditionField = value;
			}
		}

		[XmlAttribute]
		public IndexBasePointType BasePoint
		{
			get
			{
				return this.basePointField;
			}
			set
			{
				this.basePointField = value;
			}
		}

		private RestrictionType conditionField;

		private IndexBasePointType basePointField;
	}
}
