using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	internal class ProxyProbeCommon
	{
		internal static bool TryGetSmtpServer(out string smtpServer)
		{
			string exeConfigFilename = Path.Combine(ExchangeSetupContext.BinPath, "EdgeTransport.exe.config");
			Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
			{
				ExeConfigFilename = exeConfigFilename
			}, ConfigurationUserLevel.None);
			string value = configuration.AppSettings.Settings["OutboundFrontendServers"].Value;
			List<RoutingHost> configListFromValue = TransportAppConfig.GetConfigListFromValue<RoutingHost>(value, ',', new TransportAppConfig.TryParse<RoutingHost>(RoutingHost.TryParse));
			if (configListFromValue.Count <= 0)
			{
				smtpServer = null;
				return false;
			}
			smtpServer = configListFromValue[0].ToString();
			return true;
		}

		internal static void ApplyCertificateCriteria(SmtpConnectionProbeWorkDefinition workDefinition, Dictionary<string, string> attributes)
		{
			workDefinition.IgnoreCertificateNameMismatchPolicyError = true;
			workDefinition.ClientCertificate = new ClientCertificateCriteria();
			workDefinition.ClientCertificate.StoreLocation = StoreLocation.LocalMachine;
			workDefinition.ClientCertificate.StoreName = StoreName.My;
			workDefinition.ClientCertificate.FindType = X509FindType.FindBySubjectName;
			string exeConfigFilename = Path.Combine(ExchangeSetupContext.BinPath, "EdgeTransport.exe.config");
			Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
			{
				ExeConfigFilename = exeConfigFilename
			}, ConfigurationUserLevel.None);
			KeyValueConfigurationElement keyValueConfigurationElement = configuration.AppSettings.Settings["OutboundProxyExternalCertificateSubject"];
			workDefinition.ClientCertificate.TransportCertificateFqdn = ((keyValueConfigurationElement != null) ? keyValueConfigurationElement.Value : null);
			string findValue;
			if (string.IsNullOrEmpty(workDefinition.ClientCertificate.TransportCertificateFqdn) && attributes.TryGetValue("CertificateName", out findValue))
			{
				workDefinition.ClientCertificate.FindValue = findValue;
			}
			if (string.IsNullOrEmpty(workDefinition.ClientCertificate.FindValue) && string.IsNullOrEmpty(workDefinition.ClientCertificate.TransportCertificateFqdn))
			{
				workDefinition.ClientCertificate.FindValue = Utils.TryGetStringValueFromRegistry("SOFTWARE\\Microsoft\\ExchangeLabs", "ExoDatacenterCertificateName", "*.outlook.com");
			}
			if (string.IsNullOrEmpty(workDefinition.ClientCertificate.FindValue) && string.IsNullOrEmpty(workDefinition.ClientCertificate.TransportCertificateFqdn))
			{
				throw new SmtpConnectionProbeException("Unable to assign required proxy certificate lookup value.");
			}
		}

		private const string ExchangeLabsRegKey = "SOFTWARE\\Microsoft\\ExchangeLabs";

		private const string DefaultCertificateNameRegFieldName = "ExoDatacenterCertificateName";
	}
}
