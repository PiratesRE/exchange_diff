using System;
using System.IO;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement.SOAP.Publish;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal sealed class PublishWSManager : IDisposeTrackable, IDisposable
	{
		public PublishWSManager(Uri url, IWSManagerPerfCounters perfcounters, IRmsLatencyTracker latencyTracker, WebProxy proxy, TimeSpan timeout)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			this.wsProxy = new PublishWS();
			this.wsProxy.Url = RMUtil.ConvertUriToPublishUrl(url);
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
			return DisposeTracker.Get<PublishWSManager>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal IAsyncResult BeginAcquireClc(XmlNode[] rac, AsyncCallback callback, object state)
		{
			if (rac == null)
			{
				throw new ArgumentNullException("rac");
			}
			GetClientLicensorCertParams[] requestParams = new GetClientLicensorCertParams[]
			{
				new GetClientLicensorCertParams
				{
					PersonaCerts = rac
				}
			};
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "BeginAcquireClc: Fetching CLC async from {0}", this.wsProxy.Url);
			this.rmsLatencyTracker.BeginTrackRmsLatency(RmsOperationType.AcquireClc);
			return this.wsProxy.BeginGetClientLicensorCert(requestParams, callback, state);
		}

		internal XmlNode[] EndAcquireClc(IAsyncResult asyncResult)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "EndAcquireClc invoked. Uri {0}", this.wsProxy.Url);
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			GetClientLicensorCertResponse[] responses = null;
			Exception ex = null;
			try
			{
				responses = this.wsProxy.EndGetClientLicensorCert(asyncResult);
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
			this.rmsLatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireClc);
			if (ex != null)
			{
				this.perfcounters.GetClientLicensorCertFailed();
				ExTraceGlobals.RightsManagementTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Failed to acquire the CLC from {0}. Error: {1}", this.wsProxy.Url, ex);
				throw new RightsManagementException(RightsManagementFailureCode.ClcAcquisitionFailed, DrmStrings.FailedToAcquireClc(this.wsProxy.Url), ex, this.wsProxy.Url);
			}
			return this.GetClcOrExceptionFromResponse(responses);
		}

		private XmlNode[] GetClcOrExceptionFromResponse(GetClientLicensorCertResponse[] responses)
		{
			if (responses != null && responses.Length == 1 && responses[0] != null && RMUtil.IsValidCertificateChain(responses[0].CertificateChain))
			{
				this.perfcounters.GetClientLicensorCertSuccessful(this.wsProxy.GetElapsedMilliseconds());
				ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "Succesfully acquired the CLC from {0}", this.wsProxy.Url);
				return responses[0].CertificateChain;
			}
			this.perfcounters.GetClientLicensorCertFailed();
			ExTraceGlobals.RightsManagementTracer.TraceError<string>((long)this.GetHashCode(), "Invalid or empty response for the call to acquire CLC from {0} ", this.wsProxy.Url);
			throw new RightsManagementException(RightsManagementFailureCode.ClcAcquisitionFailed, DrmStrings.InvalidResponseToClcRequest(this.wsProxy.Url));
		}

		private PublishWS wsProxy;

		private DisposeTracker disposeTracker;

		private IWSManagerPerfCounters perfcounters;

		private IRmsLatencyTracker rmsLatencyTracker;
	}
}
