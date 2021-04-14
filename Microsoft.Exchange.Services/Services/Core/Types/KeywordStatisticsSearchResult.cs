using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "KeywordStatisticsSearchResult", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "KeywordStatisticsSearchResultType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class KeywordStatisticsSearchResult
	{
		[DataMember(Name = "Keyword", IsRequired = true)]
		[XmlElement("Keyword")]
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

		[DataMember(Name = "ItemHits", IsRequired = true)]
		[XmlElement("ItemHits")]
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

		[DataMember(Name = "Size", IsRequired = true)]
		[XmlElement("Size")]
		public ulong Size
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

		private string keywordField = string.Empty;

		private int itemHitsField;

		private ulong sizeField;
	}
}
