using System;
using System.Threading;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal sealed class MigrationScanner : MigrationComponent
	{
		internal MigrationScanner(string name, WaitHandle stopEvent) : base(name, stopEvent)
		{
			this.nextProcessTime = ExDateTime.MinValue;
		}

		internal override bool ShouldProcess()
		{
			return ExDateTime.UtcNow >= this.nextProcessTime;
		}

		internal override bool Process(IMigrationJobCache data)
		{
			MigrationUtil.ThrowOnNullArgument(data, "data");
			this.nextProcessTime = ExDateTime.UtcNow.Add(MigrationScanner.processDelay);
			return data.SyncWithStore();
		}

		private static readonly TimeSpan processDelay = TimeSpan.FromMinutes(5.0);

		private ExDateTime nextProcessTime;
	}
}
