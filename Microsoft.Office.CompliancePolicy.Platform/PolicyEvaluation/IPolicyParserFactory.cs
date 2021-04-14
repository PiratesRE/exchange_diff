using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public interface IPolicyParserFactory
	{
		IEnumerable<string> GetSupportedActions();

		PredicateCondition CreatePredicate(string predicateName, Property property, List<string> valueEntries);

		Action CreateAction(string actionName, List<Argument> arguments, string externalName);

		Property CreateProperty(string propertyName, string typeName);
	}
}
