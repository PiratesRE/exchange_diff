using System;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Client
{
	internal class PushNotificationsProxyPool<TClient> : ServiceProxyPool<TClient>
	{
		internal PushNotificationsProxyPool(string endpointName, string hostName, ChannelFactory<TClient> channelFactory, bool useDisposeTracker) : base(endpointName, hostName, 10, channelFactory, useDisposeTracker)
		{
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.PushNotificationClientTracer;
			}
		}

		protected override bool IsTransientException(Exception ex)
		{
			ArgumentValidator.ThrowIfNull("ex", ex);
			if (ex is FaultException<PushNotificationFault>)
			{
				return ((FaultException<PushNotificationFault>)ex).Detail.CanRetry;
			}
			return base.IsTransientException(ex);
		}

		protected override Exception GetTransientWrappedException(Exception wcfException)
		{
			if (wcfException is TimeoutException)
			{
				return new PushNotificationTransientException(Strings.ExceptionMessageTimeoutCall(base.TargetInfo, wcfException.Message), wcfException);
			}
			if (wcfException is EndpointNotFoundException)
			{
				return new PushNotificationEndpointNotFoundException(Strings.ExceptionEndpointNotFoundError(base.TargetInfo, wcfException.Message), wcfException);
			}
			if (wcfException is FaultException<PushNotificationFault>)
			{
				return new PushNotificationTransientException(Strings.ExceptionPushNotificationError(base.TargetInfo, ((FaultException<PushNotificationFault>)wcfException).Detail.Message), wcfException);
			}
			return new PushNotificationTransientException(Strings.ExceptionPushNotificationError(base.TargetInfo, wcfException.Message), wcfException);
		}

		protected override Exception GetPermanentWrappedException(Exception wcfException)
		{
			if (wcfException is FaultException<InvalidOperationException>)
			{
				return ((FaultException<InvalidOperationException>)wcfException).Detail;
			}
			if (wcfException is FaultException<ArgumentNullException>)
			{
				return ((FaultException<ArgumentNullException>)wcfException).Detail;
			}
			if (wcfException is FaultException<ArgumentException>)
			{
				return ((FaultException<ArgumentException>)wcfException).Detail;
			}
			if (wcfException is FaultException<PushNotificationFault>)
			{
				return new PushNotificationPermanentException(Strings.ExceptionPushNotificationError(base.TargetInfo, ((FaultException<PushNotificationFault>)wcfException).Detail.Message), wcfException);
			}
			return new PushNotificationPermanentException(Strings.ExceptionPushNotificationError(base.TargetInfo, wcfException.Message), wcfException);
		}

		protected override void LogCallServiceError(Exception error, string periodicKey, string debugMessage, int numberOfRetries)
		{
			this.Tracer.TraceError<string, int, string>((long)this.GetHashCode(), "Client failed to execute a service command '{0}' after '{1}' number of attempts.\n{2}.", debugMessage, numberOfRetries, error.ToTraceString());
			PushNotificationsCrimsonEvents.CallServiceError.Log<string, string, int>(debugMessage, error.ToTraceString(), numberOfRetries);
		}

		private const int MaxNumberOfClientProxies = 10;
	}
}
