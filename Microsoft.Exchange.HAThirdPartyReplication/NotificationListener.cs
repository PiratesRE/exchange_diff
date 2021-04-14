using System;
using System.ServiceModel;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	public class NotificationListener
	{
		internal TimeSpan RetryDelay
		{
			get
			{
				return this.m_retryDelay;
			}
		}

		internal TimeSpan OpenTimeout
		{
			get
			{
				return this.m_openTimeout;
			}
		}

		internal TimeSpan SendTimeout
		{
			get
			{
				return this.m_sendTimeout;
			}
		}

		internal TimeSpan ReceiveTimeout
		{
			get
			{
				return this.m_receiveTimeout;
			}
		}

		public static NotificationListener Start(INotifyCallback callback, TimeSpan retryDelay, TimeSpan openTimeout, TimeSpan sendTimeout, TimeSpan receiveTimeout)
		{
			Exception ex = null;
			ExTraceGlobals.ThirdPartyClientTracer.TraceDebug(0L, "NotificationListener is starting.");
			NotificationListener notificationListener = new NotificationListener();
			try
			{
				notificationListener.m_retryDelay = retryDelay;
				notificationListener.m_openTimeout = openTimeout;
				notificationListener.m_sendTimeout = sendTimeout;
				notificationListener.m_receiveTimeout = receiveTimeout;
				notificationListener.SetupNotifyHost(callback);
				ReplayCrimsonEvents.TPRNotificationListenerStarted.Log();
				ExTraceGlobals.ThirdPartyClientTracer.TraceDebug(0L, "NotificationListener was started successfully.");
				notificationListener.SendTimeouts();
				return notificationListener;
			}
			catch (CommunicationException ex2)
			{
				ex = ex2;
			}
			catch (Exception ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ReplayCrimsonEvents.TPRNotificationListenerFailedToStart.Log<string>(ex.Message);
				ExTraceGlobals.ThirdPartyClientTracer.TraceError<Exception>(0L, "NotificationListener failed to start: {0}", ex);
				throw ex;
			}
			return null;
		}

		public void Stop()
		{
			ReplayCrimsonEvents.TPRNotificationListenerStopped.Log();
			if (this.m_notifyHost != null)
			{
				this.m_notifyHost.Close();
			}
			if (this.m_service != null)
			{
				this.m_service = null;
			}
		}

		private void SetupNotifyHost(INotifyCallback callback)
		{
			this.m_service = new NotifyService(callback, this);
			EndpointAddress endpointAddress = new EndpointAddress("net.pipe://localhost/Microsoft.Exchange.ThirdPartyReplication.NotifyService");
			this.m_notifyHost = new ServiceHost(this.m_service, new Uri[]
			{
				endpointAddress.Uri
			});
			NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding();
			netNamedPipeBinding.OpenTimeout = this.OpenTimeout;
			netNamedPipeBinding.SendTimeout = this.SendTimeout;
			netNamedPipeBinding.ReceiveTimeout = this.ReceiveTimeout;
			try
			{
				this.m_notifyHost.AddServiceEndpoint(typeof(IInternalNotify), netNamedPipeBinding, string.Empty);
				this.m_notifyHost.Open();
			}
			catch (CommunicationException ex)
			{
				this.m_notifyHost.Abort();
				this.m_notifyHost = null;
				throw ex;
			}
		}

		private void SendTimeouts()
		{
			lock (this)
			{
				Exception ex = null;
				try
				{
					using (Request request = Request.Open(this.m_openTimeout, this.m_sendTimeout, this.m_receiveTimeout))
					{
						request.AmeIsStarting(this.m_retryDelay, this.m_openTimeout, this.m_sendTimeout, this.m_receiveTimeout);
					}
				}
				catch (FailedCommunicationException ex2)
				{
					ex = ex2;
				}
				catch (Exception ex3)
				{
					ReplayCrimsonEvents.TPRUnexpectedException.Log<string, Exception>(ex3.Message, ex3);
					ex = ex3;
				}
				if (ex != null)
				{
					ExTraceGlobals.ThirdPartyClientTracer.TraceError<Exception>(0L, "NotificationListener failed to SendTimeouts. ActiveManager is expected to contact us later: {0}", ex);
				}
			}
		}

		private ServiceHost m_notifyHost;

		private NotifyService m_service;

		private TimeSpan m_retryDelay;

		private TimeSpan m_openTimeout;

		private TimeSpan m_sendTimeout;

		private TimeSpan m_receiveTimeout;
	}
}
