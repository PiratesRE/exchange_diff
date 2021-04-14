using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RegisteredExchangeTopologyNotification : IRegisteredExchangeTopologyNotification
	{
		internal RegisteredExchangeTopologyNotification(ADNotificationCallback callback, ExchangeTopologyScope scope)
		{
			try
			{
				this.adNotificationRequestCookie = ADNotificationAdapter.RegisterExchangeTopologyChangeNotification(callback, null, scope);
			}
			catch (DataSourceOperationException innerException)
			{
				ServiceDiscoveryPermanentException ex = new ServiceDiscoveryPermanentException(ServerStrings.ExFailedToRegisterExchangeTopologyNotification, innerException);
				ExTraceGlobals.ServiceDiscoveryTracer.TraceError<ServiceDiscoveryPermanentException>(0L, "RegisteredExchangeTopologyNotification::Constructor. Failed to register. Throwing exception: {0}.", ex);
				throw ex;
			}
			catch (ADTransientException innerException2)
			{
				ServiceDiscoveryTransientException ex2 = new ServiceDiscoveryTransientException(ServerStrings.ExFailedToRegisterExchangeTopologyNotification, innerException2);
				ExTraceGlobals.ServiceDiscoveryTracer.TraceError<ServiceDiscoveryTransientException>(0L, "RegisteredExchangeTopologyNotification::Constructor. Failed to register. Throwing exception: {0}.", ex2);
				throw ex2;
			}
		}

		public void Unregister()
		{
			try
			{
				ADNotificationAdapter.UnregisterChangeNotification(this.adNotificationRequestCookie);
			}
			catch (DataSourceOperationException innerException)
			{
				ServiceDiscoveryPermanentException ex = new ServiceDiscoveryPermanentException(ServerStrings.ExFailedToUnregisterExchangeTopologyNotification, innerException);
				ExTraceGlobals.ServiceDiscoveryTracer.TraceError<ServiceDiscoveryPermanentException>(0L, "RegisteredExchangeTopologyNotification::Unregister. Failed to unregister. Throwing exception: {0}.", ex);
				throw ex;
			}
			catch (ADTransientException innerException2)
			{
				ServiceDiscoveryTransientException ex2 = new ServiceDiscoveryTransientException(ServerStrings.ExFailedToUnregisterExchangeTopologyNotification, innerException2);
				ExTraceGlobals.ServiceDiscoveryTracer.TraceError<ServiceDiscoveryTransientException>(0L, "RegisteredExchangeTopologyNotification::Unregister. Failed to unregister. Throwing exception: {0}.", ex2);
				throw ex2;
			}
		}

		private readonly ADNotificationRequestCookie adNotificationRequestCookie;
	}
}
