using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class TaskFolderActionResponse
	{
		public TaskFolderActionResponse(TaskFolderActionError errorCode)
		{
			this.WasSuccessful = false;
			this.ErrorCode = errorCode;
		}

		public TaskFolderActionResponse()
		{
			this.WasSuccessful = true;
		}

		[DataMember]
		public bool WasSuccessful { get; set; }

		[DataMember]
		public TaskFolderActionError ErrorCode { get; set; }
	}
}
