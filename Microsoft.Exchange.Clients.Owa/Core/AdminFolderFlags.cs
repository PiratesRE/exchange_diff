using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum AdminFolderFlags
	{
		Provisioned = 1,
		Protected = 2,
		MustDisplayComment = 4,
		Quota = 8,
		ELCRoot = 16
	}
}
