using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Shared.Providers
{
	internal static class ConfigurationProvider
	{
		public static IConfigurationProvider ConfigurationProviderInstance
		{
			get
			{
				if (ConfigurationProvider.configurationProvider == null)
				{
					ConfigurationProvider.configurationProvider = ADConfigurationProvider.Instance;
				}
				return ConfigurationProvider.configurationProvider;
			}
			set
			{
				ConfigurationProvider.configurationProvider = value;
			}
		}

		public static OutboundConversionOptions GetGlobalConversionOptions()
		{
			return ConfigurationProvider.ConfigurationProviderInstance.GetGlobalConversionOptions();
		}

		public static string GetDefaultDomainName()
		{
			return ConfigurationProvider.ConfigurationProviderInstance.GetDefaultDomainName();
		}

		public static bool TryGetDefaultDomainName(OrganizationId organizationId, out string domainName)
		{
			return ConfigurationProvider.ConfigurationProviderInstance.TryGetDefaultDomainName(organizationId, out domainName);
		}

		public static void SendNDRForInvalidAddresses(IReadOnlyMailItem mailItemToSubmit, List<DsnRecipientInfo> invalidRecipients, DsnMailOutHandlerDelegate dsnMailOutHandler)
		{
			ConfigurationProvider.ConfigurationProviderInstance.SendNDRForInvalidAddresses(mailItemToSubmit, invalidRecipients, dsnMailOutHandler);
		}

		public static void SendNDRForFailedSubmission(IReadOnlyMailItem ndrMailItem, SmtpResponse ndrReason, DsnMailOutHandlerDelegate dsnMailOutHandler)
		{
			ConfigurationProvider.ConfigurationProviderInstance.SendNDRForFailedSubmission(ndrMailItem, ndrReason, dsnMailOutHandler);
		}

		public static string GetQuarantineMailbox()
		{
			return ConfigurationProvider.ConfigurationProviderInstance.GetQuarantineMailbox();
		}

		public static bool GetForwardingProhibitedFeatureStatus()
		{
			return ConfigurationProvider.ConfigurationProviderInstance.GetForwardingProhibitedFeatureStatus();
		}

		private static IConfigurationProvider configurationProvider;
	}
}
