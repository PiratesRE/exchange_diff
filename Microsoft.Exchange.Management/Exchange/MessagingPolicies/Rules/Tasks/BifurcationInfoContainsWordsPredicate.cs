using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class BifurcationInfoContainsWordsPredicate : BifurcationInfoPredicate
	{
		public abstract Word[] Words { get; set; }

		internal override void Reset()
		{
			this.words = null;
			base.Reset();
		}

		internal override string Description
		{
			get
			{
				return this.LocalizedStringDescription(RuleDescription.BuildDescriptionStringFromStringArray(Utils.BuildWordStringList(this.Words), RulesTasksStrings.RuleDescriptionOrDelimiter, base.MaxDescriptionListLength));
			}
		}

		internal abstract BifurcationInfoContainsWordsPredicate.LocalizedStringDescriptionDelegate LocalizedStringDescription { get; }

		protected override void ValidateRead(List<ValidationError> errors)
		{
			ContainsWordsPredicate.ValidateReadContainsWordsPredicate(this.words, base.Name, errors);
			base.ValidateRead(errors);
		}

		internal override string GetPredicateParameters()
		{
			return string.Join(", ", (from w in this.Words
			select Utils.QuoteCmdletParameter(w.ToString())).ToArray<string>());
		}

		internal override void SuppressPiiData()
		{
			string[] array;
			string[] array2;
			this.Words = SuppressingPiiData.Redact(this.Words, out array, out array2);
		}

		protected Word[] words;

		internal delegate LocalizedString LocalizedStringDescriptionDelegate(string words);
	}
}
