using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SafeInstanceTable<T> where T : IIdentityGuid
	{
		internal void AddInstance(T instance)
		{
			this.m_rwLock.AcquireWriterLock(-1);
			try
			{
				this.m_instances.Add(instance.Identity, instance);
			}
			finally
			{
				this.m_rwLock.ReleaseWriterLock();
			}
		}

		internal virtual void RemoveInstance(T instance)
		{
			this.m_rwLock.AcquireWriterLock(-1);
			try
			{
				this.m_instances.Remove(instance.Identity);
			}
			finally
			{
				this.m_rwLock.ReleaseWriterLock();
			}
		}

		internal void UpdateInstance(T oldInstance, T newInstance)
		{
			this.m_rwLock.AcquireWriterLock(-1);
			try
			{
				this.m_instances.Remove(oldInstance.Identity);
				this.m_instances.Add(newInstance.Identity, newInstance);
			}
			finally
			{
				this.m_rwLock.ReleaseWriterLock();
			}
		}

		internal bool TryGetInstance(string identity, out T instance)
		{
			this.m_rwLock.AcquireReaderLock(-1);
			bool result;
			try
			{
				if (this.m_instances.ContainsKey(identity))
				{
					instance = this.m_instances[identity];
					result = true;
				}
				else
				{
					instance = default(T);
					result = false;
				}
			}
			finally
			{
				this.m_rwLock.ReleaseReaderLock();
			}
			return result;
		}

		internal bool TryGetInstance(Guid guid, out T instance)
		{
			string identityFromGuid = SafeInstanceTable<T>.GetIdentityFromGuid(guid);
			return this.TryGetInstance(identityFromGuid, out instance);
		}

		internal T[] GetAllInstances()
		{
			this.m_rwLock.AcquireReaderLock(-1);
			T[] result;
			try
			{
				T[] array = new T[this.m_instances.Count];
				this.m_instances.Values.CopyTo(array, 0);
				result = array;
			}
			finally
			{
				this.m_rwLock.ReleaseReaderLock();
			}
			return result;
		}

		internal ICollection<T> GetAllInstancesUnsafe()
		{
			this.m_rwLock.AcquireReaderLock(-1);
			ICollection<T> values;
			try
			{
				values = this.m_instances.Values;
			}
			finally
			{
				this.m_rwLock.ReleaseReaderLock();
			}
			return values;
		}

		internal int Count
		{
			get
			{
				this.m_rwLock.AcquireReaderLock(-1);
				int count;
				try
				{
					count = this.m_instances.Count;
				}
				finally
				{
					this.m_rwLock.ReleaseReaderLock();
				}
				return count;
			}
		}

		public static string GetIdentityFromGuid(Guid guid)
		{
			return guid.ToString().ToLowerInvariant();
		}

		protected ReaderWriterLock m_rwLock = new ReaderWriterLock();

		protected Dictionary<string, T> m_instances = new Dictionary<string, T>();
	}
}
