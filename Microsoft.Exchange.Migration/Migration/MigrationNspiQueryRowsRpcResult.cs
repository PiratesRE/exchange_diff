using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationNspiQueryRowsRpcResult : MigrationProxyRpcResult
	{
		public MigrationNspiQueryRowsRpcResult() : base(MigrationProxyRpcType.QueryRows)
		{
		}

		public MigrationNspiQueryRowsRpcResult(byte[] resultBlob) : base(resultBlob, MigrationProxyRpcType.QueryRows)
		{
		}

		public int? TotalRowCount
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2432827395U, out obj) && obj is int)
				{
					return new int?((int)obj);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.PropertyCollection[2432827395U] = value.Value;
					return;
				}
				this.PropertyCollection.Remove(2432827395U);
			}
		}
	}
}
