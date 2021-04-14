using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationNspiGetNewDsaRpcResult : MigrationProxyRpcResult
	{
		public MigrationNspiGetNewDsaRpcResult() : base(MigrationProxyRpcType.GetNewDSA)
		{
		}

		public MigrationNspiGetNewDsaRpcResult(byte[] resultBlob) : base(resultBlob, MigrationProxyRpcType.GetNewDSA)
		{
		}

		public string NspiServer
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2432892959U, out obj))
				{
					return obj as string;
				}
				return null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.PropertyCollection[2432892959U] = value;
					return;
				}
				this.PropertyCollection.Remove(2432892959U);
			}
		}
	}
}
