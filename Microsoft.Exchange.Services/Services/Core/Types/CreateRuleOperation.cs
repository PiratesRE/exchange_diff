using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "CreateRuleOperationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class CreateRuleOperation : RuleOperation
	{
		[XmlElement]
		public EwsRule Rule { get; set; }

		internal override void Execute(RuleOperationParser ruleOperationParser, Rules serverRules)
		{
			ruleOperationParser.ValidateRuleField(() => string.IsNullOrEmpty(this.Rule.RuleId), RuleValidationErrorCode.CreateWithRuleId, CoreResources.RuleErrorCreateWithRuleId, RuleFieldURI.RuleId, this.Rule.RuleId);
			this.Rule.ServerRule = Microsoft.Exchange.Data.Storage.Rule.Create(serverRules);
			ruleOperationParser.ParseRule(this.Rule);
			if (!ruleOperationParser.HasValidationError)
			{
				ruleOperationParser.InsertParsedRule(this.Rule.ServerRule);
			}
		}
	}
}
