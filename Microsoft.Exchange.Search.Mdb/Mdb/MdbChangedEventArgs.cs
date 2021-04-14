using System;

namespace Microsoft.Exchange.Search.Mdb
{
	internal sealed class MdbChangedEventArgs : EventArgs
	{
		internal MdbChangedEventArgs(MdbChangedEntry[] mdbChangedEntries)
		{
			this.changedDatabases = mdbChangedEntries;
		}

		internal MdbChangedEntry[] ChangedDatabases
		{
			get
			{
				return this.changedDatabases;
			}
		}

		private readonly MdbChangedEntry[] changedDatabases;
	}
}
