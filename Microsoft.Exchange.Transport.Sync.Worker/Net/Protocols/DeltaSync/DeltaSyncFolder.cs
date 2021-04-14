using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncFolder : DeltaSyncObject
	{
		internal DeltaSyncFolder(Guid serverId) : base(serverId)
		{
		}

		internal DeltaSyncFolder(string clientId) : base(clientId)
		{
		}

		internal string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		internal static bool IsSystemFolder(string displayName)
		{
			return !string.IsNullOrEmpty(displayName) && displayName.StartsWith(".!!", StringComparison.OrdinalIgnoreCase);
		}

		internal const string RootServerId = "00000000-0000-0000-0000-000000000000";

		internal const string InboxServerId = "00000000-0000-0000-0000-000000000001";

		internal const string DeletedItemsServerId = "00000000-0000-0000-0000-000000000002";

		internal const string SentItemsServerId = "00000000-0000-0000-0000-000000000003";

		internal const string DraftsServerId = "00000000-0000-0000-0000-000000000004";

		internal const string JunkEmailServerId = "00000000-0000-0000-0000-000000000005";

		private const string SystemFolderDisplayNamePrefix = ".!!";

		internal static readonly DeltaSyncFolder DefaultRootFolder = new DeltaSyncFolder(new Guid("00000000-0000-0000-0000-000000000000"));

		private string displayName;
	}
}
