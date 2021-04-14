using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "TargetFolderIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class TargetFolderId
	{
		[XmlElement("AddressListId", typeof(AddressListId))]
		[XmlElement("DistinguishedFolderId", typeof(DistinguishedFolderId))]
		[DataMember(IsRequired = true)]
		[XmlElement("FolderId", typeof(FolderId))]
		public BaseFolderId BaseFolderId { get; set; }

		public TargetFolderId()
		{
		}

		public TargetFolderId(BaseFolderId baseFolderId)
		{
			this.BaseFolderId = baseFolderId;
		}
	}
}
