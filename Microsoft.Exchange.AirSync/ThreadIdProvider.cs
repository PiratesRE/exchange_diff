using System;
using System.Threading;

namespace Microsoft.Exchange.AirSync
{
	internal class ThreadIdProvider
	{
		public static int ManagedThreadId
		{
			get
			{
				if (ThreadIdProvider.provider != null)
				{
					return ThreadIdProvider.provider.ManagedThreadId;
				}
				return Thread.CurrentThread.ManagedThreadId;
			}
		}

		public static IThreadIdProvider SetProvider(IThreadIdProvider provider)
		{
			IThreadIdProvider result = ThreadIdProvider.provider;
			ThreadIdProvider.provider = provider;
			return result;
		}

		private static IThreadIdProvider provider;
	}
}
