using System;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal interface IStartStop
	{
		void Start();

		void PrepareToStop();

		void Stop();
	}
}
