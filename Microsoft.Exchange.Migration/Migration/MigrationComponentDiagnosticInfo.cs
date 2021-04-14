using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationComponentDiagnosticInfo
	{
		public long Duration
		{
			get
			{
				return this.duration;
			}
			set
			{
				this.duration = value;
			}
		}

		public ExDateTime LastRunTime
		{
			get
			{
				return this.lastRunTime;
			}
			set
			{
				this.lastRunTime = value;
			}
		}

		public ExDateTime LastWorkTime
		{
			get
			{
				return this.lastWorkTime;
			}
			set
			{
				this.lastWorkTime = value;
			}
		}

		private long duration;

		private ExDateTime lastRunTime;

		private ExDateTime lastWorkTime;
	}
}
