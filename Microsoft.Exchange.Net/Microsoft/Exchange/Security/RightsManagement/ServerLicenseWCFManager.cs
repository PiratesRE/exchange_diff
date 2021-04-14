using System;
using System.ServiceModel;
using System.Xml;
using Microsoft.com.IPC.WSServerLicensingService;
using Microsoft.com.IPC.WSService;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal sealed class ServerLicenseWCFManager : LicenseManager
	{
		internal ServerLicenseWCFManager(ChannelFactory<IWSServerLicensingServiceChannel> channelFactory, Uri targetUri, XrmlCertificateChain rac, IWSManagerPerfCounters perfcounters, IRmsLatencyTracker latencyTracker) : base(perfcounters, latencyTracker)
		{
			this.channelFactory = channelFactory;
			this.channel = channelFactory.CreateChannel();
			this.targetUri = targetUri;
			this.rac = rac;
		}

		public override void Dispose()
		{
			if (this.channel != null)
			{
				if (this.channel.State == CommunicationState.Faulted)
				{
					this.channel.Abort();
				}
				else
				{
					try
					{
						this.channel.Close();
					}
					catch (TimeoutException)
					{
					}
				}
				this.channel = null;
			}
			if (this.channelFactory != null)
			{
				if (this.channelFactory.State == CommunicationState.Faulted)
				{
					this.channelFactory.Abort();
				}
				else
				{
					try
					{
						this.channelFactory.Close();
					}
					catch (TimeoutException)
					{
					}
				}
				this.channelFactory = null;
			}
			base.Dispose();
		}

		protected override void IssueNewWebRequest(LazyAsyncResult asyncResult)
		{
			LicenseManager.LicenseState licenseState = asyncResult.AsyncObject as LicenseManager.LicenseState;
			ExTraceGlobals.RightsManagementTracer.TraceDebug<Uri>((long)this.GetHashCode(), "IssueNewWebRequest for the next batch from {0}", this.channelFactory.Endpoint.Address.Uri);
			LicenseIdentity[] identitiesForNextBatch = licenseState.GetIdentitiesForNextBatch();
			if (identitiesForNextBatch != null)
			{
				VersionData versionData = new VersionData();
				versionData.MaximumVersion = "1.0.0.0";
				versionData.MinimumVersion = "1.0.0.0";
				XrmlCertificateChain issuanceLicense = new XrmlCertificateChain();
				issuanceLicense.CertificateChain = DrmClientUtils.ConvertXmlNodeArrayToStringArray(licenseState.IssuanceLicense);
				LicenseeIdentity[] licenseeIdentities = new LicenseeIdentity[identitiesForNextBatch.Length];
				for (int i = 0; i < identitiesForNextBatch.Length; i++)
				{
					licenseeIdentities[i] = new LicenseeIdentity();
					licenseeIdentities[i].Email = identitiesForNextBatch[i].Email;
					licenseeIdentities[i].ProxyAddresses = identitiesForNextBatch[i].ProxyAddresses;
				}
				RightsManagementException ex = DrmClientUtils.RunWCFOperation(delegate
				{
					this.channel.BeginAcquireServerLicense(new AcquireServerLicenseRequestMessage(versionData, this.rac, issuanceLicense, licenseeIdentities), new AsyncCallback(this.AcquireLicenseCallback), asyncResult);
				}, this.channelFactory.Endpoint.Address.Uri, this.targetUri);
				if (ex != null)
				{
					this.perfcounters.WCFAcquireServerLicenseFailed();
					ExTraceGlobals.RightsManagementTracer.TraceError<Uri, RightsManagementException>(0L, "Hit an exception when talking to the external RMS server pipeline {0} for serverlicense request. Error {1}", this.channelFactory.Endpoint.Address.Uri, ex);
					asyncResult.InvokeCallback(ex);
					return;
				}
			}
			else
			{
				if (licenseState.Exception != null)
				{
					this.perfcounters.WCFAcquireServerLicenseFailed();
				}
				else
				{
					ExTraceGlobals.RightsManagementTracer.TraceDebug((long)this.GetHashCode(), "Successfully processed requesting all the identities");
				}
				asyncResult.InvokeCallback(licenseState.Exception);
			}
		}

		protected override void AcquireLicenseCallback(IAsyncResult asyncResult)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug<Uri>((long)this.GetHashCode(), "AcquireLicenseCallback invoked. Uri {0}", this.channelFactory.Endpoint.Address.Uri);
			if (asyncResult == null)
			{
				throw new InvalidOperationException("asyncResult cannot be null here.");
			}
			LazyAsyncResult lazyAsyncResult = asyncResult.AsyncState as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState cannot be null and has to be of type LazyAsyncResult.");
			}
			LicenseManager.LicenseState licenseState = lazyAsyncResult.AsyncObject as LicenseManager.LicenseState;
			if (licenseState == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncObject cannot be null and has to be of type LicenseState.");
			}
			AcquireServerLicenseResponseMessage response = null;
			RightsManagementException ex = DrmClientUtils.RunWCFOperation(delegate
			{
				response = this.channel.EndAcquireServerLicense(asyncResult);
			}, this.channelFactory.Endpoint.Address.Uri, this.targetUri);
			this.rmsLatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireB2BLicense);
			if (ex != null)
			{
				ExTraceGlobals.RightsManagementTracer.TraceError<Uri, RightsManagementException>((long)this.GetHashCode(), "Failed to acquire server licenses from {0}. Error: {1}", this.channelFactory.Endpoint.Address.Uri, ex);
				licenseState.Exception = ex;
			}
			else
			{
				this.ParseResponses(response, licenseState);
			}
			this.IssueNewWebRequest(lazyAsyncResult);
		}

		private void ParseResponses(AcquireServerLicenseResponseMessage response, LicenseManager.LicenseState licenseState)
		{
			int currentBatchLength = licenseState.GetCurrentBatchLength();
			XmlNode[] certChain = null;
			if (response != null && response.AcquireServerLicenseResponses != null && response.AcquireServerLicenseResponses.ServerLicenseCertificateChain != null)
			{
				RMUtil.TryConvertCertChainStringArrayToXmlNodeArray(response.AcquireServerLicenseResponses.ServerLicenseCertificateChain.CertificateChain, out certChain);
			}
			if (!RMUtil.IsValidCertificateChain(certChain))
			{
				this.perfcounters.WCFAcquireServerLicenseFailed();
				ExTraceGlobals.RightsManagementTracer.TraceError<Uri>((long)this.GetHashCode(), "Invalid response for the server license request from {0}. Cert chain is not valid", this.channelFactory.Endpoint.Address.Uri);
				RightsManagementException failureForCurrentBatch = new RightsManagementException(RightsManagementFailureCode.InvalidCertificateChain, DrmStrings.InvalidResponseToServerLicensingRequest(this.channelFactory.Endpoint.Address.Uri.AbsoluteUri));
				licenseState.SetFailureForCurrentBatch(failureForCurrentBatch);
				return;
			}
			LicenseResponse[] licenseResponses = response.AcquireServerLicenseResponses.LicenseResponses;
			if (licenseResponses == null || licenseResponses.Length != currentBatchLength)
			{
				this.perfcounters.WCFAcquireServerLicenseFailed();
				ExTraceGlobals.RightsManagementTracer.TraceError<Uri, int, int>((long)this.GetHashCode(), "Invalid response for the license request from {0}. Number of licenses {1}.  Expected: {2}", this.channelFactory.Endpoint.Address.Uri, (licenseResponses != null) ? licenseResponses.Length : 0, currentBatchLength);
				licenseState.SetFailureForCurrentBatch(new RightsManagementException(RightsManagementFailureCode.UseLicenseAcquisitionFailed, DrmStrings.FailedToAcquireUseLicense(this.channelFactory.Endpoint.Address.Uri.AbsoluteUri))
				{
					IsPermanent = false
				});
				return;
			}
			for (int i = 0; i < currentBatchLength; i++)
			{
				XmlNode license = null;
				if (licenseResponses[i] == null || string.IsNullOrEmpty(licenseResponses[i].EndUseLicense) || !RMUtil.TryConvertStringToXmlNode(licenseResponses[i].EndUseLicense, out license))
				{
					string ecode = RightsManagementFailureCode.UnknownFailure.ToString();
					bool isPermanent = true;
					if (licenseResponses[i] != null && licenseResponses[i].Fault != null)
					{
						ecode = licenseResponses[i].Fault.ErrorCode.ToString();
						isPermanent = licenseResponses[i].Fault.IsPermanentFailure;
					}
					licenseState.SetFailureForCurrentBatchElement(i, new RightsManagementException(RightsManagementFailureCode.ActiveFederationFault, DrmStrings.ExternalRmsServerFailure(this.channelFactory.Endpoint.Address.Uri, ecode))
					{
						IsPermanent = isPermanent
					});
				}
				else
				{
					licenseState.SetResponseForCurrentBatchElement(i, certChain, license);
				}
			}
			ExTraceGlobals.RightsManagementTracer.TraceDebug<Uri>((long)this.GetHashCode(), "Successfully parsed the license responses from {0}", this.channelFactory.Endpoint.Address.Uri);
			this.perfcounters.WCFAcquireServerLicenseSuccessful();
		}

		private ChannelFactory<IWSServerLicensingServiceChannel> channelFactory;

		private IWSServerLicensingServiceChannel channel;

		private Uri targetUri;

		private XrmlCertificateChain rac;
	}
}
