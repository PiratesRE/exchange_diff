using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "AttributionType")]
	[XmlType(TypeName = "Attribution", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class Attribution
	{
		[XmlElement]
		[DataMember(IsRequired = true, Order = 1)]
		public string Id { get; set; }

		[DataMember(IsRequired = true, Order = 2)]
		[XmlElement]
		public ItemId SourceId { get; set; }

		[XmlElement]
		[DataMember(IsRequired = true, Order = 3)]
		public string DisplayName { get; set; }

		[DataMember(IsRequired = false, Order = 4)]
		[XmlElement]
		public bool IsWritable { get; set; }

		[DataMember(IsRequired = false, Order = 5)]
		[XmlElement]
		public bool IsQuickContact { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false, Order = 6)]
		public bool IsHidden { get; set; }

		[DataMember(IsRequired = false, Order = 7)]
		[XmlElement]
		public FolderId FolderId { get; set; }

		[XmlIgnore]
		[DataMember(IsRequired = false, Order = 8)]
		public string FolderName { get; set; }

		public Attribution()
		{
		}

		public Attribution(string id, ItemId sourceId, string displayName)
		{
			this.Id = id;
			this.SourceId = sourceId;
			this.DisplayName = displayName;
		}
	}
}
