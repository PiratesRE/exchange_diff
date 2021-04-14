using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RuleConverter : IDataConverter<Rule, RuleData>
	{
		Rule IDataConverter<Rule, RuleData>.GetNativeRepresentation(RuleData rd)
		{
			return rd.GetRule();
		}

		RuleData IDataConverter<Rule, RuleData>.GetDataRepresentation(Rule r)
		{
			return RuleData.Create(r);
		}
	}
}
