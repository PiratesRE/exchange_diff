using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ADNotificationHandler
	{
		internal ExchangeTopologyScope Scope
		{
			get
			{
				return this.scope;
			}
			set
			{
				lock (this.registeredExchangeTopologyNotificationLock)
				{
					if (this.registeredExchangeTopologyNotification != null)
					{
						throw new InvalidOperationException("Can't set scope after notification is regiestered");
					}
					this.scope = value;
				}
			}
		}

		internal void RegisterExchangeTopologyNotificationIfNeeded()
		{
			if (this.registeredExchangeTopologyNotification == null)
			{
				this.RegisterExchangeTopologyNotification();
			}
		}

		internal void UnRegisterExchangeTopologyNotification()
		{
			this.InternalUnRegisterExchangeTopologyNotification();
		}

		private void RegisterExchangeTopologyNotification()
		{
			Exception ex = null;
			try
			{
				lock (this.registeredExchangeTopologyNotificationLock)
				{
					this.registeredExchangeTopologyNotification = ServiceDiscovery.ExchangeTopologyBridge.RegisterExchangeTopologyNotification(new ADNotificationCallback(this.ADNotificationCallback), this.scope);
				}
			}
			catch (ServiceDiscoveryPermanentException ex2)
			{
				ex = ex2;
			}
			catch (ServiceDiscoveryTransientException ex3)
			{
				ex = ex3;
			}
			if (ex == null)
			{
				ExTraceGlobals.ServiceDiscoveryTracer.TraceDebug(0L, "ADNotificationHandler::RegisterExchangeTopologyNotification. Successfully registered for ExchangeTopologyNotification.");
				ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_RegisteredForTopologyChangedNotification, null, new object[0]);
				return;
			}
			string text = ex.ToString().TruncateToUseInEventLog();
			ExTraceGlobals.ServiceDiscoveryTracer.TraceError<string>(0L, "ADNotificationHandler::RegisterExchangeTopologyNotification. Failed to Failed to register for ExchangeTopologyNotification. Error = {0}", text);
			ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorRegisteringForTopologyChangedNotification, null, new object[]
			{
				text
			});
		}

		private void InternalUnRegisterExchangeTopologyNotification()
		{
			try
			{
				if (this.registeredExchangeTopologyNotification != null)
				{
					this.registeredExchangeTopologyNotification.Unregister();
				}
			}
			catch (ServiceDiscoveryPermanentException arg)
			{
				ExTraceGlobals.ServiceDiscoveryTracer.TraceError<ServiceDiscoveryPermanentException>(0L, "ADNotificationHandler::UnRegisterExchangeTopologyNotification. Failed to unregister for ExchangeTopologyNotification. Error = {0}", arg);
			}
			catch (ServiceDiscoveryTransientException arg2)
			{
				ExTraceGlobals.ServiceDiscoveryTracer.TraceError<ServiceDiscoveryTransientException>(0L, "ADNotificationHandler::UnRegisterExchangeTopologyNotification. Failed to Failed to unregister for ExchangeTopologyNotification. Error = {0}", arg2);
			}
			lock (this.thisLock)
			{
				this.notificationArrived = false;
			}
			this.registeredExchangeTopologyNotification = null;
		}

		private void ADNotificationCallback(ADNotificationEventArgs args)
		{
			lock (this.thisLock)
			{
				if (!this.notificationArrived)
				{
					this.notificationArrived = true;
					this.EnqueueTriggerCacheRefresh();
				}
			}
		}

		private void EnqueueTriggerCacheRefresh()
		{
			ThreadPool.RegisterWaitForSingleObject(this.stopTimerEvent, new WaitOrTimerCallback(this.TriggerCacheRefresh), null, ServiceDiscovery.ExchangeTopologyBridge.NotificationDelayTimeout, true);
		}

		private void TriggerCacheRefresh(object state, bool timedOut)
		{
			lock (this.thisLock)
			{
				this.notificationArrived = false;
			}
			ServiceCache.TriggerCacheRefreshDueToNotification();
		}

		private readonly object thisLock = new object();

		private readonly ManualResetEvent stopTimerEvent = new ManualResetEvent(false);

		private readonly object registeredExchangeTopologyNotificationLock = new object();

		private IRegisteredExchangeTopologyNotification registeredExchangeTopologyNotification;

		private bool notificationArrived;

		private ExchangeTopologyScope scope = ExchangeTopologyScope.ADAndExchangeServerAndSiteTopology;
	}
}
