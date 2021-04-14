using System;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Flags]
	public enum FreeBusyMemberRights
	{
		FreeBusySimple = 2048,
		FreeBusyDetailed = 4096,
		All = 6144
	}
}
