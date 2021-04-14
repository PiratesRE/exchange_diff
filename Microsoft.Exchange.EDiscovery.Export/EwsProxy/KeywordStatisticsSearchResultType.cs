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
	public class KeywordStatisticsSearchResultType
	{
		public string Keyword
		{
			get
			{
				return this.keywordField;
			}
			set
			{
				this.keywordField = value;
			}
		}

		public int ItemHits
		{
			get
			{
				return this.itemHitsField;
			}
			set
			{
				this.itemHitsField = value;
			}
		}

		public long Size
		{
			get
			{
				return this.sizeField;
			}
			set
			{
				this.sizeField = value;
			}
		}

		private string keywordField;

		private int itemHitsField;

		private long sizeField;
	}
}
