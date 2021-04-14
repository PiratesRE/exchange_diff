using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public sealed class MapiPerSessionObjectCounter : IMapiObjectCounter
	{
		public MapiPerSessionObjectCounter(MapiObjectTrackedType objectType, MapiSession session)
		{
			this.trackedObjectType = objectType;
			this.session = session;
			this.objectCounter = 0L;
			this.lastReportTime = DateTime.UtcNow.AddMonths(-1).ToBinary();
		}

		public MapiObjectTrackedType TrackedObjectType
		{
			get
			{
				return this.trackedObjectType;
			}
		}

		public DateTime LastReportTime
		{
			get
			{
				long dateData = Interlocked.Read(ref this.lastReportTime);
				return DateTime.FromBinary(dateData);
			}
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
			long num2 = ActiveObjectLimits.EffectiveLimitation(this.trackedObjectType);
			if (num2 == -1L)
			{
				return;
			}
			bool flag = mustBeStrictlyUnderQuota ? (num >= num2) : (num > num2);
			if (flag)
			{
				DateTime utcNow = DateTime.UtcNow;
				DateTime d = this.LastReportTime;
				if (utcNow - d > MapiPerSessionObjectCounter.interval)
				{
					long num3 = d.ToBinary();
					if (num3 == Interlocked.CompareExchange(ref this.lastReportTime, num3, utcNow.ToBinary()))
					{
						string periodicKey = this.session.UserDN + this.session.InternalClientType + this.trackedObjectType;
						Globals.LogPeriodicEvent(periodicKey, MSExchangeISEventLogConstants.Tuple_MaxObjectsExceeded, new object[]
						{
							this.session.UserDN,
							this.session.InternalClientType,
							num2,
							this.trackedObjectType
						});
					}
				}
				DiagnosticContext.TraceDwordAndString((LID)53840U, (uint)(num2 & (long)((ulong)-1)), this.trackedObjectType.ToString());
				DiagnosticContext.TraceDword((LID)41552U, (uint)num);
				throw new StoreException((LID)45096U, ErrorCodeValue.MaxObjectsExceeded, "Exceeded object size limitation, type=" + this.trackedObjectType.ToString());
			}
		}

		private static TimeSpan interval = TimeSpan.FromMinutes(30.0);

		private readonly MapiObjectTrackedType trackedObjectType;

		private long objectCounter;

		private long lastReportTime;

		private MapiSession session;
	}
}
