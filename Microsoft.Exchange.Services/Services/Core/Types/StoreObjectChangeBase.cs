using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(DeleteFolderPropertyUpdate))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(SetFolderPropertyUpdate))]
	[KnownType(typeof(AppendItemPropertyUpdate))]
	[KnownType(typeof(DeleteItemPropertyUpdate))]
	[KnownType(typeof(SetItemPropertyUpdate))]
	[KnownType(typeof(AppendFolderPropertyUpdate))]
	public class StoreObjectChangeBase : IStoreObjectChange
	{
		[XmlArrayItem("SetItemField", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(SetItemPropertyUpdate))]
		[XmlArrayItem("AppendToFolderField", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(AppendFolderPropertyUpdate))]
		[XmlArrayItem("DeleteItemField", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(DeleteItemPropertyUpdate))]
		[DataMember(Name = "Updates", IsRequired = true)]
		[XmlArray("Updates", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem("DeleteFolderField", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(DeleteFolderPropertyUpdate))]
		[XmlArrayItem("SetFolderField", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(SetFolderPropertyUpdate))]
		[XmlArrayItem("AppendToItemField", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(AppendItemPropertyUpdate))]
		public PropertyUpdate[] PropertyUpdates { get; set; }
	}
}
