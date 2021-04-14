using System;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class IPAllowListEntry : IPListEntry
	{
		public override IPListEntryType ListType
		{
			get
			{
				return IPListEntryType.Allow;
			}
		}
	}
}
