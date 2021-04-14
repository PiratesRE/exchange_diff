using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.QueueDigest
{
	internal class DagGroupOfServersKey : GroupOfServersKey
	{
		public DagGroupOfServersKey(ADObjectId dagId)
		{
			this.dagId = dagId;
		}

		public override bool Equals(object other)
		{
			DagGroupOfServersKey dagGroupOfServersKey = other as DagGroupOfServersKey;
			return dagGroupOfServersKey != null && this.dagId.Equals(dagGroupOfServersKey.dagId);
		}

		public override int GetHashCode()
		{
			return this.dagId.GetHashCode();
		}

		public override string ToString()
		{
			return this.dagId.ToString();
		}

		private readonly ADObjectId dagId;
	}
}
