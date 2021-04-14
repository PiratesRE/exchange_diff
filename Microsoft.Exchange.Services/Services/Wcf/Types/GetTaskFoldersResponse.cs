using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetTaskFoldersResponse : TaskFolderActionResponse
	{
		[DataMember]
		public TaskGroup[] TaskGroups { get; internal set; }
	}
}
