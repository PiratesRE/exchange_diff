using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RuleActionConverter : IDataConverter<RuleAction, RuleActionData>
	{
		RuleAction IDataConverter<RuleAction, RuleActionData>.GetNativeRepresentation(RuleActionData rad)
		{
			return rad.GetRuleAction();
		}

		RuleActionData IDataConverter<RuleAction, RuleActionData>.GetDataRepresentation(RuleAction ra)
		{
			return RuleActionData.GetRuleActionData(ra);
		}
	}
}
