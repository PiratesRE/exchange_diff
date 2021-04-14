using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class TransportRule : Rule
	{
		public TransportRule(string name) : this(name, null)
		{
			this.SenderAddressLocation = SenderAddressLocation.Header;
		}

		public TransportRule(string name, Condition condition) : base(name, condition)
		{
			this.fork = new List<RuleBifurcationInfo>();
			this.SenderAddressLocation = SenderAddressLocation.Header;
		}

		public List<RuleBifurcationInfo> Fork
		{
			get
			{
				return this.fork;
			}
			set
			{
				this.fork = value;
			}
		}

		public int RegexCharacterCount
		{
			get
			{
				int num = 0;
				foreach (RuleBifurcationInfo ruleBifurcationInfo in this.Fork)
				{
					foreach (string text in ruleBifurcationInfo.Patterns)
					{
						num += text.Length;
					}
				}
				num += this.CountRegexCharsForCondition(base.Condition);
				return num;
			}
		}

		public List<string> RecipientsAddedByActions
		{
			get
			{
				List<string> list = new List<string>();
				foreach (Action action in base.Actions)
				{
					if (string.Equals(action.Name, "AddToRecipientSmtpOnly", StringComparison.InvariantCultureIgnoreCase) || string.Equals(action.Name, "AddCcRecipientSmtpOnly", StringComparison.InvariantCultureIgnoreCase) || string.Equals(action.Name, "AddEnvelopeRecipient", StringComparison.InvariantCultureIgnoreCase) || string.Equals(action.Name, "RedirectMessage", StringComparison.InvariantCultureIgnoreCase) || string.Equals(action.Name, "GenerateIncidentReport", StringComparison.InvariantCultureIgnoreCase) || string.Equals(action.Name, "GenerateNotification", StringComparison.InvariantCultureIgnoreCase))
					{
						Value value = action.Arguments[0] as Value;
						if (value != null)
						{
							list.Add((string)value.ParsedValue);
						}
					}
				}
				return list;
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return this.ComputeMinVersion();
			}
		}

		public SenderAddressLocation SenderAddressLocation { get; set; }

		public override int GetEstimatedSize()
		{
			int num = 0;
			foreach (RuleBifurcationInfo ruleBifurcationInfo in this.Fork)
			{
				num += ruleBifurcationInfo.GetEstimatedSize();
			}
			num += 4;
			return num + base.GetEstimatedSize();
		}

		public TransportActionType MostSpecificActionType
		{
			get
			{
				TransportActionType transportActionType = TransportActionType.NonRecipientRelated;
				foreach (Action action in base.Actions)
				{
					TransportAction transportAction = (TransportAction)action;
					if (transportAction.Type == TransportActionType.RecipientRelated && transportActionType == TransportActionType.NonRecipientRelated)
					{
						transportActionType = transportAction.Type;
					}
					else if (transportAction.Type == TransportActionType.BifurcationNeeded && transportActionType != TransportActionType.BifurcationNeeded)
					{
						transportActionType = transportAction.Type;
					}
				}
				return transportActionType;
			}
		}

		public void IncrementMessagesEvaluated()
		{
			if (this.counter != null)
			{
				this.counter.MessagesEvaluated.Increment();
			}
		}

		public void IncrementMessagesProcessed()
		{
			if (this.counter != null)
			{
				this.counter.MessagesProcessed.Increment();
			}
		}

		public void CreatePerformanceCounter(string collectionName)
		{
			this.counter = RulesCounters.GetInstance(collectionName + "," + base.Name);
		}

		public void SetDlpPolicy(Guid dlpPolicyId, string dlpPolicyName)
		{
			if (Guid.Empty == dlpPolicyId)
			{
				throw new ArgumentException("dlpId");
			}
			if (string.IsNullOrEmpty(dlpPolicyName))
			{
				throw new ArgumentException("dlpPolicyName");
			}
			base.RemoveAllTags("CP");
			base.RemoveAllTags("CPN");
			base.AddTag(new RuleTag(dlpPolicyId.ToString("D"), "CP"));
			base.AddTag(new RuleTag(dlpPolicyName, "CPN"));
		}

		public bool TryGetDlpPolicy(out Tuple<Guid, string> dlpPolicy)
		{
			dlpPolicy = new Tuple<Guid, string>(Guid.Empty, string.Empty);
			Guid item;
			if (!this.TryGetDlpPolicyId(out item))
			{
				return false;
			}
			IEnumerable<RuleTag> tags = base.GetTags("CPN");
			string empty = string.Empty;
			int num = tags.Count<RuleTag>();
			if (num == 1)
			{
				dlpPolicy = new Tuple<Guid, string>(item, tags.First<RuleTag>().Name);
				return true;
			}
			if (num > 1)
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceWarning<int>(0L, "More than one DLP name found in rule [count = {0}]", num);
				return false;
			}
			dlpPolicy = new Tuple<Guid, string>(item, string.Empty);
			return true;
		}

		public bool TryGetDlpPolicyId(out Guid dlpId)
		{
			IEnumerable<RuleTag> tags = base.GetTags("CP");
			int num = tags.Count<RuleTag>();
			if (num == 1)
			{
				return Guid.TryParse(tags.First<RuleTag>().Name, out dlpId);
			}
			if (num > 1)
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceWarning<int>(0L, "More than one DLP found in rule [count = {0}]", num);
			}
			dlpId = Guid.Empty;
			return false;
		}

		private static Version MaxVersion(Version a, Version b)
		{
			if (!(a < b))
			{
				return a;
			}
			return b;
		}

		private Version ComputeMinVersion()
		{
			Version version = TransportRuleConstants.VersionedContainerBaseVersion;
			version = TransportRule.MaxVersion(version, base.MinimumVersion);
			foreach (Action action in base.Actions)
			{
				version = TransportRule.MaxVersion(version, action.MinimumVersion);
			}
			version = TransportRule.MaxVersion(version, base.Condition.MinimumVersion);
			foreach (RuleBifurcationInfo ruleBifurcationInfo in this.Fork)
			{
				version = TransportRule.MaxVersion(version, ruleBifurcationInfo.MinimumVersion);
			}
			return version;
		}

		private int CountRegexCharsForCondition(Condition condition)
		{
			int num = 0;
			if (condition is AttachmentMatchesRegexPredicate)
			{
				AttachmentMatchesRegexPredicate attachmentMatchesRegexPredicate = condition as AttachmentMatchesRegexPredicate;
				num += attachmentMatchesRegexPredicate.Value.RawValues.Sum((string pattern) => pattern.Length);
			}
			else if (condition is SenderAttributeMatchesRegexPredicate)
			{
				SenderAttributeMatchesRegexPredicate senderAttributeMatchesRegexPredicate = condition as SenderAttributeMatchesRegexPredicate;
				num += senderAttributeMatchesRegexPredicate.Value.RawValues.Sum((string pattern) => pattern.Length);
			}
			else if (condition is LegacyMatchesPredicate)
			{
				LegacyMatchesPredicate legacyMatchesPredicate = (LegacyMatchesPredicate)condition;
				num += legacyMatchesPredicate.Patterns.Sum((string pattern) => pattern.Length);
			}
			else if (condition is TextMatchingPredicate)
			{
				TextMatchingPredicate textMatchingPredicate = (TextMatchingPredicate)condition;
				num += textMatchingPredicate.Patterns.Sum((string pattern) => pattern.Length);
			}
			else
			{
				if (condition is OrCondition)
				{
					OrCondition orCondition = (OrCondition)condition;
					using (List<Condition>.Enumerator enumerator = orCondition.SubConditions.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Condition condition2 = enumerator.Current;
							num += this.CountRegexCharsForCondition(condition2);
						}
						return num;
					}
				}
				if (condition is AndCondition)
				{
					AndCondition andCondition = (AndCondition)condition;
					using (List<Condition>.Enumerator enumerator2 = andCondition.SubConditions.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Condition condition3 = enumerator2.Current;
							num += this.CountRegexCharsForCondition(condition3);
						}
						return num;
					}
				}
				if (condition is NotCondition)
				{
					NotCondition notCondition = (NotCondition)condition;
					num += this.CountRegexCharsForCondition(notCondition.SubCondition);
				}
			}
			return num;
		}

		public static readonly Version SenderAddressLocationVersion = new Version("15.00.0005.004");

		private RulesCountersInstance counter;

		private List<RuleBifurcationInfo> fork;
	}
}
