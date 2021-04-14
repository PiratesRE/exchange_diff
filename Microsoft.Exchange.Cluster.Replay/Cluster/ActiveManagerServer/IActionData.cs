using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal interface IActionData
	{
		void Initialize(ExDateTime actionTime, string dataStr);

		ExDateTime Time { get; }

		string DataStr { get; }
	}
}
