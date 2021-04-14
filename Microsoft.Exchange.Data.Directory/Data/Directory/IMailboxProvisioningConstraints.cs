using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	public interface IMailboxProvisioningConstraints
	{
		IMailboxProvisioningConstraint HardConstraint { get; }

		IMailboxProvisioningConstraint[] SoftConstraints { get; }

		bool IsMatch(MailboxProvisioningAttributes attributes);
	}
}
