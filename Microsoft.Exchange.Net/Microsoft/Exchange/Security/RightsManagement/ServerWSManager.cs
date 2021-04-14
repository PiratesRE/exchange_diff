using System;
using System.IO;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement.SOAP.Server;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal sealed class ServerWSManager : IDisposeTrackable, IDisposable
	{
		public ServerWSManager(Uri url, IWSManagerPerfCounters perfcounters, IRmsLatencyTracker latencyTracker, WebProxy proxy, TimeSpan timeout)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			this.wsProxy = new ServerWS();
			this.wsProxy.Url = RMUtil.ConvertUriToServerUrl(url, false);
			if (this.wsProxy.Url == null)
			{
				throw new RightsManagementException(RightsManagementFailureCode.FindServiceLocationFailed, DrmStrings.InvalidRmsUrl(url.ToString()));
			}
			if (proxy != null)
			{
				this.wsProxy.Proxy = proxy;
			}
			this.wsProxy.Timeout = (int)timeout.TotalMilliseconds;
			this.wsProxy.VersionDataValue = new VersionData();
			this.wsProxy.VersionDataValue.MaximumVersion = "1.2.0.0";
			this.wsProxy.VersionDataValue.MinimumVersion = "1.2.0.0";
			this.disposeTracker = this.GetDisposeTracker();
			this.perfcounters = (perfcounters ?? NoopWSManagerPerfCounters.Instance);
			this.rmsLatencyTracker = (latencyTracker ?? NoopRmsLatencyTracker.Instance);
		}

		public ServerWSManager(Uri url, ServiceType serviceLocationType, IWSManagerPerfCounters perfcounters, IRmsLatencyTracker latencyTracker, WebProxy proxy, TimeSpan timeout)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			if (serviceLocationType != ServiceType.CertificationService && serviceLocationType != ServiceType.LicensingService)
			{
				throw new ArgumentException("serviceLocationType should be either CertificationService or LicensingService!");
			}
			this.wsProxy = new ServerWS();
			this.wsProxy.Url = RMUtil.ConvertUriToServerUrl(url, serviceLocationType == ServiceType.CertificationService);
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
			return DisposeTracker.Get<ServerWSManager>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private ServiceLocationRequest[] CreateRequest(ServiceType[] serviceTypes)
		{
			ServiceLocationRequest[] array = new ServiceLocationRequest[serviceTypes.Length];
			int num = 0;
			foreach (ServiceType serviceType in serviceTypes)
			{
				array[num] = new ServiceLocationRequest();
				array[num].Type = serviceType;
				num++;
				ExTraceGlobals.RightsManagementTracer.TraceDebug<ServiceType>((long)this.GetHashCode(), "Adding serviceType : {0} to the request", serviceType);
			}
			return array;
		}

		internal IAsyncResult BeginFindServiceLocations(ServiceType[] serviceTypes, AsyncCallback callback, object state)
		{
			if (serviceTypes == null)
			{
				throw new ArgumentNullException("serviceTypes");
			}
			if (serviceTypes.Length == 0)
			{
				throw new ArgumentException("serviceTypes");
			}
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "BeginFindServiceLocations: Getting ServiceLocation types async from {0}", this.wsProxy.Url);
			ServiceLocationRequest[] serviceNames = this.CreateRequest(serviceTypes);
			this.rmsLatencyTracker.BeginTrackRmsLatency(RmsOperationType.FindServiceLocations);
			return this.wsProxy.BeginFindServiceLocations(serviceNames, callback, state);
		}

		internal ServiceLocationResponse[] EndFindServiceLocations(IAsyncResult asyncResult)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "EndFindServiceLocations invoked. Uri {0}", this.wsProxy.Url);
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			ServiceLocationResponse[] result = null;
			Exception ex = null;
			try
			{
				result = this.wsProxy.EndFindServiceLocations(asyncResult);
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
			this.rmsLatencyTracker.EndTrackRmsLatency(RmsOperationType.FindServiceLocations);
			if (ex != null)
			{
				this.perfcounters.FindServiceLocationsFailed();
				ExTraceGlobals.RightsManagementTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Failed to find the service locations from {0}. Error: {1}", this.wsProxy.Url, ex);
				throw new RightsManagementException(RightsManagementFailureCode.FindServiceLocationFailed, DrmStrings.FailedToFindServiceLocation(this.wsProxy.Url), ex, this.wsProxy.Url);
			}
			this.perfcounters.FindServiceLocationsSuccessful(this.wsProxy.GetElapsedMilliseconds());
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "Succesfully acquired the service locations from {0}", this.wsProxy.Url);
			return result;
		}

		public int GetServiceCryptoMode()
		{
			ServerInfoRequest serverInfoRequest = new ServerInfoRequest();
			serverInfoRequest.Type = ServerInfoType.ServerFeatureInfo;
			XmlNode xmlNode = null;
			Exception ex = null;
			try
			{
				xmlNode = this.wsProxy.GetServerInfo(new ServerInfoRequest[]
				{
					serverInfoRequest
				});
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
			if (ex != null || xmlNode == null)
			{
				ExTraceGlobals.RightsManagementTracer.TraceError<Exception>((long)this.GetHashCode(), "Error while obtaining RMS Server's crypto mode; RMS server is likely downlevel. Error {0}", ex);
				return 1;
			}
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "Validating RMS Server's crypto mode.  Response: {0}", xmlNode.InnerXml);
			return ServerWSManager.ExtractServerCryptoModeFromFeatureInfoXml(xmlNode);
		}

		public bool ValidateCertificationServiceVersion()
		{
			return this.ValidateServiceVersion("ServerInfoRequest[@Type=\"ServerFeatureInfo\"]/ServerFeatureInfo/Feature[@Name=\"ServerCertificationSharedIdentity\"]/@Value");
		}

		public bool ValidateLicensingServiceVersion()
		{
			return this.ValidateServiceVersion("ServerInfoRequest[@Type=\"ServerFeatureInfo\"]/ServerFeatureInfo/Feature[@Name=\"MultiForestAcquirePreLicense\"]/@Value");
		}

		internal static int ExtractServerCryptoModeFromFeatureInfoXml(XmlNode responseNode)
		{
			if (responseNode == null)
			{
				throw new ArgumentNullException("responseNode");
			}
			XmlNode xmlNode = responseNode.SelectSingleNode("ServerInfoRequest[@Type=\"ServerFeatureInfo\"]/ServerFeatureInfo/Feature[@Name=\"CryptoMode\"]/@Value");
			if (xmlNode == null)
			{
				return 1;
			}
			int result = 1;
			if (!int.TryParse(xmlNode.Value, out result))
			{
				return 1;
			}
			return result;
		}

		private bool ValidateServiceVersion(string featureXPath)
		{
			ServerInfoRequest serverInfoRequest = new ServerInfoRequest();
			serverInfoRequest.Type = ServerInfoType.VersionInfo;
			ServerInfoRequest serverInfoRequest2 = new ServerInfoRequest();
			serverInfoRequest2.Type = ServerInfoType.ServerFeatureInfo;
			XmlNode xmlNode = null;
			Exception ex = null;
			try
			{
				xmlNode = this.wsProxy.GetServerInfo(new ServerInfoRequest[]
				{
					serverInfoRequest2,
					serverInfoRequest
				});
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
			if (ex != null || xmlNode == null)
			{
				ExTraceGlobals.RightsManagementTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Hit an error while validating the service version. Feature:  {0}, Error {1}", featureXPath, ex);
				throw new RightsManagementException(RightsManagementFailureCode.GetServerInfoFailed, DrmStrings.FailedToGetServerInfo(this.wsProxy.Url), ex, this.wsProxy.Url);
			}
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Validating service version for feature: {0}. Response : {1}", featureXPath, xmlNode.InnerText);
			XmlNode xmlNode2 = xmlNode.SelectSingleNode(featureXPath);
			if (xmlNode2 != null)
			{
				return string.Equals(xmlNode2.Value, "true", StringComparison.OrdinalIgnoreCase);
			}
			xmlNode2 = xmlNode.SelectSingleNode("ServerInfoRequest[@Type=\"VersionInfo\"]/VersionInfo/@Version");
			return xmlNode2 != null && string.Compare(xmlNode2.Value, "6.1.0.0", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private const string XPathSharedIdentityFeature = "ServerInfoRequest[@Type=\"ServerFeatureInfo\"]/ServerFeatureInfo/Feature[@Name=\"ServerCertificationSharedIdentity\"]/@Value";

		private const string XPathAcquirePreLicenseFeature = "ServerInfoRequest[@Type=\"ServerFeatureInfo\"]/ServerFeatureInfo/Feature[@Name=\"MultiForestAcquirePreLicense\"]/@Value";

		private const string XPathCryptoModeFeature = "ServerInfoRequest[@Type=\"ServerFeatureInfo\"]/ServerFeatureInfo/Feature[@Name=\"CryptoMode\"]/@Value";

		private const string XPathVersionInfo = "ServerInfoRequest[@Type=\"VersionInfo\"]/VersionInfo/@Version";

		private const string MinimumRequiredRmsVersion = "6.1.0.0";

		private ServerWS wsProxy;

		private DisposeTracker disposeTracker;

		private IWSManagerPerfCounters perfcounters;

		private IRmsLatencyTracker rmsLatencyTracker;
	}
}
