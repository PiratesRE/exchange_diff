using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class SimplePolicyParserFactory : IPolicyParserFactory
	{
		public IEnumerable<string> GetSupportedActions()
		{
			return SimplePolicyParserFactory.supportedActions;
		}

		public PredicateCondition CreatePredicate(string predicateName, Property property, List<string> valueEntries)
		{
			return null;
		}

		public Action CreateAction(string actionName, List<Argument> arguments, string externalName)
		{
			if (actionName != null)
			{
				if (actionName == "Hold")
				{
					return new HoldAction(arguments, externalName);
				}
				if (actionName == "RetentionExpire")
				{
					return new RetentionExpireAction(arguments, externalName);
				}
				if (actionName == "RetentionRecycle")
				{
					return new RetentionRecycleAction(arguments, externalName);
				}
				if (actionName == "BlockAccess")
				{
					return new BlockAccessAction(arguments, externalName);
				}
				if (actionName == "GenerateIncidentReport")
				{
					return new GenerateIncidentReportAction(arguments, externalName);
				}
				if (actionName == "NotifyAuthors")
				{
					return new NotifyAuthorsAction(arguments, externalName);
				}
			}
			return null;
		}

		public Property CreateProperty(string propertyName, string typeName)
		{
			return null;
		}

		private static List<string> supportedActions = new List<string>
		{
			"Hold",
			"RetentionExpire",
			"RetentionRecycle",
			"BlockAccess",
			"GenerateIncidentReport",
			"NotifyAuthors"
		};
	}
}
