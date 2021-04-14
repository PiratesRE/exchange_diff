using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Filtering;
using Microsoft.Filtering.Results;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AttachmentIsPasswordProtectedPredicate : PredicateCondition
	{
		public AttachmentIsPasswordProtectedPredicate(ShortList<string> entries, RulesCreationContext creationContext) : base(null, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "attachmentIsPasswordProtected";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return AttachmentIsPasswordProtectedPredicate.AttachmentIsPasswordProtectedBaseVersion;
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
				where RuleAgentResultUtils.IsEncrypted(streamIdentity)
				select streamIdentity;
				result = source.Any<StreamIdentity>();
			}
			catch (Exception ex)
			{
				if (!TransportRulesErrorHandler.IsKnownFipsException(ex) || TransportRulesErrorHandler.IsTimeoutException(ex))
				{
					throw;
				}
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string>(0L, "Exception (FIPS-related) encountered while getting the extracted content information from the attachment.  The attachment will be treated as non-password protected. Error: {0}", ex.ToString());
				result = false;
			}
			return result;
		}

		internal static readonly Version AttachmentIsPasswordProtectedBaseVersion = new Version("15.00.0005.01");
	}
}
