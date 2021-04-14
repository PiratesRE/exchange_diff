using System;
using System.IO;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement.SOAP.License;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal sealed class LicenseWSManager : IDisposeTrackable, IDisposable
	{
		public LicenseWSManager(Uri url, IWSManagerPerfCounters perfcounters, IRmsLatencyTracker latencyTracker, WebProxy proxy, TimeSpan timeout)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			this.wsProxy = new LicenseWS();
			this.wsProxy.Url = RMUtil.ConvertUriToLicenseUrl(url);
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
			return DisposeTracker.Get<LicenseWSManager>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public IAsyncResult BeginAcquireUseLicense(XmlNode[] rac, XmlNode[] issuanceLicense, AsyncCallback callback, object state)
		{
			if (rac == null)
			{
				throw new ArgumentNullException("rac");
			}
			if (issuanceLicense == null)
			{
				throw new ArgumentNullException("issuanceLicense");
			}
			AcquireLicenseParams[] requestParams = new AcquireLicenseParams[]
			{
				new AcquireLicenseParams
				{
					LicenseeCerts = rac,
					IssuanceLicense = issuanceLicense
				}
			};
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "BeginAcquireUseLicense: Fetching the use license async from {0}", this.wsProxy.Url);
			this.rmsLatencyTracker.BeginTrackRmsLatency(RmsOperationType.AcquireLicense);
			return this.wsProxy.BeginAcquireLicense(requestParams, callback, state);
		}

		public string EndAcquireUseLicense(IAsyncResult asyncResult)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "EndAcquireUseLicense invoked. Uri {0}", this.wsProxy.Url);
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AcquireLicenseResponse[] responses = null;
			Exception ex = null;
			try
			{
				responses = this.wsProxy.EndAcquireLicense(asyncResult);
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
			this.rmsLatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireLicense);
			if (ex != null)
			{
				this.perfcounters.AcquireLicenseFailed();
				ExTraceGlobals.RightsManagementTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Failed to acquire the use license from {0}. Error: {1}", this.wsProxy.Url, ex);
				throw new RightsManagementException(RightsManagementFailureCode.UseLicenseAcquisitionFailed, DrmStrings.FailedToAcquireUseLicense(this.wsProxy.Url), ex, this.wsProxy.Url);
			}
			return this.GetUseLicenseOrExceptionFromResponse(responses);
		}

		private static LocalizedString GetExceptionString(XmlNode node)
		{
			XmlNode xmlNode = node.SelectSingleNode("descendant::ExceptionString");
			if (xmlNode == null || string.IsNullOrEmpty(xmlNode.InnerText))
			{
				return DrmStrings.RmExceptionGenericMessage;
			}
			return new LocalizedString(xmlNode.InnerText);
		}

		private string GetUseLicenseOrExceptionFromResponse(AcquireLicenseResponse[] responses)
		{
			if (responses == null || responses.Length <= 0 || responses[0] == null || !RMUtil.IsValidCertificateChain(responses[0].CertificateChain))
			{
				this.perfcounters.AcquireLicenseFailed();
				ExTraceGlobals.RightsManagementTracer.TraceError<string>((long)this.GetHashCode(), "Invalid or empty response for the call to acquire use license from {0} ", this.wsProxy.Url);
				throw new RightsManagementException(RightsManagementFailureCode.UseLicenseAcquisitionFailed, DrmStrings.InvalidUseLicenseResponse(this.wsProxy.Url));
			}
			XmlNode xmlNode = responses[0].CertificateChain[0];
			if (string.Equals(xmlNode.Name, "XrML", StringComparison.OrdinalIgnoreCase))
			{
				this.perfcounters.AcquireLicenseSuccessful(this.wsProxy.GetElapsedMilliseconds());
				ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "Succesfully acquired the use license from {0} ", this.wsProxy.Url);
				return RMUtil.ConvertXmlNodeArrayToString(responses[0].CertificateChain);
			}
			if (string.Equals(xmlNode.Name, "AcquireLicenseException", StringComparison.OrdinalIgnoreCase))
			{
				this.perfcounters.AcquireLicenseFailed();
				LocalizedString exceptionString = LicenseWSManager.GetExceptionString(xmlNode);
				ExTraceGlobals.RightsManagementTracer.TraceError<string, LocalizedString>((long)this.GetHashCode(), "Hit an error when acquiring use license from {0}. Error {1}", this.wsProxy.Url, exceptionString);
				throw new RightsManagementException(RightsManagementFailureCode.UseLicenseAcquisitionFailed, exceptionString, this.wsProxy.Url);
			}
			this.perfcounters.AcquireLicenseFailed();
			ExTraceGlobals.RightsManagementTracer.TraceError<string>((long)this.GetHashCode(), "Invalid response when acquiring use license from {0}. Neither the license or the exception node is set", this.wsProxy.Url);
			throw new RightsManagementException(RightsManagementFailureCode.UseLicenseAcquisitionFailed, DrmStrings.RmExceptionGenericMessage, this.wsProxy.Url);
		}

		private const string EulElementName = "XrML";

		private const string ExceptionElementName = "AcquireLicenseException";

		private const string ExceptionXPath = "descendant::ExceptionString";

		private LicenseWS wsProxy;

		private DisposeTracker disposeTracker;

		private IWSManagerPerfCounters perfcounters;

		private IRmsLatencyTracker rmsLatencyTracker;
	}
}
