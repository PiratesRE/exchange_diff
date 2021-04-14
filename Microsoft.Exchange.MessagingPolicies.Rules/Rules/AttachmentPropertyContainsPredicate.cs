using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class AttachmentPropertyContainsPredicate : TextMatchingPredicate
	{
		public AttachmentPropertyContainsPredicate(ShortList<string> entries, RulesCreationContext creationContext) : base(null, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "attachmentPropertyContains";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return AttachmentPropertyContainsPredicate.AttachmentPropertyContainsBaseVersion;
			}
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			this.searchPropertyValues = AttachmentPropertyContainsPredicate.ParsePredicateParameters(entries);
			if (!this.searchPropertyValues.Any<KeyValuePair<string, List<string>>>())
			{
				throw new RulesValidationException(TransportRulesStrings.InvalidAttachmentPropertyParameter(this.Name));
			}
			return Value.CreateValue(typeof(string[]), entries);
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			transportRulesEvaluationContext.PredicateName = this.Name;
			foreach (KeyValuePair<string, List<string>> keyValuePair in this.searchPropertyValues)
			{
				foreach (IDictionary<string, string> dictionary in transportRulesEvaluationContext.Message.AttachmentProperties)
				{
					string text;
					if (dictionary.TryGetValue(keyValuePair.Key, out text))
					{
						bool flag = TransportUtils.IsMatchTplKeyword(text, keyValuePair.Key, keyValuePair.Value, transportRulesEvaluationContext);
						if (flag)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		internal static List<KeyValuePair<string, List<string>>> ParsePredicateParameters(IEnumerable<string> parameters)
		{
			List<KeyValuePair<string, List<string>>> list = new List<KeyValuePair<string, List<string>>>(parameters.Count<string>());
			foreach (string text in parameters)
			{
				int num = text.IndexOf(':');
				if (num >= 0 && num < text.Length - 1)
				{
					string text2 = text.Substring(0, num).Trim().ToLowerInvariant();
					if (!string.IsNullOrEmpty(text2))
					{
						List<string> list2 = (from w in text.Substring(num + 1).ToLowerInvariant().Split(new char[]
						{
							','
						})
						select w.Trim()).ToList<string>();
						if (!list2.Any(new Func<string, bool>(string.IsNullOrEmpty)))
						{
							list.Add(new KeyValuePair<string, List<string>>(text2, list2));
						}
					}
				}
			}
			return list;
		}

		internal static readonly Version AttachmentPropertyContainsBaseVersion = new Version("15.00.0014.00");

		private List<KeyValuePair<string, List<string>>> searchPropertyValues;
	}
}
