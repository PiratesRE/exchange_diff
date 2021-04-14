using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal abstract class ServiceProxyPool<TClient> : IServiceProxyPool<TClient>, IDisposeTrackable, IDisposable
	{
		protected ServiceProxyPool(string endpointName, string hostName, int maxNumberOfClientProxies, ChannelFactory<TClient> channelFactory, bool useDisposeTracker) : this(endpointName, hostName, maxNumberOfClientProxies, useDisposeTracker)
		{
			ArgumentValidator.ThrowIfNull("channelFactory", channelFactory);
			this.channelFactoryList = new List<ChannelFactory<TClient>>
			{
				channelFactory
			};
		}

		protected ServiceProxyPool(string endpointName, string hostName, int maxNumberOfClientProxies, List<ChannelFactory<TClient>> channelFactoryList, bool useDisposeTracker) : this(endpointName, hostName, maxNumberOfClientProxies, useDisposeTracker)
		{
			ArgumentValidator.ThrowIfNull("channelFactoryList", channelFactoryList);
			ArgumentValidator.ThrowIfZeroOrNegative("channelFactoryList.Count", channelFactoryList.Count);
			this.channelFactoryList = channelFactoryList;
		}

		private ServiceProxyPool(string endpointName, string hostName, int maxNumberOfClientProxies, bool useDisposeTracker)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("endpointName", endpointName);
			ArgumentValidator.ThrowIfNullOrEmpty("hostName", hostName);
			this.maxNumberOfClientProxies = maxNumberOfClientProxies;
			this.EndpointName = endpointName;
			this.TargetInfo = string.Format("{0} ({1})", endpointName, hostName);
			this.pool = new ConcurrentStack<ServiceProxyPool<TClient>.WCFConnectionStateTuple>();
			this.outstandingProxies = 0L;
			if (useDisposeTracker)
			{
				this.disposeTracker = this.GetDisposeTracker();
			}
			this.counters = ServiceProxyPoolCounters.GetInstance(string.Format("{0} {1}", endpointName, Process.GetCurrentProcess().ProcessName));
		}

		protected ChannelFactory<TClient> ChannelFactory
		{
			get
			{
				return this.channelFactoryList[0];
			}
		}

		private protected string EndpointName { protected get; private set; }

		private protected string TargetInfo { protected get; private set; }

		protected abstract Microsoft.Exchange.Diagnostics.Trace Tracer { get; }

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ServiceProxyPool<TClient>>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Tracer.TraceDebug<int, string>((long)this.GetHashCode(), "Disposing of ServiceProxyPool instance {0} for {1}", this.GetHashCode(), this.EndpointName);
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			while (!this.pool.IsEmpty)
			{
				ServiceProxyPool<TClient>.WCFConnectionStateTuple wcfconnectionStateTuple = null;
				if (this.pool.TryPop(out wcfconnectionStateTuple))
				{
					WcfUtils.DisposeWcfClientGracefully((ICommunicationObject)((object)wcfconnectionStateTuple.Client), false);
					this.counters.ProxyInstanceCount.Decrement();
				}
			}
			foreach (ChannelFactory<TClient> channelFactory in this.channelFactoryList)
			{
				if (channelFactory != null)
				{
					WcfUtils.DisposeWcfClientGracefully(channelFactory, false);
				}
			}
			GC.SuppressFinalize(this);
		}

		internal void CallServiceWithRetry(Action<IPooledServiceProxy<TClient>> action, string debugMessage, int numberOfRetries = 3)
		{
			this.CallServiceWithRetry(action, debugMessage, null, numberOfRetries, false);
		}

		internal void CallServiceWithRetryAsyncBegin(Action<IPooledServiceProxy<TClient>> action, string debugMessage, int numberOfRetries = 3)
		{
			this.CallServiceWithRetry(action, debugMessage, null, numberOfRetries, true);
		}

		internal void CallServiceWithRetryAsyncEnd(IPooledServiceProxy<TClient> cachedProxy, Action<IPooledServiceProxy<TClient>> action, string debugMessage)
		{
			ArgumentValidator.ThrowIfNull("cachedProxy", cachedProxy);
			ServiceProxyPool<TClient>.WCFConnectionStateTuple wcfconnectionStateTuple = cachedProxy as ServiceProxyPool<TClient>.WCFConnectionStateTuple;
			ArgumentValidator.ThrowIfNull("proxyToUse", wcfconnectionStateTuple);
			this.CallServiceWithRetry(action, debugMessage, wcfconnectionStateTuple, 1, false);
		}

		public bool TryCallServiceWithRetryAsyncBegin(Action<IPooledServiceProxy<TClient>> action, string debugMessage, int numberOfRetries, out Exception exception)
		{
			return this.TryCallServiceWithRetry(action, debugMessage, null, numberOfRetries, true, out exception);
		}

		public bool TryCallServiceWithRetryAsyncEnd(IPooledServiceProxy<TClient> cachedProxy, Action<IPooledServiceProxy<TClient>> action, string debugMessage, out Exception exception)
		{
			ArgumentValidator.ThrowIfNull("cachedProxy", cachedProxy);
			ServiceProxyPool<TClient>.WCFConnectionStateTuple wcfconnectionStateTuple = cachedProxy as ServiceProxyPool<TClient>.WCFConnectionStateTuple;
			ArgumentValidator.ThrowIfNull("proxyToUse", wcfconnectionStateTuple);
			return this.TryCallServiceWithRetry(action, debugMessage, wcfconnectionStateTuple, 1, false, out exception);
		}

		protected abstract Exception GetTransientWrappedException(Exception wcfException);

		protected abstract Exception GetPermanentWrappedException(Exception wcfException);

		protected abstract void LogCallServiceError(Exception error, string priodicKey, string debugMessage, int numberOfRetries);

		protected virtual bool IsTransientException(Exception ex)
		{
			ArgumentValidator.ThrowIfNull("ex", ex);
			return ServiceProxyPool<TClient>.transientExceptions.Contains(ex.GetType());
		}

		private Exception HandleException(Exception ex, string debugMessage)
		{
			if (this.IsTransientException(ex))
			{
				this.Tracer.TraceError<string, string, string>((long)this.GetHashCode(), "{0} got WCF {1} exception {2}. Transient Exception.", debugMessage, ex.GetType().Name, ex.ToString());
				return this.GetTransientWrappedException(ex);
			}
			this.Tracer.TraceError<string, string, string>((long)this.GetHashCode(), "{0} got WCF {1} exception {2}. Permanent Exception.", debugMessage, ex.GetType().Name, ex.ToString());
			return this.GetPermanentWrappedException(ex);
		}

		private void CallServiceWithRetry(Action<IPooledServiceProxy<TClient>> action, string debugMessage, ServiceProxyPool<TClient>.WCFConnectionStateTuple proxyToUse, int numberOfRetries, bool doNotReturnProxyOnSuccess)
		{
			Exception ex;
			if (!this.TryCallServiceWithRetry(action, debugMessage, proxyToUse, numberOfRetries, doNotReturnProxyOnSuccess, out ex))
			{
				throw ex;
			}
		}

		private bool TryCallServiceWithRetry(Action<IPooledServiceProxy<TClient>> action, string debugMessage, ServiceProxyPool<TClient>.WCFConnectionStateTuple proxyToUse, int numberOfRetries, bool doNotReturnProxyOnSuccess, out Exception exception)
		{
			ArgumentValidator.ThrowIfNull("action", action);
			Stopwatch stopwatch = Stopwatch.StartNew();
			exception = null;
			int num = numberOfRetries;
			debugMessage = (string.IsNullOrEmpty(debugMessage) ? string.Empty : debugMessage);
			Exception ex;
			Exception error;
			for (;;)
			{
				ServiceProxyPool<TClient>.WCFConnectionStateTuple wcfconnectionStateTuple = null;
				ex = null;
				error = null;
				bool flag = false;
				try
				{
					wcfconnectionStateTuple = (proxyToUse ?? this.GetClient(numberOfRetries - num, ref flag, num == numberOfRetries || num > 1));
					this.counters.NumberOfCalls.Increment();
					action(wcfconnectionStateTuple);
					wcfconnectionStateTuple.LastUsed = DateTime.UtcNow;
				}
				catch (Exception ex2)
				{
					error = ex2;
					ex = this.HandleException(ex2, debugMessage);
				}
				finally
				{
					if (wcfconnectionStateTuple != null)
					{
						if (ex == null)
						{
							if (!doNotReturnProxyOnSuccess)
							{
								if (!flag)
								{
									this.ReturnClientToPool(wcfconnectionStateTuple);
									this.DecrementOutstandingProxies(debugMessage);
								}
								else
								{
									WcfUtils.DisposeWcfClientGracefully((ICommunicationObject)((object)wcfconnectionStateTuple.Client), false);
									wcfconnectionStateTuple = null;
									this.counters.ProxyInstanceCount.Decrement();
								}
							}
						}
						else
						{
							WcfUtils.DisposeWcfClientGracefully((ICommunicationObject)((object)wcfconnectionStateTuple.Client), false);
							wcfconnectionStateTuple = null;
							this.counters.ProxyInstanceCount.Decrement();
							this.DecrementOutstandingProxies(debugMessage);
						}
					}
					num--;
				}
				stopwatch.Stop();
				this.counters.AverageLatency.IncrementBy(stopwatch.ElapsedMilliseconds);
				this.counters.AverageLatencyBase.Increment();
				if (ex == null)
				{
					break;
				}
				if (0 >= num || !(ex is TransientException))
				{
					goto IL_14C;
				}
			}
			return true;
			IL_14C:
			this.LogCallServiceError(error, this.TargetInfo, debugMessage, numberOfRetries - num);
			exception = ex;
			return false;
		}

		private ServiceProxyPool<TClient>.WCFConnectionStateTuple GetClient(int retry, ref bool doNotReturnProxyAfterRetry, bool useCache = true)
		{
			ServiceProxyPool<TClient>.WCFConnectionStateTuple wcfconnectionStateTuple = null;
			if (retry > 0 && this.channelFactoryList.Count > retry)
			{
				useCache = false;
				doNotReturnProxyAfterRetry = true;
			}
			while (useCache && !this.pool.IsEmpty && this.pool.TryPop(out wcfconnectionStateTuple) && !this.IsWCFClientUsable(wcfconnectionStateTuple))
			{
				WcfUtils.DisposeWcfClientGracefully((ICommunicationObject)((object)wcfconnectionStateTuple.Client), false);
				this.counters.ProxyInstanceCount.Decrement();
				wcfconnectionStateTuple = null;
			}
			ICommunicationObject communicationObject;
			if (wcfconnectionStateTuple == null)
			{
				wcfconnectionStateTuple = new ServiceProxyPool<TClient>.WCFConnectionStateTuple();
				wcfconnectionStateTuple.Client = this.GetChannelFactory(retry).CreateChannel();
				communicationObject = (ICommunicationObject)((object)wcfconnectionStateTuple.Client);
				communicationObject.Open();
				this.counters.ProxyInstanceCount.Increment();
			}
			else
			{
				communicationObject = (ICommunicationObject)((object)wcfconnectionStateTuple.Client);
			}
			if (communicationObject.State != CommunicationState.Opened)
			{
				this.Tracer.TraceDebug((long)this.GetHashCode(), "Channel is created but not in opened state. Will wait and retry.");
				int num = 3;
				while (communicationObject.State != CommunicationState.Opened)
				{
					Thread.Sleep(500);
					num--;
					if (num == 0)
					{
						break;
					}
				}
				if (communicationObject.State != CommunicationState.Opened)
				{
					this.Tracer.TraceError<int>((long)this.GetHashCode(), "Channel cannot be opened with {0} retries.", 3);
				}
				else
				{
					this.Tracer.TraceDebug<int>((long)this.GetHashCode(), "Channel was opened with {0} retries.", 3 - num);
				}
			}
			long arg = Interlocked.Increment(ref this.outstandingProxies);
			this.counters.OutstandingCalls.Increment();
			this.Tracer.TraceDebug<long>((long)this.GetHashCode(), "ServiceProxyPool.GetClient: Outstanding Proxies are {0}", arg);
			return wcfconnectionStateTuple;
		}

		private ChannelFactory<TClient> GetChannelFactory(int retry)
		{
			if (this.channelFactoryList.Count - 1 < retry)
			{
				return this.channelFactoryList[this.channelFactoryList.Count - 1];
			}
			return this.channelFactoryList[retry];
		}

		private void ReturnClientToPool(ServiceProxyPool<TClient>.WCFConnectionStateTuple clientInfo)
		{
			ArgumentValidator.ThrowIfNull("clientInfo", clientInfo);
			if (this.IsWCFClientUsable(clientInfo) && this.pool.Count <= this.maxNumberOfClientProxies)
			{
				this.pool.Push(clientInfo);
				return;
			}
			WcfUtils.DisposeWcfClientGracefully((ICommunicationObject)((object)clientInfo.Client), false);
			this.counters.ProxyInstanceCount.Decrement();
		}

		private bool IsWCFClientUsable(ServiceProxyPool<TClient>.WCFConnectionStateTuple clientInfo)
		{
			ArgumentValidator.ThrowIfNull("clientInfo", clientInfo);
			if (DateTime.UtcNow - clientInfo.LastUsed >= ServiceProxyPool<TClient>.ClientMaximumLifetime)
			{
				return false;
			}
			CommunicationState state = ((ICommunicationObject)((object)clientInfo.Client)).State;
			return state != CommunicationState.Closing && state != CommunicationState.Closed && state != CommunicationState.Faulted;
		}

		private void DecrementOutstandingProxies(string debugMessage)
		{
			long arg = Interlocked.Decrement(ref this.outstandingProxies);
			this.counters.OutstandingCalls.Decrement();
			this.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "ServiceProxyPool.DecrementOutstandingProxies: {0} Outstanding Proxies are {1}", debugMessage, arg);
		}

		private const int TotalRetries = 3;

		internal static readonly TimeSpan DefaultInactivityTimeout = TimeSpan.FromMinutes(10.0);

		private static readonly TimeSpan ClientMaximumLifetime = ServiceProxyPool<TClient>.DefaultInactivityTimeout.Subtract(TimeSpan.FromSeconds(30.0));

		private static readonly Type[] transientExceptions = new Type[]
		{
			typeof(TimeoutException),
			typeof(EndpointNotFoundException),
			typeof(FaultException<ExceptionDetail>),
			typeof(CommunicationException),
			typeof(InvalidOperationException)
		};

		private readonly int maxNumberOfClientProxies;

		private DisposeTracker disposeTracker;

		private ConcurrentStack<ServiceProxyPool<TClient>.WCFConnectionStateTuple> pool;

		private List<ChannelFactory<TClient>> channelFactoryList;

		private long outstandingProxies;

		private ServiceProxyPoolCountersInstance counters;

		private class WCFConnectionStateTuple : IPooledServiceProxy<TClient>
		{
			public TClient Client { get; set; }

			public DateTime LastUsed { get; set; }

			public string Tag { get; set; }
		}
	}
}
