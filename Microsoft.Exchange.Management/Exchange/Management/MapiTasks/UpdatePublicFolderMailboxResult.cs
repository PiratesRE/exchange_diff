using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Serializable]
	public class UpdatePublicFolderMailboxResult
	{
		public LocalizedString Message { get; set; }

		public UpdatePublicFolderMailboxResult(LocalizedString msg)
		{
			this.Message = msg;
		}
	}
}
