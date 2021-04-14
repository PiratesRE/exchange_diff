using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Common;

namespace Microsoft.Exchange.Cluster.Replay.DatabaseCopyLayout
{
	internal class DatabaseGroupEqualityComparer : IEqualityComparer<DatabaseGroupLayoutEntry>
	{
		public static DatabaseGroupEqualityComparer Instance
		{
			get
			{
				return DatabaseGroupEqualityComparer.d_instance;
			}
		}

		private DatabaseGroupEqualityComparer()
		{
		}

		public bool Equals(DatabaseGroupLayoutEntry source, DatabaseGroupLayoutEntry target)
		{
			return StringUtil.IsEqualIgnoreCase(source.DatabaseGroupId, target.DatabaseGroupId) && source.DatabaseNameList.SequenceEqual(target.DatabaseNameList, StringComparer.InvariantCultureIgnoreCase);
		}

		public int GetHashCode(DatabaseGroupLayoutEntry key)
		{
			int num = 1;
			foreach (string text in key.DatabaseNameList)
			{
				num ^= text.GetHashCode();
			}
			return key.DatabaseGroupId.GetHashCode() ^ num;
		}

		private static readonly DatabaseGroupEqualityComparer d_instance = new DatabaseGroupEqualityComparer();
	}
}
