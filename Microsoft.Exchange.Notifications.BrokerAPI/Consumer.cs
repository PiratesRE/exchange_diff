using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	internal sealed class Consumer
	{
		private Consumer()
		{
			string key;
			switch (key = ApplicationName.Current.Name.ToLowerInvariant())
			{
			case "msexchangeowaapppool":
				this.ConsumerId = ConsumerId.OWA;
				return;
			case "perseusharnessruntime":
			case "perseusstudio":
			case "powershell":
			case "testconsumer":
			case "topoagent":
				this.ConsumerId = ConsumerId.Test;
				return;
			}
			throw new InvalidOperationException();
		}

		public static Consumer Current
		{
			get
			{
				return Consumer.lazy.Value;
			}
		}

		public ConsumerId ConsumerId { get; private set; }

		private static readonly Lazy<Consumer> lazy = new Lazy<Consumer>(() => new Consumer());
	}
}
