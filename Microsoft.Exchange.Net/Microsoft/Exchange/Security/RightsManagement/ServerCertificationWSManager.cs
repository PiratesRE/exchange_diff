using System;
using System.IO;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement.SOAP.ServerCertification;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal sealed class ServerCertificationWSManager : IDisposeTrackable, IDisposable
	{
		public ServerCertificationWSManager(Uri url, IWSManagerPerfCounters perfcounters, IRmsLatencyTracker latencyTracker, WebProxy proxy, TimeSpan timeout)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			this.wsProxy = new ServerCertificationWS();
			this.wsProxy.Url = RMUtil.ConvertUriToServerCertificationUrl(url);
			if (this.wsProxy.Url == null)
			{
				throw new InvalidRmsUrlException(url.OriginalString);
			}
			if (proxy != null)
			{
				this.wsProxy.Proxy = proxy;
			}
			this.wsProxy.Timeout = (int)timeout.TotalMilliseconds;
			this.wsProxy.Credentials = CredentialCache.DefaultCredentials;
			this.wsProxy.VersionDataValue = new VersionData();
			this.wsProxy.VersionDataValue.MaximumVersion = "1.1.0.0";
			this.wsProxy.VersionDataValue.MinimumVersion = "1.1.0.0";
			this.disposeTracker = this.GetDisposeTracker();
			this.perfcounters = (perfcounters ?? NoopWSManagerPerfCounters.Instance);
			this.rmsLatencyTracker = (latencyTracker ?? NoopRmsLatencyTracker.Instance);
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			if (this.wsProxy != null)
			{
				this.wsProxy.Dispose();
				this.wsProxy = null;
			}
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ServerCertificationWSManager>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal IAsyncResult BeginAcquireRac(XmlNode[] machineCertificateChain, AsyncCallback callback, object state)
		{
			if (machineCertificateChain == null)
			{
				throw new ArgumentNullException("machineCertificateChain");
			}
			CertifyParams certifyParams = new CertifyParams();
			certifyParams.Persistent = true;
			certifyParams.MachineCertificateChain = machineCertificateChain;
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "BeginAcquireRac: Fetching RAC async from {0}", this.wsProxy.Url);
			this.rmsLatencyTracker.BeginTrackRmsLatency(RmsOperationType.AcquireServerBoxRac);
			return this.wsProxy.BeginCertify(certifyParams, callback, state);
		}

		internal XmlNode[] EndAcquireRac(IAsyncResult asyncResult)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "EndAcquireRac invoked. Uri {0}", this.wsProxy.Url);
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			CertifyResponse response = null;
			Exception ex = null;
			try
			{
				response = this.wsProxy.EndCertify(asyncResult);
			}
			catch (SoapException ex2)
			{
				ex = ex2;
			}
			catch (WebException ex3)
			{
				ex = ex3;
			}
			catch (InvalidOperationException ex4)
			{
				ex = ex4;
			}
			catch (IOException ex5)
			{
				ex = ex5;
			}
			catch (UnauthorizedAccessException ex6)
			{
				ex = ex6;
			}
			this.rmsLatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireServerBoxRac);
			if (ex != null)
			{
				this.perfcounters.CertifyFailed();
				ExTraceGlobals.RightsManagementTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Failed to acquire the RAC from {0}. Error: {1}", this.wsProxy.Url, ex);
				throw new RightsManagementException(RightsManagementFailureCode.RacAcquisitionFailed, DrmStrings.FailedToAcquireServerBoxRac(this.wsProxy.Url), ex, this.wsProxy.Url);
			}
			return this.GetRacOrExceptionFromResponse(response);
		}

		private XmlNode[] GetRacOrExceptionFromResponse(CertifyResponse response)
		{
			if (response != null && response.CertificateChain != null && response.CertificateChain.Length != 0)
			{
				bool flag = false;
				for (int i = 0; i < response.CertificateChain.Length; i++)
				{
					if (response.CertificateChain[i] == null || string.IsNullOrEmpty(response.CertificateChain[i].OuterXml))
					{
						flag = true;
						ExTraceGlobals.RightsManagementTracer.TraceError<string, int, int>((long)this.GetHashCode(), "Invalid response for the call to acquire RAC from {0}. The certificate chain {1} is not present. Number of certificates {2}", this.wsProxy.Url, i, response.CertificateChain.Length);
						break;
					}
				}
				if (!flag)
				{
					this.perfcounters.CertifySuccessful(this.wsProxy.GetElapsedMilliseconds());
					ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "Succesfully acquired the RAC from {0}", this.wsProxy.Url);
					return response.CertificateChain;
				}
			}
			this.perfcounters.CertifyFailed();
			ExTraceGlobals.RightsManagementTracer.TraceError<string>((long)this.GetHashCode(), "Empty response for the call to acquire RAC from {0} ", this.wsProxy.Url);
			throw new RightsManagementException(RightsManagementFailureCode.RacAcquisitionFailed, DrmStrings.InvalidResponseToServerBoxRacRequest(this.wsProxy.Url));
		}

		private ServerCertificationWS wsProxy;

		private DisposeTracker disposeTracker;

		private IWSManagerPerfCounters perfcounters;

		private IRmsLatencyTracker rmsLatencyTracker;
	}
}
