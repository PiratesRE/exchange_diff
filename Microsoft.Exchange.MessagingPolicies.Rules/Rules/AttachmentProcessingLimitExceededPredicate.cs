using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Filtering;
using Microsoft.Filtering.Results;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AttachmentProcessingLimitExceededPredicate : PredicateCondition
	{
		public AttachmentProcessingLimitExceededPredicate(ShortList<string> entries, RulesCreationContext creationContext) : base(null, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "attachmentProcessingLimitExceeded";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return AttachmentProcessingLimitExceededPredicate.AttachmentProcessingLimitExceededBaseVersion;
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
			IEnumerable<StreamIdentity> source = from streamIdentity in transportRulesEvaluationContext.Message.GetSupportedAttachmentStreamIdentities()
			where RuleAgentResultUtils.HasExceededProcessingLimit(streamIdentity)
			select streamIdentity;
			return source.Any<StreamIdentity>();
		}

		internal static readonly Version AttachmentProcessingLimitExceededBaseVersion = new Version("15.00.0004.00");
	}
}
