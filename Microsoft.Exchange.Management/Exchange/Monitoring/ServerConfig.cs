using System;

namespace Microsoft.Exchange.Monitoring
{
	[Flags]
	internal enum ServerConfig
	{
		Unknown = 0,
		DagMemberNoDatabases = 1,
		DagMember = 2,
		Stopped = 4,
		RcrSource = 8,
		RcrTarget = 16
	}
}
