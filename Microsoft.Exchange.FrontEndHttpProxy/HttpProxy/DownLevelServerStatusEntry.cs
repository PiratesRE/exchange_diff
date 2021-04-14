using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;

namespace Microsoft.Exchange.HttpProxy
{
	internal class DownLevelServerStatusEntry
	{
		public BackEndServer BackEndServer { get; set; }

		public bool IsHealthy { get; set; }
	}
}
