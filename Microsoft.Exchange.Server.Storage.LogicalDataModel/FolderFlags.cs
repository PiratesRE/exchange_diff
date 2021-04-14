using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	public enum FolderFlags
	{
		Ipm = 1,
		Search = 2,
		Normal = 4,
		Rules = 8
	}
}
