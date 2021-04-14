using System;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class IPBlockListEntry : IPListEntry
	{
		public override IPListEntryType ListType
		{
			get
			{
				return IPListEntryType.Block;
			}
		}
	}
}
