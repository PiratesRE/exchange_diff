using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NamedPropMap
	{
		internal NamedPropMap(Action<int> sizeChangedDelegate)
		{
			this.sizeChangedDelegate = sizeChangedDelegate;
		}

		internal static MiddleTierStoragePerformanceCountersInstance GetPerfCounters()
		{
			if (NamedPropMap.middleTierStoragePerformanceCountersInstanceName.Length == 0)
			{
				Process currentProcess = Process.GetCurrentProcess();
				NamedPropMap.middleTierStoragePerformanceCountersInstanceName = string.Format("{0} - {1}", currentProcess.ProcessName, currentProcess.Id);
			}
			return MiddleTierStoragePerformanceCounters.GetInstance(NamedPropMap.middleTierStoragePerformanceCountersInstanceName);
		}

		internal bool TryGetNamedPropFromPropId(ushort propId, out NamedProp namedProp)
		{
			bool result;
			try
			{
				this.lockObject.EnterReadLock();
				result = this.propIdToNamedPropMap.TryGetValue(propId, out namedProp);
			}
			finally
			{
				try
				{
					this.lockObject.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		internal bool TryGetPropIdFromNamedProp(NamedProp namedProp, out ushort propId)
		{
			bool result;
			try
			{
				this.lockObject.EnterReadLock();
				result = this.namedPropToPropIdMap.TryGetValue(namedProp, out propId);
			}
			finally
			{
				try
				{
					this.lockObject.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		internal int Size
		{
			get
			{
				return this.size;
			}
		}

		internal int UnregisterSizeChangedDelegate()
		{
			int result;
			lock (this.sizeChangedDelegateLock)
			{
				this.sizeChangedDelegate = null;
				result = this.size;
			}
			return result;
		}

		internal void AddMapping(bool addUnresolvedProperties, ICollection<ushort> propIds, ICollection<NamedProp> namedProps)
		{
			ICollection<NamedProp> collection = NamedPropertyDefinition.NamedPropertyKey.AddNamedProps(namedProps);
			try
			{
				this.lockObject.EnterWriteLock();
				IEnumerator<NamedProp> enumerator = collection.GetEnumerator();
				IEnumerator<ushort> enumerator2 = propIds.GetEnumerator();
				while (enumerator2.MoveNext() && enumerator.MoveNext())
				{
					NamedProp namedProp = enumerator.Current;
					if (namedProp != null)
					{
						ushort num = enumerator2.Current;
						if ((addUnresolvedProperties || num != 0) && !this.namedPropToPropIdMap.ContainsKey(namedProp))
						{
							int num2 = 0;
							this.namedPropToPropIdMap[namedProp] = num;
							this.perfCounters.NamedPropertyCacheEntries.Increment();
							num2++;
							if (num != 0)
							{
								this.propIdToNamedPropMap[num] = namedProp;
								this.perfCounters.NamedPropertyCacheEntries.Increment();
								num2++;
							}
							lock (this.sizeChangedDelegateLock)
							{
								if (this.sizeChangedDelegate != null)
								{
									this.sizeChangedDelegate(num2);
								}
								this.size += num2;
							}
						}
					}
				}
			}
			finally
			{
				try
				{
					this.lockObject.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		internal const ushort PropIdUnresolved = 0;

		private const int InitialMapSize = 100;

		private readonly MiddleTierStoragePerformanceCountersInstance perfCounters = NamedPropMap.GetPerfCounters();

		private static string middleTierStoragePerformanceCountersInstanceName = string.Empty;

		private Dictionary<NamedProp, ushort> namedPropToPropIdMap = new Dictionary<NamedProp, ushort>(100);

		private Dictionary<ushort, NamedProp> propIdToNamedPropMap = new Dictionary<ushort, NamedProp>(100);

		private int size;

		private Action<int> sizeChangedDelegate;

		private object sizeChangedDelegateLock = new object();

		private ReaderWriterLockSlim lockObject = new ReaderWriterLockSlim();
	}
}
