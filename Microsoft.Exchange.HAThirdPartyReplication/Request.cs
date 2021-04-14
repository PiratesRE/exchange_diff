using System;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	public class Request : IDisposable
	{
		public static Request Open(TimeSpan openTimeout, TimeSpan sendTimeout, TimeSpan receiveTimeout)
		{
			Request request = new Request();
			EndpointAddress remoteAddress = new EndpointAddress("net.pipe://localhost/Microsoft.Exchange.ThirdPartyReplication.RequestService");
			request.m_wcfClient = new InternalRequestClient(ClientServices.SetupBinding(openTimeout, sendTimeout, receiveTimeout), remoteAddress);
			return request;
		}

		public void Dispose()
		{
			if (this.m_wcfClient != null)
			{
				ExTraceGlobals.ThirdPartyClientTracer.TraceDebug((long)this.GetHashCode(), "Request.Dispose invoked");
				this.m_wcfClient.Abort();
				this.m_wcfClient = null;
			}
		}

		public string GetPrimaryActiveManager()
		{
			byte[] exBytes = null;
			string pam = null;
			ClientServices.CallService(delegate
			{
				pam = this.m_wcfClient.GetPrimaryActiveManager(out exBytes);
			});
			if (exBytes != null)
			{
				Exception ex = (Exception)Serialization.BytesToObject(exBytes);
				ExTraceGlobals.ThirdPartyClientTracer.TraceError<Exception>((long)this.GetHashCode(), "GetPAM fails: {0}", ex);
				throw ex;
			}
			ExTraceGlobals.ThirdPartyClientTracer.TraceDebug<string>((long)this.GetHashCode(), "GetPAM returns: {0}", pam);
			return pam;
		}

		public void ChangeActiveServer(Guid dbId, string newNode)
		{
			Exception serverEx = null;
			byte[] exBytes = null;
			ClientServices.CallService(delegate
			{
				exBytes = this.m_wcfClient.ChangeActiveServer(dbId, newNode);
				if (exBytes != null)
				{
					serverEx = (Exception)Serialization.BytesToObject(exBytes);
				}
			});
			if (serverEx != null)
			{
				ExTraceGlobals.ThirdPartyClientTracer.TraceError<Guid, string, Exception>((long)this.GetHashCode(), "ChangeActiveServer({0},{1}) fails: {2}", dbId, newNode, serverEx);
				throw serverEx;
			}
		}

		public void ImmediateDismountMailboxDatabase(Guid dbId)
		{
			Exception serverEx = null;
			byte[] exBytes = null;
			ClientServices.CallService(delegate
			{
				exBytes = this.m_wcfClient.ImmediateDismountMailboxDatabase(dbId);
				if (exBytes != null)
				{
					serverEx = (Exception)Serialization.BytesToObject(exBytes);
				}
			});
			if (serverEx != null)
			{
				ExTraceGlobals.ThirdPartyClientTracer.TraceError<Guid, Exception>((long)this.GetHashCode(), "ImmediateDismountMailboxDatabase({0}) fails: {1}", dbId, serverEx);
				throw serverEx;
			}
		}

		internal void AmeIsStarting(TimeSpan retryDelay, TimeSpan openTimeout, TimeSpan sendTimeout, TimeSpan receiveTimeout)
		{
			ClientServices.CallService(delegate
			{
				this.m_wcfClient.AmeIsStarting(retryDelay, openTimeout, sendTimeout, receiveTimeout);
			});
		}

		internal void AmeIsStopping()
		{
			ClientServices.CallService(delegate
			{
				this.m_wcfClient.AmeIsStopping();
			});
		}

		private InternalRequestClient m_wcfClient;
	}
}
