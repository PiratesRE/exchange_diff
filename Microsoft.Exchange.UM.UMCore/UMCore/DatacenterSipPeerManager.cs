using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DatacenterSipPeerManager : SipPeerManager
	{
		internal override UMSipPeer AVAuthenticationService
		{
			get
			{
				return this.avAuthenticationService;
			}
		}

		internal override UMSipPeer SIPAccessService
		{
			get
			{
				return this.sipAccessService;
			}
		}

		internal override bool IsHeuristicBasedSIPAccessService
		{
			get
			{
				return false;
			}
		}

		internal override UMSipPeer SBCService
		{
			get
			{
				return this.sbcService;
			}
		}

		internal DatacenterSipPeerManager(UMADSettings adSettings) : base(adSettings)
		{
			this.sipPeerList = new List<UMSipPeer>();
			Organization organization = this.ReadForestWideSettings();
			ProtocolConnectionSettings sipaccessService = organization.SIPAccessService;
			if (sipaccessService != null)
			{
				this.sipAccessService = new UMSipPeer(new UMSmartHost(sipaccessService.Hostname.HostnameString), sipaccessService.Port, true, true, true, IPAddressFamily.Any);
				this.sipPeerList.Add(this.sipAccessService);
			}
			ProtocolConnectionSettings avauthenticationService = organization.AVAuthenticationService;
			if (avauthenticationService != null)
			{
				this.avAuthenticationService = new UMSipPeer(new UMSmartHost(avauthenticationService.Hostname.HostnameString), avauthenticationService.Port, true, true, IPAddressFamily.Any);
			}
			ProtocolConnectionSettings sipsessionBorderController = organization.SIPSessionBorderController;
			if (sipsessionBorderController != null)
			{
				this.sbcService = new UMSipPeer(new UMSmartHost(sipsessionBorderController.Hostname.HostnameString), sipsessionBorderController.Port, true, true, IPAddressFamily.Any);
				this.sipPeerList.Add(this.sbcService);
			}
			UMSipPeer item = new UMSipPeer(new UMSmartHost(Utils.GetOwnerHostFqdn()), base.ServiceADSettings.SipTlsListeningPort, true, true, IPAddressFamily.Any);
			this.sipPeerList.Add(item);
			UMSipPeer sipPeerFromUMCertificateSubjectName = base.GetSipPeerFromUMCertificateSubjectName();
			if (sipPeerFromUMCertificateSubjectName != null)
			{
				this.sipPeerList.Add(sipPeerFromUMCertificateSubjectName);
			}
		}

		internal override List<UMSipPeer> GetSecuredSipPeers()
		{
			return this.sipPeerList;
		}

		internal override bool IsAccessProxy(string matchedFqdn)
		{
			return DatacenterSipPeerManager.IsMatchedFqdnForSipPeer(matchedFqdn, this.SIPAccessService);
		}

		internal override bool IsAccessProxyWithOrgTestHook(string matchedFqdn, string orgParameter)
		{
			bool flag = this.IsAccessProxy(matchedFqdn);
			if (flag && Utils.RunningInTestMode && string.IsNullOrEmpty(orgParameter))
			{
				flag = false;
			}
			return flag;
		}

		internal override bool IsSIPSBC(string matchedFqdn)
		{
			return DatacenterSipPeerManager.IsMatchedFqdnForSipPeer(matchedFqdn, this.SBCService);
		}

		protected override void LogUnknownTcpGateway(string remoteEndPoint)
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallRejected, null, new object[]
			{
				(CallId.Id == null) ? "<null>" : CallId.Id,
				Strings.CallFromUnknownTcpGateway(remoteEndPoint).ToString()
			});
		}

		protected override void LogUnknownTlsGateway(X509Certificate2 certificate, string matchedFqdn, string remoteEndPoint)
		{
			string text = Strings.CallFromUnknownTlsGateway(remoteEndPoint, certificate.Thumbprint, TlsCertificateInfo.GetFQDNs(certificate).ToString(","));
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallRejected, null, new object[]
			{
				(CallId.Id == null) ? "<null>" : CallId.Id,
				text
			});
		}

		protected override void OnLocalServerUpdated()
		{
		}

		protected override UMIPGateway FindGatewayForTlsEndPoint(X509Certificate2 certificate, ReadOnlyCollection<PlatformSignalingHeader> sipHeaders, PlatformSipUri requestUri, Guid tenantGuid, ref string matchedFqdn)
		{
			base.DebugTrace("DatacenterSipPeerManager::FindGatewayForTlsEndPoint.", new object[0]);
			UMIPGateway umipgateway = null;
			List<string> fqdns = null;
			bool isMonitoring = false;
			string orgParameter = (requestUri != null) ? requestUri.FindParameter("ms-organization") : string.Empty;
			if (this.IsAccessProxyWithOrgTestHook(matchedFqdn, orgParameter))
			{
				base.DebugTrace("Inbound call from Access Proxy", new object[0]);
				umipgateway = this.GetSipAccessServiceGateway(tenantGuid);
			}
			else if (this.IsSIPSBC(matchedFqdn))
			{
				base.DebugTrace("Inbound call from SBC", new object[0]);
				if (DatacenterSipPeerManager.CheckIfCertificateHeadersPresent(sipHeaders, out fqdns, out isMonitoring))
				{
					umipgateway = base.GetUMIPGateway(fqdns, tenantGuid, isMonitoring);
				}
				else
				{
					base.DebugTrace("Received a TLS call from SBC but no remote certificate fqdn(s) were passed in the header", new object[0]);
				}
			}
			else
			{
				base.DebugTrace("Inbound call from an unknown source", new object[0]);
			}
			if (umipgateway != null)
			{
				string text = umipgateway.Address.ToString();
				base.DebugTrace("Populating matched FQDN. Old Value={0}, New Value={1}", new object[]
				{
					matchedFqdn,
					text
				});
				matchedFqdn = text;
			}
			return umipgateway;
		}

		private static bool CheckIfCertificateHeadersPresent(ReadOnlyCollection<PlatformSignalingHeader> sipHeaders, out List<string> certFqdns, out bool isMonitoring)
		{
			certFqdns = new List<string>();
			isMonitoring = false;
			if (sipHeaders != null)
			{
				foreach (PlatformSignalingHeader platformSignalingHeader in sipHeaders)
				{
					if (string.Equals(platformSignalingHeader.Name, "P-Certificate-Subject-Common-Name", StringComparison.InvariantCultureIgnoreCase) || string.Equals(platformSignalingHeader.Name, "P-Certificate-Subject-Alternative-Name", StringComparison.InvariantCultureIgnoreCase))
					{
						certFqdns.Add(platformSignalingHeader.Value);
						if (string.Equals(platformSignalingHeader.Value, "um.o365.exchangemon.net", StringComparison.InvariantCultureIgnoreCase))
						{
							isMonitoring = true;
						}
					}
				}
			}
			return certFqdns.Count > 0;
		}

		private static bool IsMatchedFqdnForSipPeer(string matchedFqdn, UMSipPeer peer)
		{
			return peer != null && string.Equals(peer.Address.ToString(), matchedFqdn, StringComparison.OrdinalIgnoreCase);
		}

		private Organization ReadForestWideSettings()
		{
			CachedOrganizationConfiguration instance = CachedOrganizationConfiguration.GetInstance(OrganizationId.ForestWideOrgId, CachedOrganizationConfiguration.ConfigurationTypes.OrganizationConfiguration);
			return instance.OrganizationConfiguration.Configuration;
		}

		private UMIPGateway GetSipAccessServiceGateway(Guid tenantGuid)
		{
			base.DebugTrace("DatacenterSipPeerManager.GetSipAccessServiceGateway tenantGuid={0}", new object[]
			{
				tenantGuid
			});
			UMIPGateway result = null;
			try
			{
				ADSessionSettings adsessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(tenantGuid);
				result = this.SIPAccessService.ToUMIPGateway(adsessionSettings.CurrentOrganizationId);
			}
			catch (Exception ex)
			{
				base.DebugTrace("DatacenterSipPeerManager.GetSipAccessServiceGateway -  An exception occurred {0}", new object[]
				{
					ex.Message
				});
				if (!base.HandleADException(ex))
				{
					throw;
				}
			}
			return result;
		}

		private readonly List<UMSipPeer> sipPeerList;

		private UMSipPeer avAuthenticationService;

		private UMSipPeer sipAccessService;

		private UMSipPeer sbcService;
	}
}
