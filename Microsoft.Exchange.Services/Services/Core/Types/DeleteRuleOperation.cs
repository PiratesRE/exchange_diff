using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DeleteRuleOperationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class DeleteRuleOperation : RuleOperation
	{
		[XmlElement]
		public string RuleId { get; set; }

		internal override void Execute(RuleOperationParser ruleOperationParser, Rules serverRules)
		{
			Rule rule = ruleOperationParser.ParseRuleId(this.RuleId, 0);
			if (!ruleOperationParser.HasValidationError)
			{
				rule.MarkDelete();
				ruleOperationParser.AddDeletedRule(rule);
			}
		}
	}
}
