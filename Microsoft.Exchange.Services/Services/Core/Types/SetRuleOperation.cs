using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SetRuleOperationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class SetRuleOperation : RuleOperation
	{
		[XmlElement]
		public EwsRule Rule { get; set; }

		internal override void Execute(RuleOperationParser ruleOperationParser, Rules serverRules)
		{
			int ruleIndex = this.Rule.Priority + 10 - 1;
			this.Rule.ServerRule = ruleOperationParser.ParseRuleId(this.Rule.RuleId, ruleIndex);
			if (this.Rule.ServerRule == null)
			{
				this.Rule.ServerRule = Microsoft.Exchange.Data.Storage.Rule.Create(serverRules);
			}
			ruleOperationParser.ParseRule(this.Rule);
			if (!ruleOperationParser.HasValidationError)
			{
				ruleOperationParser.InsertParsedRule(this.Rule.ServerRule);
			}
		}
	}
}
