using System;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	internal class Notify : IDisposable
	{
		public static Notify Open(TimeSpan openTimeout, TimeSpan sendTimeout, TimeSpan receiveTimeout)
		{
			Notify notify = new Notify();
			EndpointAddress remoteAddress = new EndpointAddress("net.pipe://localhost/Microsoft.Exchange.ThirdPartyReplication.NotifyService");
			notify.m_wcfClient = new InternalNotifyClient(ClientServices.SetupBinding(openTimeout, sendTimeout, receiveTimeout), remoteAddress);
			ExTraceGlobals.ThirdPartyClientTracer.TraceDebug((long)notify.GetHashCode(), "Notify.Open successful");
			return notify;
		}

		public void Dispose()
		{
			if (this.m_wcfClient != null)
			{
				ExTraceGlobals.ThirdPartyClientTracer.TraceDebug((long)this.GetHashCode(), "Notify.Dispose invoked");
				this.m_wcfClient.Abort();
				this.m_wcfClient = null;
			}
		}

		internal TimeSpan SendTimeout
		{
			get
			{
				return this.m_wcfClient.Endpoint.Binding.SendTimeout;
			}
			set
			{
				this.m_wcfClient.Endpoint.Binding.SendTimeout = value;
			}
		}

		internal TimeSpan ReceiveTimeout
		{
			get
			{
				return this.m_wcfClient.Endpoint.Binding.ReceiveTimeout;
			}
			set
			{
				this.m_wcfClient.Endpoint.Binding.ReceiveTimeout = value;
			}
		}

		public void BecomePame()
		{
			ExTraceGlobals.ThirdPartyClientTracer.TraceDebug((long)this.GetHashCode(), "Notify.BecomePame attempted");
			ClientServices.CallService(delegate
			{
				this.m_wcfClient.BecomePame();
			});
		}

		public void RevokePame()
		{
			ExTraceGlobals.ThirdPartyClientTracer.TraceDebug((long)this.GetHashCode(), "Notify.RevokePame attempted");
			ClientServices.CallService(delegate
			{
				this.m_wcfClient.RevokePame();
			});
		}

		public NotificationResponse DatabaseMoveNeeded(Guid dbId, string currentActiveFqdn, bool mountDesired)
		{
			NotificationResponse rc = NotificationResponse.Incomplete;
			ExTraceGlobals.ThirdPartyClientTracer.TraceDebug<Guid, string, bool>((long)this.GetHashCode(), "Notify.DatabaseMoveNeeded({0},{1},{2}) attempted", dbId, currentActiveFqdn, mountDesired);
			ClientServices.CallService(delegate
			{
				rc = this.m_wcfClient.DatabaseMoveNeeded(dbId, currentActiveFqdn, mountDesired);
			});
			return rc;
		}

		internal void GetTimeouts(out TimeSpan retryDelay, out TimeSpan openTimeout, out TimeSpan sendTimeout, out TimeSpan receiveTimeout)
		{
			try
			{
				this.m_wcfClient.GetTimeouts(out retryDelay, out openTimeout, out sendTimeout, out receiveTimeout);
			}
			catch (Exception ex)
			{
				throw new FailedCommunicationException(ex.Message, ex);
			}
		}

		private InternalNotifyClient m_wcfClient;
	}
}
