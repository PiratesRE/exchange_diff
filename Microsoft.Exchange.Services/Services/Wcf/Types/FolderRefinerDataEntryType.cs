using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[XmlType(TypeName = "FolderRefinerDataEntryType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class FolderRefinerDataEntryType : RefinerDataEntryType
	{
		public FolderRefinerDataEntryType()
		{
		}

		public FolderRefinerDataEntryType(FolderId folderId, long hitCount, string refinementQuery) : base(hitCount, refinementQuery)
		{
			this.FolderId = folderId;
		}

		[DataMember(IsRequired = true)]
		public FolderId FolderId { get; set; }
	}
}
