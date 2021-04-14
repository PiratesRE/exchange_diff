using System;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AttachmentContainsWordsPredicate : ContainsPredicate
	{
		public AttachmentContainsWordsPredicate(ShortList<string> entries, RulesCreationContext creationContext) : base(null, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "attachmentContainsWords";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return TransportRuleConstants.VersionedContainerBaseVersion;
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			transportRulesEvaluationContext.PredicateName = this.Name;
			return AttachmentMatcher.AttachmentMatches(base.Value, baseContext, new AttachmentMatcher.TracingDelegate(ExTraceGlobals.TransportRulesEngineTracer.TraceDebug));
		}
	}
}
