using System;

namespace Microsoft.Exchange.Diagnostics
{
	public sealed class ThreadTraceSettings
	{
		internal ThreadTraceSettings()
		{
		}

		public void EnableTracing()
		{
			this.enabledCount++;
		}

		public void DisableTracing()
		{
			if (this.enabledCount == 0)
			{
				throw new InvalidOperationException("Mismatched number of calls to enable/disable tracing");
			}
			this.enabledCount--;
		}

		public bool IsEnabled
		{
			get
			{
				return this.enabledCount > 0;
			}
		}

		private int enabledCount;
	}
}
