using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class TaskFolderEntry
	{
		[DataMember]
		public ItemId ItemId { get; set; }

		[DataMember]
		public string FolderName { get; set; }

		[DataMember]
		public string ParentGroupId { get; set; }

		[DataMember]
		public FolderId TaskFolderId { get; set; }

		[DataMember]
		public TaskFolderEntryType TaskFolderEntryType { get; set; }
	}
}
