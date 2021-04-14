using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderHierarchySyncStateInfo : SyncStateInfo
	{
		public override int Version
		{
			get
			{
				return 2;
			}
		}

		public override string UniqueName
		{
			get
			{
				return "Root";
			}
			set
			{
				throw new InvalidOperationException("FolderHierarchySyncStateInfo.UniqueName is not settable.");
			}
		}

		public const string UniqueNameString = "Root";
	}
}
