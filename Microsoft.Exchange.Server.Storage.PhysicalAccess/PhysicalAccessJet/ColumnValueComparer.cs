using System;
using System.Collections.Generic;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class ColumnValueComparer : IComparer<ColumnValue>
	{
		private ColumnValueComparer()
		{
		}

		public static ColumnValueComparer Instance
		{
			get
			{
				return ColumnValueComparer.instance;
			}
		}

		public int Compare(ColumnValue x, ColumnValue y)
		{
			return x.Columnid.CompareTo(y.Columnid);
		}

		private static readonly ColumnValueComparer instance = new ColumnValueComparer();
	}
}
