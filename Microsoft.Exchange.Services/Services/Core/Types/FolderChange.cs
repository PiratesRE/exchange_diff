using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "FolderChangeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class FolderChange : StoreObjectChangeBase
	{
		[XmlElement("DistinguishedFolderId", typeof(DistinguishedFolderId))]
		[DataMember(Name = "FolderId", IsRequired = true)]
		[XmlElement("FolderId", typeof(FolderId))]
		public BaseFolderId FolderId { get; set; }
	}
}
