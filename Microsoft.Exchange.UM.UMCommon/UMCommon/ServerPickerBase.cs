using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class ServerPickerBase<ManagedObjectType, ContextObjectType> : IServerPicker<ManagedObjectType, ContextObjectType> where ManagedObjectType : class
	{
		protected ServerPickerBase() : this(ServerPickerBase<ManagedObjectType, ContextObjectType>.DefaultRetryInterval, ServerPickerBase<ManagedObjectType, ContextObjectType>.DefaultRefreshInterval, ServerPickerBase<ManagedObjectType, ContextObjectType>.DefaultRefreshIntervalOnFailure, ExTraceGlobals.UtilTracer)
		{
		}

		protected ServerPickerBase(TimeSpan retryInterval, TimeSpan refreshInterval, TimeSpan refreshIntervalOnFailure, Trace tracer)
		{
			this.tracer = tracer;
			this.refreshInterval = refreshInterval;
			this.refreshIntervalOnFailure = refreshIntervalOnFailure;
			this.servers = new List<ManagedObjectType>();
			this.unavailableServers = new RetryQueue<ManagedObjectType>(tracer, retryInterval);
		}

		internal ServerPickerBase<ManagedObjectType, ContextObjectType>.LoadConfigurationDelegate InternalLoadConfigurationDelegate
		{
			get
			{
				return this.loadConfigurationDelegate;
			}
			set
			{
				this.loadConfigurationDelegate = value;
			}
		}

		internal ServerPickerBase<ManagedObjectType, ContextObjectType>.IsHealthyDelegate InternalIsHealthyDelegate
		{
			get
			{
				return this.healthyDelegate;
			}
			set
			{
				this.healthyDelegate = value;
			}
		}

		internal ServerPickerBase<ManagedObjectType, ContextObjectType>.IsValidDelegate InternalIsValidDelegate
		{
			get
			{
				return this.validDelegate;
			}
			set
			{
				this.validDelegate = value;
			}
		}

		protected Trace Tracer
		{
			get
			{
				return this.tracer;
			}
		}

		protected TimeSpan RetryInterval
		{
			get
			{
				return this.unavailableServers.RetryInterval;
			}
			set
			{
				this.unavailableServers.RetryInterval = value;
			}
		}

		private bool TimeToRefresh
		{
			get
			{
				return ExDateTime.Compare(ExDateTime.UtcNow, this.nextRefresh) >= 0;
			}
		}

		public List<ManagedObjectType> GetRegisteredUMServers()
		{
			lock (this.lockObject)
			{
				if (this.TimeToRefresh)
				{
					this.RefreshServers();
				}
			}
			return this.servers;
		}

		public virtual ManagedObjectType PickNextServer(ContextObjectType context)
		{
			int num = 0;
			return this.PickNextServer(context, out num);
		}

		public virtual ManagedObjectType PickNextServer(ContextObjectType context, out int totalServers)
		{
			totalServers = 0;
			List<ManagedObjectType> list = new List<ManagedObjectType>();
			List<ManagedObjectType> list2 = new List<ManagedObjectType>();
			ManagedObjectType managedObjectType = default(ManagedObjectType);
			lock (this.lockObject)
			{
				if (this.TimeToRefresh)
				{
					this.RefreshServers();
				}
				this.UpdateRetryQueue();
				totalServers = this.servers.Count + this.unavailableServers.Count;
				if (this.servers.Count == 0)
				{
					CallIdTracer.TraceError(this.Tracer, this.GetHashCode(), "ServerPickerBase::PickNextServer() No servers found", new object[0]);
					return default(ManagedObjectType);
				}
				if (this.currentServerIndex >= this.servers.Count)
				{
					this.currentServerIndex = 0;
				}
				for (int i = 0; i < this.servers.Count; i++)
				{
					list.Add(this.servers[(this.currentServerIndex + i) % this.servers.Count]);
				}
				this.unavailableServers.CopyTo(list2);
				this.currentServerIndex++;
			}
			for (int j = 0; j < list.Count; j++)
			{
				managedObjectType = list[j];
				if (this.CanUse(context, managedObjectType))
				{
					return managedObjectType;
				}
			}
			for (int k = 0; k < list2.Count; k++)
			{
				managedObjectType = list2[k];
				if (this.CanUse(context, managedObjectType))
				{
					return managedObjectType;
				}
			}
			return default(ManagedObjectType);
		}

		public virtual ManagedObjectType PickNextServer(ContextObjectType context, Guid tenantGuid, out int totalServers)
		{
			return this.PickNextServer(context, out totalServers);
		}

		public virtual void ServerUnavailable(ManagedObjectType server)
		{
			CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "ServerPickerBase::ServerUnavailable {0}", new object[]
			{
				server
			});
			lock (this.lockObject)
			{
				if (this.servers.Remove(server))
				{
					this.unavailableServers.Enqueue(server);
					CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "Server {0} being added to unavailable list", new object[]
					{
						server
					});
				}
				if (this.servers.Count == 0)
				{
					CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "We don't have any more good servers. Trying to promote a bad server now...", new object[0]);
					ManagedObjectType managedObjectType = this.unavailableServers.Dequeue(true);
					if (managedObjectType != null)
					{
						CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "Server {0} being promoted from Unavailable to Active because we don't have any other good servers!", new object[]
						{
							managedObjectType
						});
						this.servers.Add(managedObjectType);
					}
				}
			}
		}

		internal void ServerInvalid(ManagedObjectType server)
		{
			CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "ServerPiclerBase::ServerInvalid {0}", new object[]
			{
				server
			});
			lock (this.lockObject)
			{
				this.servers.Remove(server);
				this.unavailableServers.Remove(server);
			}
		}

		protected abstract List<ManagedObjectType> LoadConfiguration();

		protected virtual bool IsHealthy(ContextObjectType context, ManagedObjectType item)
		{
			return true;
		}

		protected virtual bool IsValid(ContextObjectType context, ManagedObjectType candidate)
		{
			return true;
		}

		protected bool ServerInRetry(ManagedObjectType server)
		{
			CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "ServerPickerBase::ServerInRetry Server = {0}", new object[]
			{
				server
			});
			lock (this.lockObject)
			{
				if (this.unavailableServers.Contains(server))
				{
					return true;
				}
			}
			return false;
		}

		protected bool RemoveServer(ManagedObjectType server)
		{
			CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "ServerPickerBase::RemoveServer Server = {0}", new object[]
			{
				server
			});
			bool result = false;
			lock (this.lockObject)
			{
				result = this.servers.Remove(server);
			}
			return result;
		}

		protected void RefreshServers()
		{
			Exception ex = null;
			CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "ServerPickerBase::RefreshServers", new object[0]);
			lock (this.lockObject)
			{
				CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "ServerPickerBase::RefreshServers. TimeToRefresh", new object[0]);
				try
				{
					this.nextRefresh = ExDateTime.UtcNow.Add(this.refreshInterval);
					List<ManagedObjectType> list = this.InternalLoadConfiguration();
					this.unavailableServers.DeleteInvalid(list);
					List<ManagedObjectType> list2 = new List<ManagedObjectType>();
					foreach (ManagedObjectType managedObjectType in list)
					{
						if (!this.ServerInRetry(managedObjectType))
						{
							CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "Adding {0} to the list", new object[]
							{
								managedObjectType
							});
							list2.Add(managedObjectType);
						}
						else
						{
							CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "{0} is in retry, skipping", new object[]
							{
								managedObjectType
							});
						}
					}
					this.servers = list2;
				}
				catch (ADTransientException ex2)
				{
					ex = ex2;
				}
				catch (ADOperationException ex3)
				{
					ex = ex3;
				}
				catch (DataValidationException ex4)
				{
					ex = ex4;
				}
				catch (ExClusTransientException ex5)
				{
					ex = ex5;
				}
				if (ex != null)
				{
					this.nextRefresh = ExDateTime.UtcNow.Add(this.refreshIntervalOnFailure);
					CallIdTracer.TraceError(this.Tracer, this.GetHashCode(), "LoadConfiguration failed. We will retry at {0}: Error={1}", new object[]
					{
						this.nextRefresh,
						ex
					});
				}
			}
		}

		private bool CanUse(ContextObjectType context, ManagedObjectType nextServer)
		{
			CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "ServerPickerBase::CanUse() Found {0} as the next server", new object[]
			{
				nextServer
			});
			if (!this.InternalIsValid(context, nextServer))
			{
				CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "ServerPickerBase::IsValid {0} returned false", new object[]
				{
					nextServer
				});
			}
			else
			{
				if (this.InternalIsHealthy(context, nextServer))
				{
					return true;
				}
				this.ServerUnavailable(nextServer);
			}
			return false;
		}

		private bool InternalIsHealthy(ContextObjectType context, ManagedObjectType item)
		{
			if (this.InternalIsHealthyDelegate != null)
			{
				return this.InternalIsHealthyDelegate(context, item);
			}
			return this.IsHealthy(context, item);
		}

		private bool InternalIsValid(ContextObjectType context, ManagedObjectType candidate)
		{
			if (this.InternalIsValidDelegate != null)
			{
				return this.InternalIsValidDelegate(context, candidate);
			}
			return this.IsValid(context, candidate);
		}

		private List<ManagedObjectType> InternalLoadConfiguration()
		{
			if (this.InternalLoadConfigurationDelegate != null)
			{
				return this.InternalLoadConfigurationDelegate();
			}
			return this.LoadConfiguration();
		}

		private void UpdateRetryQueue()
		{
			ManagedObjectType managedObjectType = default(ManagedObjectType);
			do
			{
				managedObjectType = this.unavailableServers.Dequeue();
				if (managedObjectType != null)
				{
					CallIdTracer.TraceDebug(this.Tracer, this.GetHashCode(), "Server {0} being moved from retry to active", new object[]
					{
						managedObjectType
					});
					this.servers.Add(managedObjectType);
				}
			}
			while (managedObjectType != null);
		}

		internal static readonly TimeSpan DefaultRetryInterval = TimeSpan.FromMinutes(5.0);

		internal static readonly TimeSpan DefaultRefreshInterval = TimeSpan.FromMinutes(5.0);

		internal static readonly TimeSpan DefaultRefreshIntervalOnFailure = TimeSpan.FromMinutes(1.0);

		private ServerPickerBase<ManagedObjectType, ContextObjectType>.LoadConfigurationDelegate loadConfigurationDelegate;

		private ServerPickerBase<ManagedObjectType, ContextObjectType>.IsHealthyDelegate healthyDelegate;

		private ServerPickerBase<ManagedObjectType, ContextObjectType>.IsValidDelegate validDelegate;

		private List<ManagedObjectType> servers;

		private RetryQueue<ManagedObjectType> unavailableServers;

		private int currentServerIndex;

		private ExDateTime nextRefresh = ExDateTime.UtcNow;

		private TimeSpan refreshInterval;

		private TimeSpan refreshIntervalOnFailure;

		private Trace tracer;

		private object lockObject = new object();

		internal delegate List<ManagedObjectType> LoadConfigurationDelegate();

		internal delegate bool IsHealthyDelegate(ContextObjectType context, ManagedObjectType server);

		internal delegate bool IsValidDelegate(ContextObjectType context, ManagedObjectType server);
	}
}
