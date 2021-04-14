using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SyncFolderHierarchyDeleteType : SyncFolderHierarchyChangeBase
	{
		public SyncFolderHierarchyDeleteType()
		{
		}

		public SyncFolderHierarchyDeleteType(FolderId folderId)
		{
			this.FolderId = folderId;
		}

		[DataMember(EmitDefaultValue = false)]
		public FolderId FolderId { get; set; }

		public override SyncFolderHierarchyChangesEnum ChangeType
		{
			get
			{
				return SyncFolderHierarchyChangesEnum.Delete;
			}
		}
	}
}
