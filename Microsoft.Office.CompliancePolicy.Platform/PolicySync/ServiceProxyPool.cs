using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal class ServiceProxyPool<TClient> : IServiceProxyPool<TClient>, IDisposable
	{
		public ServiceProxyPool(string endpointName, string hostName, uint maxNumberOfClientProxies, ChannelFactory<TClient> channelFactory, ExecutionLog logProvider)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("endpointName", endpointName);
			ArgumentValidator.ThrowIfNullOrEmpty("hostName", hostName);
			ArgumentValidator.ThrowIfNull("channelFactory", channelFactory);
			ArgumentValidator.ThrowIfNull("logProvider", logProvider);
			this.ChannelFactory = channelFactory;
			this.MaxNumberOfClientProxies = maxNumberOfClientProxies;
			this.EndpointName = endpointName;
			this.TargetInfo = string.Format("{0} ({1})", endpointName, hostName);
			this.outstandingProxies = 0L;
			this.logProvider = logProvider;
		}

		public uint MaxNumberOfClientProxies { get; set; }

		private protected ChannelFactory<TClient> ChannelFactory { protected get; private set; }

		private protected string EndpointName { protected get; private set; }

		private protected string TargetInfo { protected get; private set; }

		public void Dispose()
		{
			while (!this.pool.IsEmpty)
			{
				WCFConnectionStateTuple<TClient> wcfconnectionStateTuple = null;
				if (this.pool.TryPop(out wcfconnectionStateTuple))
				{
					PolicySyncUtils.DisposeWcfClientGracefully((ICommunicationObject)((object)wcfconnectionStateTuple.Client), this.logProvider, false);
				}
			}
			if (this.ChannelFactory != null)
			{
				PolicySyncUtils.DisposeWcfClientGracefully(this.ChannelFactory, this.logProvider, false);
			}
			GC.SuppressFinalize(this);
		}

		public bool TryCallServiceWithRetryAsyncBegin(Action<IPooledServiceProxy<TClient>> action, string debugMessage, int numberOfRetries, out Exception exception)
		{
			return this.TryCallServiceWithRetry(action, debugMessage, null, numberOfRetries, true, out exception);
		}

		public bool TryCallServiceWithRetryAsyncEnd(IPooledServiceProxy<TClient> cachedProxy, Action<IPooledServiceProxy<TClient>> action, string debugMessage, out Exception exception)
		{
			ArgumentValidator.ThrowIfNull("cachedProxy", cachedProxy);
			WCFConnectionStateTuple<TClient> wcfconnectionStateTuple = cachedProxy as WCFConnectionStateTuple<TClient>;
			ArgumentValidator.ThrowIfNull("proxyToUse", wcfconnectionStateTuple);
			return this.TryCallServiceWithRetry(action, debugMessage, wcfconnectionStateTuple, 1, false, out exception);
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
			WCFConnectionStateTuple<TClient> wcfconnectionStateTuple = cachedProxy as WCFConnectionStateTuple<TClient>;
			ArgumentValidator.ThrowIfNull("proxyToUse", wcfconnectionStateTuple);
			this.CallServiceWithRetry(action, debugMessage, wcfconnectionStateTuple, 1, false);
		}

		protected SyncAgentTransientException GetTransientWrappedException(FaultException<PolicySyncTransientFault> transientFault)
		{
			ArgumentValidator.ThrowIfNull("transientFault", transientFault);
			return new SyncAgentTransientException(transientFault.Detail.Message, transientFault.InnerException, transientFault.Detail.IsPerObjectException, SyncAgentErrorCode.Generic);
		}

		protected SyncAgentPermanentException GetPermanentWrappedException(FaultException<PolicySyncPermanentFault> permanentFault)
		{
			ArgumentValidator.ThrowIfNull("permanentFault", permanentFault);
			return new SyncAgentPermanentException(permanentFault.Detail.Message, permanentFault.InnerException, permanentFault.Detail.IsPerObjectException, SyncAgentErrorCode.Generic);
		}

		protected void LogCallServiceError(Exception error, string periodicKey, string debugMessage, int numberOfRetries)
		{
			PolicySyncUtils.ServiceProxyPoolErrorData serviceProxyPoolErrorData = new PolicySyncUtils.ServiceProxyPoolErrorData(periodicKey, debugMessage, numberOfRetries);
			this.logProvider.LogOneEntry(this.ToString(), null, ExecutionLog.EventType.Error, serviceProxyPoolErrorData.ToString(), error);
		}

		private static bool IsUsable(CommunicationState clientState)
		{
			return clientState != CommunicationState.Closing && clientState != CommunicationState.Closed && clientState != CommunicationState.Faulted;
		}

		private SyncAgentExceptionBase HandleException(Exception exception, string debugMessage)
		{
			if (exception is FaultException<PolicySyncTransientFault>)
			{
				return this.GetTransientWrappedException(exception as FaultException<PolicySyncTransientFault>);
			}
			if (exception is FaultException<PolicySyncPermanentFault>)
			{
				return this.GetPermanentWrappedException(exception as FaultException<PolicySyncPermanentFault>);
			}
			return new SyncAgentPermanentException("Unexpected exception occured, considered by default a permanent exception.", exception);
		}

		private void CallServiceWithRetry(Action<IPooledServiceProxy<TClient>> action, string debugMessage, WCFConnectionStateTuple<TClient> proxyToUse, int numberOfRetries, bool doNotReturnProxyOnSuccess)
		{
			Exception ex;
			if (!this.TryCallServiceWithRetry(action, debugMessage, proxyToUse, numberOfRetries, doNotReturnProxyOnSuccess, out ex))
			{
				throw ex;
			}
		}

		private bool TryCallServiceWithRetry(Action<IPooledServiceProxy<TClient>> action, string debugMessage, WCFConnectionStateTuple<TClient> proxyToUse, int numberOfRetries, bool doNotReturnProxyOnSuccess, out Exception exception)
		{
			ArgumentValidator.ThrowIfNull("action", action);
			exception = null;
			int num = numberOfRetries;
			debugMessage = (string.IsNullOrEmpty(debugMessage) ? string.Empty : debugMessage);
			Exception ex;
			Exception error;
			for (;;)
			{
				WCFConnectionStateTuple<TClient> wcfconnectionStateTuple = null;
				ex = null;
				error = null;
				try
				{
					wcfconnectionStateTuple = (proxyToUse ?? this.GetClient(num == numberOfRetries || num > 1));
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
								this.ReturnClientToPool(wcfconnectionStateTuple);
								this.DecrementOutstandingProxies(debugMessage);
							}
						}
						else
						{
							PolicySyncUtils.DisposeWcfClientGracefully((ICommunicationObject)((object)wcfconnectionStateTuple.Client), this.logProvider, false);
							wcfconnectionStateTuple = null;
							this.DecrementOutstandingProxies(debugMessage);
						}
					}
					num--;
				}
				if (ex == null)
				{
					break;
				}
				if (0 >= num || !(ex is SyncAgentTransientException))
				{
					goto IL_C2;
				}
			}
			return true;
			IL_C2:
			this.LogCallServiceError(error, this.TargetInfo, debugMessage, num);
			exception = ex;
			return false;
		}

		private WCFConnectionStateTuple<TClient> GetClient(bool useCache = true)
		{
			WCFConnectionStateTuple<TClient> wcfconnectionStateTuple = null;
			while (useCache && !this.pool.IsEmpty && this.pool.TryPop(out wcfconnectionStateTuple) && !this.IsWCFClientUsable(wcfconnectionStateTuple))
			{
				PolicySyncUtils.DisposeWcfClientGracefully((ICommunicationObject)((object)wcfconnectionStateTuple.Client), this.logProvider, false);
				wcfconnectionStateTuple = null;
			}
			ICommunicationObject communicationObject;
			if (wcfconnectionStateTuple == null)
			{
				wcfconnectionStateTuple = new WCFConnectionStateTuple<TClient>();
				wcfconnectionStateTuple.Client = this.ChannelFactory.CreateChannel();
				communicationObject = (ICommunicationObject)((object)wcfconnectionStateTuple.Client);
				communicationObject.Open();
			}
			else
			{
				communicationObject = (ICommunicationObject)((object)wcfconnectionStateTuple.Client);
			}
			if (communicationObject.State != CommunicationState.Opened)
			{
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
			}
			Interlocked.Increment(ref this.outstandingProxies);
			return wcfconnectionStateTuple;
		}

		private void ReturnClientToPool(WCFConnectionStateTuple<TClient> clientInfo)
		{
			ArgumentValidator.ThrowIfNull("clientInfo", clientInfo);
			if (this.IsWCFClientUsable(clientInfo) && (long)this.pool.Count <= (long)((ulong)this.MaxNumberOfClientProxies))
			{
				this.pool.Push(clientInfo);
				return;
			}
			PolicySyncUtils.DisposeWcfClientGracefully((ICommunicationObject)((object)clientInfo.Client), this.logProvider, false);
		}

		private bool IsWCFClientUsable(WCFConnectionStateTuple<TClient> clientInfo)
		{
			ArgumentValidator.ThrowIfNull("clientInfo", clientInfo);
			return !(DateTime.UtcNow - clientInfo.LastUsed >= ServiceProxyPool<TClient>.ClientMaximumLifetime) && ServiceProxyPool<TClient>.IsUsable((clientInfo.Client as ICommunicationObject).State);
		}

		private void DecrementOutstandingProxies(string debugMessage)
		{
			Interlocked.Decrement(ref this.outstandingProxies);
		}

		private const int TotalRetries = 3;

		public static readonly TimeSpan DefaultInactivityTimeout = TimeSpan.FromMinutes(10.0);

		private static readonly TimeSpan ClientMaximumLifetime = ServiceProxyPool<TClient>.DefaultInactivityTimeout.Subtract(TimeSpan.FromSeconds(30.0));

		private readonly ConcurrentStack<WCFConnectionStateTuple<TClient>> pool = new ConcurrentStack<WCFConnectionStateTuple<TClient>>();

		private long outstandingProxies;

		private ExecutionLog logProvider;
	}
}
