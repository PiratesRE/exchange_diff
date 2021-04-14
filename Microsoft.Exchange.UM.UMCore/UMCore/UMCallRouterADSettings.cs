using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMCallRouterADSettings : UMADSettings
	{
		public override ProtocolConnectionSettings SIPAccessService
		{
			get
			{
				return null;
			}
		}

		public override UMStartupMode UMStartupMode
		{
			get
			{
				return this.routerSettings.UMStartupMode;
			}
		}

		public override string UMCertificateThumbprint
		{
			get
			{
				return this.routerSettings.UMCertificateThumbprint;
			}
		}

		public override string Fqdn
		{
			get
			{
				return Utils.GetLocalHostFqdn();
			}
		}

		public override UMSmartHost ExternalServiceFqdn
		{
			get
			{
				return this.routerSettings.ExternalServiceFqdn;
			}
		}

		public override IPAddressFamily IPAddressFamily
		{
			get
			{
				return this.routerSettings.IPAddressFamily;
			}
		}

		public override bool IPAddressFamilyConfigurable
		{
			get
			{
				return this.routerSettings.IPAddressFamilyConfigurable;
			}
		}

		public override string UMPodRedirectTemplate
		{
			get
			{
				return this.routerSettings.UMPodRedirectTemplate;
			}
		}

		public override string UMForwardingAddressTemplate
		{
			get
			{
				return this.routerSettings.UMForwardingAddressTemplate;
			}
		}

		public override int SipTcpListeningPort
		{
			get
			{
				return this.routerSettings.SipTcpListeningPort;
			}
		}

		public override int SipTlsListeningPort
		{
			get
			{
				return this.routerSettings.SipTlsListeningPort;
			}
		}

		public override ADObjectId Id
		{
			get
			{
				return this.routerSettings.Id;
			}
		}

		public override bool IsSIPAccessServiceNeeded
		{
			get
			{
				return false;
			}
		}

		public UMCallRouterADSettings()
		{
			UMADSettings.ExecuteADOperation(delegate
			{
				ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
				this.routerSettings = adtopologyLookup.GetLocalCallRouterSettings();
				if (this.routerSettings == null)
				{
					throw new ExchangeServerNotFoundException(Utils.GetLocalHostName());
				}
			});
		}

		internal override void SubscribeToADNotifications(ADNotificationEvent notificationHandler)
		{
			ADNotificationsManager.Instance.CallRouterSettings.ConfigChanged += notificationHandler;
		}

		internal override UMADSettings RefreshFromAD()
		{
			return new UMCallRouterADSettings();
		}

		private SIPFEServerConfiguration routerSettings;
	}
}
