using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderSyncStateUtil
	{
		public delegate void CommitStateModificationsDelegate();
	}
}
