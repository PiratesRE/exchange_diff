using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	public interface IMailboxProvisioningConstraint
	{
		string Value { get; }

		bool IsMatch(MailboxProvisioningAttributes attributes);
	}
}
