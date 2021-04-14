using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TransientDatabaseErrorSuppression : TransientErrorSuppression<Guid>
	{
		protected override void InitializeTable()
		{
			this.m_errorTable = new Dictionary<Guid, TransientErrorInfo>(48);
		}
	}
}
