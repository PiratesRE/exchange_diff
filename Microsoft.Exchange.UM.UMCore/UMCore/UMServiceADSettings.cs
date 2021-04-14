using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMServiceADSettings : UMADSettings
	{
		public override ProtocolConnectionSettings SIPAccessService
		{
			get
			{
				return this.umServer.SIPAccessService;
			}
		}

		public override UMStartupMode UMStartupMode
		{
			get
			{
				return this.umServer.UMStartupMode;
			}
		}

		public override string UMCertificateThumbprint
		{
			get
			{
				return this.umServer.UMCertificateThumbprint;
			}
		}

		public override string Fqdn
		{
			get
			{
				return this.fqdn;
			}
		}

		public override UMSmartHost ExternalServiceFqdn
		{
			get
			{
				return this.umServer.ExternalServiceFqdn;
			}
		}

		public override IPAddressFamily IPAddressFamily
		{
			get
			{
				return this.umServer.IPAddressFamily;
			}
		}

		public override bool IPAddressFamilyConfigurable
		{
			get
			{
				return this.umServer.IPAddressFamilyConfigurable;
			}
		}

		public override string UMPodRedirectTemplate
		{
			get
			{
				return this.umServer.UMPodRedirectTemplate;
			}
		}

		public override string UMForwardingAddressTemplate
		{
			get
			{
				return this.umServer.UMForwardingAddressTemplate;
			}
		}

		public override int SipTcpListeningPort
		{
			get
			{
				return this.umServer.SipTcpListeningPort;
			}
		}

		public override int SipTlsListeningPort
		{
			get
			{
				return this.umServer.SipTlsListeningPort;
			}
		}

		public override ADObjectId Id
		{
			get
			{
				return this.id;
			}
		}

		public override bool IsSIPAccessServiceNeeded
		{
			get
			{
				return true;
			}
		}

		public UMServiceADSettings()
		{
			UMADSettings.ExecuteADOperation(delegate
			{
				ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
				Server localServer = adtopologyLookup.GetLocalServer();
				if (localServer == null)
				{
					throw new ExchangeServerNotFoundException(Utils.GetLocalHostName());
				}
				this.umServer = new UMServer(localServer);
				this.fqdn = localServer.Fqdn;
				this.id = localServer.Id;
			});
		}

		internal override void SubscribeToADNotifications(ADNotificationEvent notificationHandler)
		{
			ADNotificationsManager.Instance.Server.ConfigChanged += notificationHandler;
		}

		internal override UMADSettings RefreshFromAD()
		{
			return new UMServiceADSettings();
		}

		private string fqdn;

		private UMServer umServer;

		private ADObjectId id;
	}
}
