using System;

namespace Microsoft.Exchange.Data.GroupMailbox.Common
{
	internal static class Extensions
	{
		internal static bool Contains(this GroupMailboxConfigurationAction groupMailboxConfigurationActionMask, GroupMailboxConfigurationAction flag)
		{
			return (groupMailboxConfigurationActionMask & flag) != GroupMailboxConfigurationAction.None;
		}
	}
}
