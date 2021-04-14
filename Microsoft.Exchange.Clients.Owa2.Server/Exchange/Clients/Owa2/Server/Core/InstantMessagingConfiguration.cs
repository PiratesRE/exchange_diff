using System;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class InstantMessagingConfiguration
	{
		private InstantMessagingConfiguration(VdirConfiguration vdirConfiguration)
		{
			this.vdirConfiguration = vdirConfiguration;
		}

		public string ServerName
		{
			get
			{
				if (this.vdirConfiguration == null)
				{
					return BaseApplication.GetAppSetting<string>("IMServerName", string.Empty);
				}
				return this.vdirConfiguration.InstantMessagingServerName;
			}
		}

		public string CertificateThumbprint
		{
			get
			{
				if (this.vdirConfiguration == null)
				{
					return BaseApplication.GetAppSetting<string>("IMCertificateThumbprint", string.Empty);
				}
				return this.vdirConfiguration.InstantMessagingCertificateThumbprint;
			}
		}

		public int PortNumber
		{
			get
			{
				return BaseApplication.GetAppSetting<int>("IMPortNumber", -1);
			}
		}

		public static InstantMessagingConfiguration GetInstance(VdirConfiguration vdirConfiguration)
		{
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.UseVdirConfigForInstantMessaging.Enabled)
			{
				return new InstantMessagingConfiguration(vdirConfiguration);
			}
			return new InstantMessagingConfiguration(null);
		}

		public bool CheckConfiguration()
		{
			bool result = true;
			string arg = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.UseVdirConfigForInstantMessaging.Enabled ? "OWA Virtual Directory object" : "web.config";
			if (string.IsNullOrWhiteSpace(this.ServerName))
			{
				result = false;
				OwaDiagnostics.PublishMonitoringEventNotification(ExchangeComponent.OwaDependency.Name, Feature.InstantMessage.ToString(), string.Format("Instant Messaging Server name is null or empty on {0}.", arg), ResultSeverityLevel.Error);
			}
			if (string.IsNullOrWhiteSpace(this.CertificateThumbprint))
			{
				result = false;
				OwaDiagnostics.PublishMonitoringEventNotification(ExchangeComponent.OwaDependency.Name, Feature.InstantMessage.ToString(), string.Format("Instant Messaging Certificate Thumbprint is null or empty on {0}.", arg), ResultSeverityLevel.Error);
			}
			return result;
		}

		public const string ServerNameKey = "IMServerName";

		public const string PortNumberKey = "IMPortNumber";

		public const string CertificateThumbprintKey = "IMCertificateThumbprint";

		private VdirConfiguration vdirConfiguration;
	}
}
