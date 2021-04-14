using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Filtering;
using Microsoft.Filtering.Results;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AttachmentIsUnsupportedPredicate : PredicateCondition
	{
		public AttachmentIsUnsupportedPredicate(ShortList<string> entries, RulesCreationContext creationContext) : base(null, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "attachmentIsUnsupported";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return TransportRuleConstants.VersionedContainerBaseVersion;
			}
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			if (entries.Count != 0)
			{
				throw new RulesValidationException(RulesStrings.ValueIsNotAllowed(this.Name));
			}
			return null;
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			transportRulesEvaluationContext.PredicateName = this.Name;
			bool result;
			try
			{
				IEnumerable<StreamIdentity> source = from streamIdentity in transportRulesEvaluationContext.Message.GetAttachmentStreamIdentities()
				where RuleAgentResultUtils.IsUnsupported(streamIdentity)
				select streamIdentity;
				result = source.Any<StreamIdentity>();
			}
			catch (FilteringServiceFailureException ex)
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string>(0L, "Exception (FIPS-related) encountered while getting the extracted content information from the attachment.  The attachment will be treated as unsupported. Error: {0}", ex.ToString());
				result = true;
			}
			return result;
		}
	}
}
