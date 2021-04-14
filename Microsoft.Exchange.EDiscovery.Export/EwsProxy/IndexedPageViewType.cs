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
	public class IndexedPageViewType : BasePagingType
	{
		[XmlAttribute]
		public int Offset
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

		private int offsetField;

		private IndexBasePointType basePointField;
	}
}
