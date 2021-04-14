using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Common
{
	internal static class MailUserTaskHelper
	{
		public static void ValidateExternalEmailAddress(ADRecipient recipient, IConfigurationSession configurationSession, Task.ErrorLoggerDelegate writeError, ProvisioningCache provisioningCache)
		{
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.ValidateExternalEmailAddressInAcceptedDomain.Enabled)
			{
				SmtpProxyAddress smtpProxyAddress = recipient.ExternalEmailAddress as SmtpProxyAddress;
				if (smtpProxyAddress == null)
				{
					writeError(new RecipientTaskException(Strings.ErrorExternalEmailAddressNotSmtpAddress((recipient.ExternalEmailAddress == null) ? "$null" : recipient.ExternalEmailAddress.ToString())), ExchangeErrorCategory.Client, recipient.Identity);
					return;
				}
				if (RecipientTaskHelper.SMTPAddressCheckWithAcceptedDomain(configurationSession, recipient.OrganizationId, writeError, provisioningCache))
				{
					RecipientTaskHelper.ValidateInAcceptedDomain(configurationSession, recipient.OrganizationId, new SmtpAddress(smtpProxyAddress.SmtpAddress).Domain, writeError, provisioningCache);
				}
				recipient.EmailAddressPolicyEnabled = false;
				if (recipient.PrimarySmtpAddress == SmtpAddress.Empty)
				{
					recipient.PrimarySmtpAddress = new SmtpAddress(smtpProxyAddress.SmtpAddress);
				}
			}
		}
	}
}
