using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Shared.Providers
{
	internal interface IConfigurationProvider
	{
		OutboundConversionOptions GetGlobalConversionOptions();

		string GetDefaultDomainName();

		bool TryGetDefaultDomainName(OrganizationId organizationId, out string domainName);

		void SendNDRForInvalidAddresses(IReadOnlyMailItem mailItemToSubmit, List<DsnRecipientInfo> invalidRecipients, DsnMailOutHandlerDelegate dsnMailOutHandler);

		void SendNDRForFailedSubmission(IReadOnlyMailItem ndrMailItem, SmtpResponse ndrReason, DsnMailOutHandlerDelegate dsnMailOutHandler);

		string GetQuarantineMailbox();

		bool GetForwardingProhibitedFeatureStatus();
	}
}
