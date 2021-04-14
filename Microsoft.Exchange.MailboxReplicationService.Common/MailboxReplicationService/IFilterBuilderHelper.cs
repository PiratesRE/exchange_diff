using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IFilterBuilderHelper
	{
		PropTag MapNamedProperty(NamedPropData npd, PropType propType);

		string[] MapRecipient(string recipientId);

		Guid[] MapPolicyTag(string policyTagStr);
	}
}
