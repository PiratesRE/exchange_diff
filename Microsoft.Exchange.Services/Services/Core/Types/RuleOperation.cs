using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(DeleteRuleOperation))]
	[XmlInclude(typeof(SetRuleOperation))]
	[XmlType(TypeName = "RuleOperationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(CreateRuleOperation))]
	public abstract class RuleOperation
	{
		internal abstract void Execute(RuleOperationParser ruleOperationParser, Rules serverRules);
	}
}
