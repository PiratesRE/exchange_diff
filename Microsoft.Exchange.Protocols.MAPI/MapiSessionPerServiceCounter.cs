using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class MapiSessionPerServiceCounter : IMapiObjectCounter
	{
		private MapiSessionPerServiceCounter(MapiServiceType serviceType)
		{
			this.serviceType = serviceType;
			this.objectCounter = 0L;
		}

		public long GetCount()
		{
			return Interlocked.Read(ref this.objectCounter);
		}

		public void IncrementCount()
		{
			Interlocked.Increment(ref this.objectCounter);
		}

		public void DecrementCount()
		{
			Interlocked.Decrement(ref this.objectCounter);
		}

		public void CheckObjectQuota(bool mustBeStrictlyUnderQuota)
		{
			long num = Interlocked.Read(ref this.objectCounter);
			long num2 = ActiveObjectLimits.EffectiveLimitation(this.serviceType);
			if (num2 == -1L)
			{
				return;
			}
			bool flag = mustBeStrictlyUnderQuota ? (num >= num2) : (num > num2);
			if (flag)
			{
				throw new StoreException((LID)53288U, ErrorCodeValue.MaxObjectsExceeded, "Exceeded object size limitation, service type=" + this.serviceType.ToString());
			}
		}

		internal static IMapiObjectCounter GetObjectCounter(MapiServiceType serviceType)
		{
			if (serviceType >= MapiServiceType.UnknownServiceType)
			{
				return UnlimitedObjectCounter.Instance;
			}
			return MapiSessionPerServiceCounter.serviceCounters[(int)serviceType];
		}

		internal static void Initialize()
		{
			for (int i = 0; i < 8; i++)
			{
				MapiSessionPerServiceCounter.serviceCounters[i] = new MapiSessionPerServiceCounter((MapiServiceType)i);
			}
		}

		private static TimeSpan interval = TimeSpan.FromMinutes(5.0);

		private static MapiSessionPerServiceCounter[] serviceCounters = new MapiSessionPerServiceCounter[8];

		private readonly MapiServiceType serviceType;

		private long objectCounter;
	}
}
