using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.Storage
{
	internal sealed class DataGenerationManager<TGeneration> where TGeneration : DataGeneration, new()
	{
		public DataGenerationManager(TimeSpan newGenerationTimeSpan, Func<TimeSpan> ageToCleanup, DataGenerationCategory category, DataGenerationTable table, int recentGenerationCount, bool autoExpireEnabled, bool autoCreateEnabled, bool expireSuspended = false)
		{
			if (expireSuspended)
			{
				this.SuspendDataCleanup();
			}
			this.autoCreateEnabled = autoCreateEnabled;
			this.autoExpireEnabled = autoExpireEnabled;
			this.category = category;
			this.generationTimeSpan = newGenerationTimeSpan;
			this.maintenanceTimer = new GuardedTimer(delegate(object o)
			{
				this.DoMaintenance();
			});
			this.ageToCleanup = ageToCleanup;
			this.table = table;
			this.recentGenerationCount = recentGenerationCount;
			Stopwatch stopwatch = Stopwatch.StartNew();
			foreach (DataGenerationRow dataGenerationRow in table.GetAllRows())
			{
				if (dataGenerationRow.Category == (int)category)
				{
					TGeneration generation = Activator.CreateInstance<TGeneration>();
					generation.Load(dataGenerationRow);
					this.AddGeneration(generation);
				}
			}
			this.ScheduleMaintenance();
			ExTraceGlobals.StorageTracer.TraceDebug<DataGenerationCategory, int, TimeSpan>((long)this.GetHashCode(), "Generation Manager for {0} created and loaded with {1} pre-existing generations in {2}.", category, this.generations.Count, stopwatch.Elapsed);
			ExTraceGlobals.StorageTracer.TracePerformance<DataGenerationCategory, int, TimeSpan>((long)this.GetHashCode(), "Generation Manager for {0} created and loaded with {1} pre-existing generations in {2}.", category, this.generations.Count, stopwatch.Elapsed);
			this.perfCounters.GenerationCount.IncrementBy((long)this.generations.Count);
		}

		internal Func<DateTime> ReferenceClock
		{
			get
			{
				return this.referenceClock;
			}
			set
			{
				this.referenceClock = value;
			}
		}

		public void SuspendDataCleanup()
		{
			this.expirationLock.EnterReadLock();
			Interlocked.Increment(ref this.suspendExpirationCount);
			this.expirationLock.ExitReadLock();
		}

		public void ResumeDataCleanup()
		{
			this.expirationLock.EnterReadLock();
			try
			{
				if (Interlocked.Decrement(ref this.suspendExpirationCount) < 0)
				{
					throw new InvalidOperationException("Cleanup was not suspended.");
				}
			}
			finally
			{
				this.expirationLock.ExitReadLock();
			}
		}

		public void SuspendDataCleanup(DateTime startDate, DateTime endDate)
		{
			this.expirationLock.EnterUpgradeableReadLock();
			this.suspendedExpirationPeriods.Add(new DataGenerationManager<TGeneration>.DatePeriod
			{
				Start = startDate,
				End = endDate
			});
			this.expirationLock.ExitUpgradeableReadLock();
		}

		public void ResumeDataCleanup(DateTime startDate, DateTime endDate)
		{
			this.expirationLock.EnterUpgradeableReadLock();
			try
			{
				if (!this.suspendedExpirationPeriods.Remove(new DataGenerationManager<TGeneration>.DatePeriod
				{
					Start = startDate,
					End = endDate
				}))
				{
					throw new InvalidOperationException("Cleanup was not suspended for this period.");
				}
			}
			finally
			{
				this.expirationLock.ExitUpgradeableReadLock();
			}
		}

		public TGeneration GetGeneration(int generationId)
		{
			this.generationsLock.EnterReadLock();
			TGeneration tgeneration;
			try
			{
				tgeneration = this.generations.Values.FirstOrDefault((TGeneration g) => g.GenerationId == generationId);
			}
			finally
			{
				this.generationsLock.ExitReadLock();
			}
			if (tgeneration != null)
			{
				tgeneration.Attach();
			}
			return tgeneration;
		}

		public TGeneration GetCurrentGeneration()
		{
			return this.GetGeneration(this.ReferenceClock());
		}

		public TGeneration GetGeneration(DateTime timeStamp)
		{
			DataGenerationManager<TGeneration>.GenerationKey genKey = new DataGenerationManager<TGeneration>.GenerationKey(this.GetGenerationStart(timeStamp), this.generationTimeSpan);
			TGeneration tgeneration = this.generationCache.Find((TGeneration gen) => gen.StartTime == genKey.StartTime && gen.Duration == genKey.Duration);
			if (tgeneration == null)
			{
				this.generationsLock.EnterReadLock();
				try
				{
					ExTraceGlobals.StorageTracer.TraceDebug<DateTime>((long)this.GetHashCode(), "Could not find generation for {0} in cache, looking in the list.", timeStamp);
					this.generations.TryGetValue(genKey, out tgeneration);
				}
				finally
				{
					this.generationsLock.ExitReadLock();
				}
				if (tgeneration == null)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<DateTime>((long)this.GetHashCode(), "Could not find generation for {0}, creating new one.", timeStamp);
					tgeneration = this.CreateGeneration(genKey);
				}
			}
			if (tgeneration != null)
			{
				tgeneration.Attach();
			}
			return tgeneration;
		}

		public TGeneration[] GetGenerations(DateTime startDate, DateTime endDate)
		{
			this.generationsLock.EnterReadLock();
			TGeneration[] array;
			try
			{
				array = (from gen in this.generations.Values
				where gen.Contains(startDate, endDate)
				select gen).ToArray<TGeneration>();
			}
			finally
			{
				this.generationsLock.ExitReadLock();
			}
			foreach (TGeneration tgeneration in array)
			{
				tgeneration.Attach();
			}
			ExTraceGlobals.StorageTracer.TraceDebug<DateTime, DateTime, int>((long)this.GetHashCode(), "GetGenerations for ({0},{1}) returned {2} generations.", startDate, endDate, array.Length);
			return array;
		}

		public TGeneration[] GetRecentGenerations()
		{
			DateTime dateTime = this.ReferenceClock();
			DateTime startDate = dateTime;
			if (this.recentGenerationCount > 0)
			{
				startDate = startDate.AddMilliseconds(-1.0 * this.generationTimeSpan.TotalMilliseconds * (double)(this.recentGenerationCount - 1));
			}
			else
			{
				startDate = DateTime.MinValue;
			}
			return this.GetGenerations(startDate, dateTime);
		}

		public void ExpireGenerations()
		{
			this.expirationLock.EnterWriteLock();
			try
			{
				if (this.suspendExpirationCount > 0)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<int>((long)this.GetHashCode(), "DataCleanup/Expiration was called but is disabled with {0} requests.", this.suspendExpirationCount);
				}
				else
				{
					DateTime now = this.ReferenceClock();
					this.generationsLock.EnterReadLock();
					TGeneration[] array;
					try
					{
						array = (from g in this.generations.Values.TakeWhile((TGeneration g) => now - g.EndTime > this.ageToCleanup())
						where !this.suspendedExpirationPeriods.Any((DataGenerationManager<TGeneration>.DatePeriod p) => g.Contains(p.Start, p.End))
						select g).ToArray<TGeneration>();
					}
					finally
					{
						this.generationsLock.ExitReadLock();
					}
					ExTraceGlobals.StorageTracer.TraceDebug<int>((long)this.GetHashCode(), "Expiring {0} generations.", array.Length);
					this.perfCounters.GenerationExpiredCount.RawValue = (long)array.Length;
					Stopwatch stopwatch = new Stopwatch();
					foreach (TGeneration generation in array)
					{
						stopwatch.Start();
						generation.Attach();
						GenerationCleanupMode generationCleanupMode = generation.Cleanup();
						if (generationCleanupMode == GenerationCleanupMode.Drop)
						{
							this.RemoveGeneration(generation);
							this.perfCounters.GenerationExpiredCount.Decrement();
						}
						stopwatch.Stop();
						ExTraceGlobals.StorageTracer.TracePerformance<TimeSpan, GenerationCleanupMode>((long)this.GetHashCode(), "Expiring generation took {0} mode = {1}", stopwatch.Elapsed, generationCleanupMode);
						this.perfCounters.GenerationLastCleanupLatency.RawValue = stopwatch.ElapsedMilliseconds;
						stopwatch.Reset();
					}
				}
			}
			finally
			{
				this.expirationLock.ExitWriteLock();
			}
		}

		public void Unload()
		{
			this.maintenanceTimer.Change(-1, -1);
			this.maintenanceTimer.Dispose(true);
			foreach (TGeneration tgeneration in this.generations.Values)
			{
				tgeneration.Unload();
				ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "Generation Manager detached table {0}", tgeneration.Name);
			}
		}

		public XElement GetDiagnosticInfo(string argument)
		{
			XElement xelement = new XElement("GenerationManager");
			xelement.Add(new XElement("SuspendAllExpirationCount", this.suspendExpirationCount));
			xelement.Add(new XElement("SuspendedExpirationPeriods", from p in this.suspendedExpirationPeriods
			select p.GetDiagnosticInfo()));
			if (argument.Equals("generations", StringComparison.InvariantCultureIgnoreCase))
			{
				foreach (TGeneration tgeneration in this.generations.Values)
				{
					xelement.Add(tgeneration.GetDiagnosticInfo(argument));
				}
			}
			return xelement;
		}

		private TGeneration CreateGeneration(DateTime timeStamp)
		{
			return this.CreateGeneration(new DataGenerationManager<TGeneration>.GenerationKey(this.GetGenerationStart(timeStamp), this.generationTimeSpan));
		}

		private TGeneration CreateGeneration(DataGenerationManager<TGeneration>.GenerationKey genKey)
		{
			this.generationsLock.EnterUpgradeableReadLock();
			TGeneration result;
			try
			{
				TGeneration tgeneration;
				this.generations.TryGetValue(genKey, out tgeneration);
				if (tgeneration == null)
				{
					tgeneration = Activator.CreateInstance<TGeneration>();
					tgeneration.Load(genKey.StartTime, genKey.StartTime + genKey.Duration, this.category, this.table);
					this.AddGeneration(tgeneration);
				}
				result = tgeneration;
			}
			finally
			{
				this.generationsLock.ExitUpgradeableReadLock();
			}
			return result;
		}

		private void ScheduleMaintenance()
		{
			if (this.autoExpireEnabled || this.autoCreateEnabled)
			{
				DateTime dateTime = this.ReferenceClock();
				TimeSpan timeSpan = this.GetGenerationStart(dateTime).Add(TimeSpan.FromTicks(this.generationTimeSpan.Ticks * 90L / 100L)).Subtract(dateTime);
				if (this.autoCreateEnabled)
				{
					this.CreateGeneration(dateTime);
					if (timeSpan.Ticks < 0L)
					{
						this.CreateGeneration(dateTime + this.generationTimeSpan);
					}
				}
				if (timeSpan.Ticks < 0L)
				{
					timeSpan = timeSpan.Add(this.generationTimeSpan);
				}
				this.maintenanceTimer.Change(timeSpan, this.generationTimeSpan);
				ExTraceGlobals.StorageTracer.TraceDebug<DataGenerationCategory, TimeSpan, TimeSpan>((long)this.GetHashCode(), "Generation manager ({0}) maintenance scheduled for {1} from now and will repeat itself every {2}", this.category, timeSpan, this.generationTimeSpan);
			}
		}

		private void DoMaintenance()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			if (this.autoCreateEnabled)
			{
				this.CreateGeneration(this.ReferenceClock() + this.generationTimeSpan);
			}
			if (this.autoExpireEnabled)
			{
				this.ExpireGenerations();
			}
			ExTraceGlobals.StorageTracer.TracePerformance<DataGenerationCategory, TimeSpan>((long)this.GetHashCode(), "Generation manager({0}) maintenance took {1}", this.category, stopwatch.Elapsed);
		}

		private DateTime GetGenerationStart(DateTime timeStamp)
		{
			return timeStamp.Subtract(TimeSpan.FromTicks(timeStamp.Ticks % this.generationTimeSpan.Ticks));
		}

		private void RemoveGeneration(TGeneration generation)
		{
			this.generationsLock.EnterWriteLock();
			try
			{
				this.generations.Remove(new DataGenerationManager<TGeneration>.GenerationKey(generation));
				this.generationCache.Remove(generation);
				this.perfCounters.GenerationCount.Decrement();
			}
			finally
			{
				this.generationsLock.ExitWriteLock();
			}
			generation.Unload();
		}

		private void AddGeneration(TGeneration generation)
		{
			this.generationsLock.EnterWriteLock();
			try
			{
				TransportHelpers.AttemptAddToDictionary<DataGenerationManager<TGeneration>.GenerationKey, TGeneration>(this.generations, new DataGenerationManager<TGeneration>.GenerationKey(generation), generation, null);
				this.generationCache.Add(generation);
				this.perfCounters.GenerationCount.Increment();
			}
			finally
			{
				this.generationsLock.ExitWriteLock();
			}
		}

		private const int AutoCreateAdvancePercentage = 90;

		private readonly Func<TimeSpan> ageToCleanup;

		private readonly DataGenerationCategory category;

		private readonly DataGenerationManager<TGeneration>.SimpleCache<TGeneration> generationCache = new DataGenerationManager<TGeneration>.SimpleCache<TGeneration>(3);

		private readonly SortedList<DataGenerationManager<TGeneration>.GenerationKey, TGeneration> generations = new SortedList<DataGenerationManager<TGeneration>.GenerationKey, TGeneration>();

		private readonly ReaderWriterLockSlim generationsLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		private readonly ReaderWriterLockSlim expirationLock = new ReaderWriterLockSlim();

		private readonly List<DataGenerationManager<TGeneration>.DatePeriod> suspendedExpirationPeriods = new List<DataGenerationManager<TGeneration>.DatePeriod>();

		private readonly DatabasePerfCountersInstance perfCounters = DatabasePerfCounters.GetInstance("other");

		private readonly GuardedTimer maintenanceTimer;

		private readonly TimeSpan generationTimeSpan;

		private readonly DataGenerationTable table;

		private readonly int recentGenerationCount;

		private readonly bool autoCreateEnabled;

		private readonly bool autoExpireEnabled;

		private int suspendExpirationCount;

		private Func<DateTime> referenceClock = () => DateTime.UtcNow;

		private class SimpleCache<T> where T : class
		{
			public SimpleCache(int size)
			{
				this.cachedInstances = new T[size];
			}

			public T Find(Func<T, bool> func)
			{
				return this.cachedInstances.FirstOrDefault((T item) => item != null && func(item));
			}

			public void Add(T item)
			{
				for (int i = this.cachedInstances.Length - 1; i >= 0; i--)
				{
					Interlocked.Exchange<T>(ref this.cachedInstances[i], (i == 0) ? item : this.cachedInstances[i - 1]);
				}
			}

			public void Remove(T item)
			{
				for (int i = 0; i < this.cachedInstances.Length; i++)
				{
					Interlocked.CompareExchange<T>(ref this.cachedInstances[i], default(T), item);
				}
			}

			private readonly T[] cachedInstances;
		}

		private struct GenerationKey : IComparable<DataGenerationManager<TGeneration>.GenerationKey>
		{
			public GenerationKey(DateTime start, TimeSpan duration)
			{
				this.StartTime = start;
				this.Duration = duration;
			}

			public GenerationKey(TGeneration gen)
			{
				this = new DataGenerationManager<TGeneration>.GenerationKey(gen.StartTime, gen.Duration);
			}

			public int CompareTo(DataGenerationManager<TGeneration>.GenerationKey other)
			{
				int num = this.StartTime.CompareTo(other.StartTime);
				if (num == 0)
				{
					return this.Duration.CompareTo(other.Duration);
				}
				return num;
			}

			public DateTime StartTime;

			public TimeSpan Duration;
		}

		private struct DatePeriod
		{
			public XElement GetDiagnosticInfo()
			{
				XElement xelement = new XElement("DatePeriod");
				xelement.SetAttributeValue("Start", this.Start);
				xelement.SetAttributeValue("End", this.End);
				return xelement;
			}

			public DateTime Start;

			public DateTime End;
		}
	}
}
