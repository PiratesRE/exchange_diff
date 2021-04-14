using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class PerThreadData
	{
		internal static PerThreadData CurrentThreadData
		{
			get
			{
				PerThreadData perThreadData = PerThreadData.currentThread;
				if (perThreadData == null)
				{
					perThreadData = new PerThreadData();
					PerThreadData.currentThread = perThreadData;
				}
				return perThreadData;
			}
		}

		internal ThreadTraceSettings ThreadTraceSettings
		{
			get
			{
				return this.threadTraceSettings;
			}
		}

		[ThreadStatic]
		private static PerThreadData currentThread;

		private ThreadTraceSettings threadTraceSettings = new ThreadTraceSettings();
	}
}
