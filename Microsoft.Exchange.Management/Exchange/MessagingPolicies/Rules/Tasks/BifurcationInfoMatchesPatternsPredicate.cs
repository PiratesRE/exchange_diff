using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class BifurcationInfoMatchesPatternsPredicate : BifurcationInfoPredicate
	{
		public abstract Pattern[] Patterns { get; set; }

		public bool UseLegacyRegex
		{
			get
			{
				return this.useLegacyRegex;
			}
			set
			{
				this.useLegacyRegex = value;
			}
		}

		internal override void Reset()
		{
			this.patterns = null;
			this.UseLegacyRegex = false;
			base.Reset();
		}

		internal override string Description
		{
			get
			{
				return this.LocalizedStringDescription(RuleDescription.BuildDescriptionStringFromStringArray(Utils.BuildPatternStringList(this.Patterns), RulesTasksStrings.RuleDescriptionOrDelimiter, base.MaxDescriptionListLength));
			}
		}

		internal abstract BifurcationInfoMatchesPatternsPredicate.LocalizedStringDescriptionDelegate LocalizedStringDescription { get; }

		protected override void ValidateRead(List<ValidationError> errors)
		{
			MatchesPatternsPredicate.ValidateReadMatchesPatternsPredicate(this.patterns, this.UseLegacyRegex, base.Name, errors);
			base.ValidateRead(errors);
		}

		internal override string GetPredicateParameters()
		{
			return string.Join(", ", (from p in this.patterns
			select Utils.QuoteCmdletParameter(p.ToString())).ToArray<string>());
		}

		internal override void SuppressPiiData()
		{
			this.Patterns = Utils.RedactPatterns(this.Patterns);
		}

		private bool useLegacyRegex;

		protected Pattern[] patterns;

		internal delegate LocalizedString LocalizedStringDescriptionDelegate(string patterns);
	}
}
