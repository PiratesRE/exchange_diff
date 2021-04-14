using System;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	public abstract class Visitor15
	{
		internal abstract void Visit(PolicyNudges15 policyNudges);

		internal abstract void Visit(PolicyNudgeRules15 policyNudgeRules);

		internal abstract void Visit(PolicyNudgeRule15 policyNudgeRule);

		internal abstract void Visit(ClassificationItems15 classificationItems);

		internal abstract void Visit(ClassificationDefinitions15 classificationDefinitions);

		internal abstract void Visit(ClassificationDefinition15 classificationDefinition);
	}
}
