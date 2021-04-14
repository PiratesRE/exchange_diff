using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class FailoverData : IActionData
	{
		public ExDateTime Time { get; private set; }

		public string DataStr { get; private set; }

		public FailoverData()
		{
			this.Initialize(ExDateTime.Now, string.Empty);
		}

		public void Initialize(ExDateTime actionTime, string dataStr)
		{
			this.Time = actionTime;
			this.DataStr = dataStr;
		}
	}
}
