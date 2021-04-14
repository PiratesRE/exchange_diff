using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class MatchesPatternsPredicate : TransportRulePredicate
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

		internal abstract MatchesPatternsPredicate.LocalizedStringDescriptionDelegate LocalizedStringDescription { get; }

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

		internal static void ValidateReadMatchesPatternsPredicate(Pattern[] patterns, bool useLegacyRegex, string name, List<ValidationError> errors)
		{
			if (patterns == null || patterns.Length == 0)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, name));
				return;
			}
			foreach (Pattern pattern in patterns)
			{
				string value = pattern.Value;
				if (!string.IsNullOrEmpty(value))
				{
					int index;
					if (!Utils.CheckIsUnicodeStringWellFormed(value, out index))
					{
						errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.CommentsHaveInvalidChars((int)value[index]), name));
					}
					else
					{
						try
						{
							Pattern.ValidatePattern(value, useLegacyRegex, false);
						}
						catch (ValidationArgumentException ex)
						{
							LocalizedString description = ValidationError.CombineErrorDescriptions(new List<ValidationError>
							{
								new RulePhrase.RulePhraseValidationError(RulesTasksStrings.InvalidRegex(value), name),
								new RulePhrase.RulePhraseValidationError(ex.LocalizedString, name)
							});
							errors.Add(new RulePhrase.RulePhraseValidationError(description, name));
						}
						catch (ArgumentException)
						{
							errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.InvalidRegex(value), name));
						}
					}
				}
			}
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
